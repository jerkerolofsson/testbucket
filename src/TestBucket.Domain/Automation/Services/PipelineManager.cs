using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Automation;
using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Projects;
using TestBucket.Domain.Automation.Mapping;
using TestBucket.Domain.Automation.Models;
using TestBucket.Domain.Automation.Specifications.Pipelines;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing;

namespace TestBucket.Domain.Automation.Services;
internal class PipelineManager : IPipelineManager
{
    private readonly ILogger<PipelineManager> _logger;
    private readonly IPipelineRepository _repository;
    private readonly IProjectRepository _projectRepository;
    private readonly List<IExternalPipelineRunner> _pipelineRunners;
    private readonly IMemoryCache _memoryCache;

    private readonly List<IPipelineObserver> _observers = [];

    public PipelineManager(
        ILogger<PipelineManager> logger,
        IPipelineRepository repository,
        IProjectRepository projectRepository,
        IEnumerable<IExternalPipelineRunner> runners,
        IMemoryCache memoryCache)
    {
        _pipelineRunners = runners.ToList();
        _logger = logger;
        _repository = repository;
        _projectRepository = projectRepository;
        _memoryCache = memoryCache;
    }


    #region Observers
    /// <summary>
    /// Adds an observer
    /// </summary>
    /// <param name="listener"></param>
    public void AddObserver(IPipelineObserver observer) => _observers.Add(observer);

    /// <summary>
    /// Removes an observer
    /// </summary>
    /// <param name="observer"></param>
    public void RemoveObserver(IPipelineObserver observer) => _observers.Remove(observer);
    #endregion Observers

    public async Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(ClaimsPrincipal principal, long testProjectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();

        var key = "integration:" + testProjectId + ":" + tenantId;
        var result = await _memoryCache.GetOrCreateAsync(key, async (e) =>
        {
            e.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1);
            var integrations = await _projectRepository.GetProjectIntegrationsAsync(tenantId, testProjectId);
            return integrations;
        });

        return result!;
    }
    public async Task<IReadOnlyList<IExternalPipelineRunner>> GetExternalPipelineRunnersAsync(ClaimsPrincipal principal, long testProjectId)
    {
        ExternalSystemDto[] configs = await GetIntegrationConfigsAsync(principal, testProjectId);

        var result = new List<IExternalPipelineRunner>();
        foreach (var integration in _pipelineRunners)
        {
            var config = configs.Where(x => x.Name == integration.SystemName && x.Enabled).FirstOrDefault();
            if (config is not null)
            {
                result.Add(integration);
            }
        }
        return result;
    }

    public async Task<ExternalSystemDto[]> GetIntegrationConfigsAsync(ClaimsPrincipal principal, long testProjectId)
    {
        var integrations = (await GetProjectIntegrationsAsync(principal, testProjectId)).ToArray();
        var configs = integrations.Select(x => x.ToDto()).ToArray();
        return configs;
    }

    internal async Task<PipelineDto?> RefreshStatusAsync(ClaimsPrincipal principal, long pipelineId, CancellationToken cancellationToken)
    {
        Pipeline? pipeline = await _repository.GetByIdAsync(pipelineId);
        if (pipeline is not null)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var dto = await GetPipelineStatusAsync(principal, pipeline, cts.Token);

            if (dto is not null)
            {
                var changed = dto.CopyTo(pipeline);
                if (changed)
                {
                    await _repository.UpdateAsync(pipeline);

                    await OnPipelineUpdatedAsync(pipeline);
                }
            }

            return dto;
        }
        return null;
    }

    private async Task OnPipelineUpdatedAsync(Pipeline pipeline)
    {
        foreach(var observer in _observers)
        {
            try
            {
                await observer.OnPipelineUpdatedAsync(pipeline);
            }
            catch { } // External code may throw, ignore it
        }
    }

    /// <summary>
    /// Returns the job log
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="pipeline"></param>
    /// <param name="pipelineJobIdentifier"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string?> ReadTraceAsync(ClaimsPrincipal principal, Pipeline pipeline, string pipelineJobIdentifier, CancellationToken cancellationToken)
    {
        if (pipeline.CiCdPipelineIdentifier is null || pipeline.TestProjectId is null)
        {
            return null;
        }

        ExternalSystemDto[] configs = await GetIntegrationConfigsAsync(principal, pipeline.TestProjectId.Value);
        var config = configs.Where(x => x.Name == pipeline.CiCdSystem).FirstOrDefault();
        if (config is null)
        {
            return null;
        }

        var runner = GetRunner(pipeline.CiCdSystem);
        if (runner is not null)
        {
            return await runner.ReadTraceAsync(config, pipelineJobIdentifier, cancellationToken);
        }
        return null;
    }

    /// <summary>
    /// Adds a new pipeline in the repository and begins to monitor the state
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="pipeline"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task AddAsync(ClaimsPrincipal principal, Pipeline pipeline)
    {
        pipeline.Created = DateTimeOffset.UtcNow;
        pipeline.Modified = DateTimeOffset.UtcNow;
        pipeline.CreatedBy = principal.Identity?.Name ?? throw new Exception("No identity in current principal");
        pipeline.ModifiedBy = principal.Identity?.Name ?? throw new Exception("No identity in current principal");
        pipeline.TenantId = principal.GetTenantIdOrThrow();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        try
        {
            var dto = await GetPipelineStatusAsync(principal, pipeline, cts.Token);
            dto?.CopyTo(pipeline);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch pipeline status");
        }

        await _repository.AddAsync(pipeline);

        var monitor = new PipelineMonitor(this, principal, pipeline);
        monitor.Start();
    }

    private IExternalPipelineRunner? GetRunner(string? systemName)
    {
        if(systemName is null)
        {
            return null;
        }
        return _pipelineRunners.Where(x => x.SystemName == systemName).FirstOrDefault();
    }

    /// <summary>
    /// Returns the latest pipeline status by calling the external runner and collecting pipeline and job status
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="pipeline"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<PipelineDto?> GetPipelineStatusAsync(ClaimsPrincipal principal, Pipeline pipeline, CancellationToken cancellationToken)
    {
        if(pipeline.CiCdPipelineIdentifier is null || pipeline.TestProjectId is null)
        {
            return null;
        }

        ExternalSystemDto[] configs = await GetIntegrationConfigsAsync(principal, pipeline.TestProjectId.Value);
        var config = configs.Where(x => x.Name == pipeline.CiCdSystem).FirstOrDefault();
        if (config is null)
        {
            return null;
        }

        var runner = GetRunner(pipeline.CiCdSystem);
        if (runner is not null)
        {
            return await runner.GetPipelineAsync(config, pipeline.CiCdPipelineIdentifier, cancellationToken);
        }
        return null;
    }

    public async Task<IReadOnlyList<Pipeline>> GetPipelinesForTestRunAsync(ClaimsPrincipal principal, long testRunId)
    {
        var tenantId = principal.GetTenantIdOrThrow();

        FilterSpecification<Pipeline>[] filters = [new FilterByTenant<Pipeline>(tenantId), new FilterPipelinesByTestRun(testRunId)];

        return await _repository.GetPipelinesForTestRunAsync(filters, testRunId);
    }

    public async Task<Pipeline?> GetPipelineByIdAsync(ClaimsPrincipal principal, long id)
    {
        var pipeline = await _repository.GetByIdAsync(id);
        if (pipeline is { })
        { 
            principal.ThrowIfEntityTenantIsDifferent(pipeline.TenantId);
        }
        return pipeline;
    }
}
