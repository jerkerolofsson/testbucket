using System.Security.Claims;

using Mediator;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Automation;
using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Automation.Artifact.Events;
using TestBucket.Domain.Automation.Mapping;
using TestBucket.Domain.Automation.Pipelines.Models;
using TestBucket.Domain.Automation.Pipelines.Specifications;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Events;

namespace TestBucket.Domain.Automation.Pipelines;
internal class PipelineManager : IPipelineManager
{
    private readonly ILogger<PipelineManager> _logger;
    private readonly IPipelineRepository _repository;
    private readonly IProjectRepository _projectRepository;
    private readonly List<IExternalPipelineRunner> _pipelineRunners;
    private readonly IMemoryCache _memoryCache;
    private readonly IMediator _mediator;

    private readonly List<IPipelineObserver> _observers = [];

    public PipelineManager(
        ILogger<PipelineManager> logger,
        IPipelineRepository repository,
        IProjectRepository projectRepository,
        IEnumerable<IExternalPipelineRunner> runners,
        IMemoryCache memoryCache,
        IMediator mediator)
    {
        _pipelineRunners = runners.ToList();
        _logger = logger;
        _repository = repository;
        _projectRepository = projectRepository;
        _memoryCache = memoryCache;
        _mediator = mediator;
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

    /// <summary>
    /// Gets all project integrations
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testProjectId"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Gets all runners
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testProjectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<IExternalPipelineRunner>> GetExternalPipelineRunnersAsync(ClaimsPrincipal principal, long testProjectId)
    {
        ExternalSystemDto[] configs = await GetIntegrationConfigsAsync(principal, testProjectId);

        var result = new List<IExternalPipelineRunner>();
        foreach (var integration in _pipelineRunners)
        {
            var config = configs.Where(x => x.Provider == integration.SystemName && x.Enabled).FirstOrDefault();
            if (config is not null)
            {
                result.Add(integration);
            }
        }
        return result;
    }

    /// <summary>
    /// Gets all configurations as DTOs (this is a DTO mapping of GetIntegrationConfigsAsync)
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testProjectId"></param>
    /// <returns></returns>
    public async Task<ExternalSystemDto[]> GetIntegrationConfigsAsync(ClaimsPrincipal principal, long testProjectId)
    {
        var integrations = (await GetProjectIntegrationsAsync(principal, testProjectId)).ToArray();
        var configs = integrations.Select(x => x.ToDto()).ToArray();
        return configs;
    }

    /// <summary>
    /// Refreshes the status of a pipeline
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="pipelineId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal async Task<Pipeline?> RefreshStatusAsync(ClaimsPrincipal principal, long pipelineId, CancellationToken cancellationToken)
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

                    await OnPipelineUpdatedAsync(principal, pipeline, dto);
                }
            }

            return pipeline;
        }
        return null;
    }

    private async Task OnPipelineUpdatedAsync(ClaimsPrincipal principal, Pipeline pipeline, PipelineDto dto)
    {
        // Update test run fields
        if(dto.HeadCommit is not null && pipeline.TestRunId is not null)
        {
            await _mediator.Send(new SetCommitForRunRequest(principal, pipeline.TestRunId.Value, dto.HeadCommit));
        }

        // Notify observers
        foreach (var observer in _observers)
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
        var config = configs.Where(x => x.Provider == pipeline.CiCdSystem && x.ExternalProjectId == pipeline.CiCdProjectId).FirstOrDefault();
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

    private async Task<ConfiguredExternalService<IExternalPipelineRunner>?> ConfigurePipelineRunnerAsync(ClaimsPrincipal principal, Pipeline pipeline)
    {
        if (pipeline.CiCdPipelineIdentifier is null || pipeline.TestProjectId is null)
        {
            return null;
        }

        ExternalSystemDto[] configs = await GetIntegrationConfigsAsync(principal, pipeline.TestProjectId.Value);
        var config = configs.Where(x => x.Provider == pipeline.CiCdSystem && x.ExternalProjectId == pipeline.CiCdProjectId).FirstOrDefault();
        if (config is null)
        {
            return null;
        }

        var runner = GetRunner(pipeline.CiCdSystem);
        if(runner is null)
        {
            return null;
        }
        return new ConfiguredExternalService<IExternalPipelineRunner>(config, runner);
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
        var configuredRunner = await ConfigurePipelineRunnerAsync(principal, pipeline);
        if (configuredRunner is not null && pipeline.CiCdPipelineIdentifier is not null)
        {
            return await configuredRunner.Service.GetPipelineAsync(configuredRunner.Config, pipeline.CiCdPipelineIdentifier, cancellationToken);
        }
        return null;
    }

    /// <summary>
    /// Returns all pipelines for the specified test run
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testRunId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<Pipeline>> GetPipelinesForTestRunAsync(ClaimsPrincipal principal, long testRunId)
    {
        var tenantId = principal.GetTenantIdOrThrow();

        FilterSpecification<Pipeline>[] filters = [new FilterByTenant<Pipeline>(tenantId), new FilterPipelinesByTestRun(testRunId)];

        return await _repository.GetPipelinesForTestRunAsync(filters, testRunId);
    }

    public async Task<Pipeline?> GetPipelineByExternalAsync(ClaimsPrincipal principal, string ciCdSystemName, string ciCdProjectId, string ciCdPipelineId)
    {
        var tenantId = principal.GetTenantIdOrThrow();

        FilterSpecification<Pipeline>[] filters = 
        [
            new FilterByTenant<Pipeline>(tenantId), 
            new FilterPipelinesByExternalComponents(ciCdSystemName, ciCdProjectId, ciCdPipelineId)
        ];

        var items = await _repository.SearchAsync(filters, 0, 1);
        return items.Items.FirstOrDefault();
    }

    /// <summary>
    /// Returns a pipeline by database ID
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="id"></param>
    /// <returns></returns>

    public async Task<Pipeline?> GetPipelineByIdAsync(ClaimsPrincipal principal, long id)
    {
        var pipeline = await _repository.GetByIdAsync(id);
        if (pipeline is { })
        { 
            principal.ThrowIfEntityTenantIsDifferent(pipeline.TenantId);
        }
        return pipeline;
    }

    internal async Task OnPipelineCompletedAsync(ClaimsPrincipal principal, Pipeline pipeline)
    {
        _logger.LogInformation("Pipeline completed: {CiCdSystem} #{CiCdPipelineIdentifier}", pipeline.CiCdSystem, pipeline.CiCdPipelineIdentifier);

        await DownloadArtifactsFromPipelineAsync(principal, pipeline);

        if(pipeline.TestRunId is not null)
        {
            await CloseRunAsync(principal, pipeline.TestRunId.Value);
        }
    }

    private async Task CloseRunAsync(ClaimsPrincipal principal, long testRunId)
    {
        await _mediator.Send(new CloseRunRequest(principal, testRunId));
    }

    private async Task DownloadArtifactsFromPipelineAsync(ClaimsPrincipal principal, Pipeline pipeline)
    {
        var configuredRunner = await ConfigurePipelineRunnerAsync(principal, pipeline);
        if (configuredRunner is not null &&
            (configuredRunner.Config.SupportedCapabilities & ExternalSystemCapability.ReadPipelineArtifacts) == ExternalSystemCapability.ReadPipelineArtifacts &&
            configuredRunner.Config.TestResultsArtifactsPattern is not null &&
            pipeline.PipelineJobs is not null)
        {
            foreach (var job in pipeline.PipelineJobs)
            {
                if (job.HasArtifacts && job.CiCdJobIdentifier is not null && pipeline.CiCdPipelineIdentifier is not null)
                {
                    // Add as attachments to the test case
                    try
                    {
                        var bytes = await configuredRunner.Service.GetArtifactsZipAsByteArrayAsync(configuredRunner.Config,
                            pipeline.CiCdPipelineIdentifier,
                            job.CiCdJobIdentifier,
                             configuredRunner.Config.TestResultsArtifactsPattern,
                            CancellationToken.None);

                        if (bytes.Length > 0 && pipeline.TestRunId is not null && pipeline.TenantId is not null)
                        {
                            // Sends event that will scan the artifact zip for test results and add them to the run as attachments
                            await _mediator.Publish(new JobArtifactDownloaded(principal, pipeline.TestRunId.Value, configuredRunner.Config.TestResultsArtifactsPattern, bytes));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Artifact has expired
                        _logger.LogError(ex, "Failed to download and process artifact from CI/CD system");
                    }
                }
            }
        }
    }
}
