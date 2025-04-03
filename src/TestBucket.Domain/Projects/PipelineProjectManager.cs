using System.Security.Claims;

using Microsoft.Extensions.Caching.Memory;

using OneOf.Types;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Projects;
using TestBucket.Domain.Projects.Models;

namespace TestBucket.Domain.Projects;

/// <summary>
/// Manages external CI/CD systems for a project
/// </summary>
internal class PipelineProjectManager : IPipelineProjectManager
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly List<IExternalPipelineRunner> _pipelineRunners;

    public PipelineProjectManager(IProjectRepository projectRepository,
        IMemoryCache memoryCache,
        IEnumerable<IExternalPipelineRunner> runners)
    {
        _pipelineRunners = runners.ToList();
        _projectRepository = projectRepository;
        _memoryCache = memoryCache;
    }

    private async Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(ClaimsPrincipal principal, long testProjectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();

        var key = "integration:" + testProjectId + ":" + tenantId;
        var result = await _memoryCache.GetOrCreateAsync(key, async (e) =>
        {
            e.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1);
            return await _projectRepository.GetProjectIntegrationsAsync(tenantId, testProjectId);
        });

        return result!;
    }
    public async Task<IReadOnlyList<IExternalPipelineRunner>> GetExternalPipelineRunnersAsync(ClaimsPrincipal principal, long testProjectId)
    {
        ExternalSystemDto[] configs = await GetIntegrationConfigsAsync(principal, testProjectId);

        var result = new List<IExternalPipelineRunner>();
        foreach (var integration in _pipelineRunners)
        {
            var config = configs.Where(x => x.Name == integration.SystemName).FirstOrDefault();
            if (config is not null)
            {
                result.Add(integration);
            }
        }
        return result;
    }

    private async Task<ExternalSystemDto[]> GetIntegrationConfigsAsync(ClaimsPrincipal principal, long testProjectId)
    {
        var integrations = (await GetProjectIntegrationsAsync(principal, testProjectId)).ToArray();
        var configs = integrations.Select(x => new ExternalSystemDto
        {
            Name = x.Name,
            AccessToken = x.AccessToken,
            BaseUrl = x.BaseUrl,
            ExternalProjectId = x.ExternalProjectId,
            ReadOnly = x.ReadOnly

        }).ToArray();
        return configs;
    }

    public async Task CreatePipelineAsync(ClaimsPrincipal principal, TestExecutionContext context)
    {
        // Add some variables
        context.Variables["TB_RUN_ID"] = context.TestRunId.ToString();

        ExternalSystemDto[] configs = await GetIntegrationConfigsAsync(principal, context.ProjectId);
        var config = configs.Where(x => x.Name == context.CiCdSystem).FirstOrDefault();
        if (config is null)
        {
            throw new InvalidOperationException($"There is no external system configuration registered with the system ID: {context.CiCdSystem}");
        }

        var runner = _pipelineRunners.Where(x => x.SystemName == context.CiCdSystem).FirstOrDefault();
        if(runner is null)
        {
            throw new InvalidOperationException($"There is no IExternalPipelineRunner registered with the system ID: {context.CiCdSystem}");
        }

        await runner.StartAsync(config, context, CancellationToken.None);

        // Todo: add pipeline..
        if(context.CiCdPipelineIdentifier is not null)
        {

        }
    }
}
