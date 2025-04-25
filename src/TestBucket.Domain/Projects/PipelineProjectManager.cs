using System.Security.Claims;

using Microsoft.Extensions.Caching.Memory;

using TestBucket.Contracts.Automation;
using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Automation.Pipelines;
using TestBucket.Domain.Automation.Pipelines.Models;
using TestBucket.Domain.Testing.Compiler;

namespace TestBucket.Domain.Projects;

/// <summary>
/// Manages external CI/CD systems for a project
/// </summary>
internal class PipelineProjectManager : IPipelineProjectManager
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly IProjectRepository _projectRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly IPipelineManager _pipelineManager;
    private readonly List<IExternalPipelineRunner> _pipelineRunners;
    private readonly IProjectTokenGenerator _projectTokenGenerator;
    private readonly ITestCompiler _compiler;

    public PipelineProjectManager(
        ISettingsProvider settingsProvider,
        IProjectRepository projectRepository,
        IMemoryCache memoryCache,
        IPipelineManager pipelineManager,
        IEnumerable<IExternalPipelineRunner> runners,
        IProjectTokenGenerator projectTokenGenerator,
        ITestCompiler compiler)
    {
        _pipelineRunners = runners.ToList();
        _settingsProvider = settingsProvider;
        _projectRepository = projectRepository;
        _memoryCache = memoryCache;
        _pipelineManager = pipelineManager;
        _projectTokenGenerator = projectTokenGenerator;
        _compiler = compiler;
    }


    private async Task AddDefaultAutomationVariablesAsync(TestExecutionContext context)
    {
        // Access token
        if (context.TenantId is not null)
        {
            context.Variables["TB_TOKEN"] = await _projectTokenGenerator.GenerateCiCdAccessTokenAsync(context.TenantId, context.ProjectId, context.TestRunId, context.TestSuiteId);
            context.Variables["TB_TENANT_ID"] = context.TenantId.ToString();
        }

        // URL for CI/CD pipeline to post back the results.
        // If we are behind a reverse proxy or similar we cannot rely on detection, so the user could configure this with
        // an environment variable
        var settings = await _settingsProvider.LoadGlobalSettingsAsync();
        var endpoint = settings.PublicEndpointUrl;
        if (endpoint is not null)
        {
            context.Variables[TestBucketEnvironmentVariables.TB_PUBLIC_ENDPOINT] = endpoint;
        }
    }

    public async Task CreatePipelineAsync(ClaimsPrincipal principal, TestExecutionContext context)
    {
        // Assign variables and lock resources for the execution
        await AssignVariablesAsync(principal, context);

        ExternalSystemDto[] configs = await _pipelineManager.GetIntegrationConfigsAsync(principal, context.ProjectId);
        var config = configs.Where(x => x.Id == context.CiCdExternalSystemId).FirstOrDefault();
        if (config is null)
        {
            throw new InvalidOperationException($"There is no external system configuration registered with the system ID: {context.CiCdSystem}");
        }

        var runner = _pipelineRunners.Where(x => x.SystemName == context.CiCdSystem).FirstOrDefault();
        if (runner is null)
        {
            throw new InvalidOperationException($"There is no IExternalPipelineRunner registered with the system ID: {context.CiCdSystem}");
        }

        try
        {
            await runner.CreateAsync(config, context, CancellationToken.None);

            if (context.CiCdPipelineIdentifier is not null)
            {
                // Add a pipeline
                var pipeline = new Pipeline
                {
                    Status = PipelineStatus.Created,
                    CiCdPipelineIdentifier = context.CiCdPipelineIdentifier,
                    CiCdProjectId = config.ExternalProjectId,
                    CiCdSystem = runner.SystemName,
                    TestRunId = context.TestRunId,
                    TestProjectId = context.ProjectId,
                    TeamId = context.TeamId,
                    TenantId = context.TenantId,
                };
                await _pipelineManager.AddAsync(principal, pipeline);
            }
        }
        catch(Exception ex)
        {
            var pipeline = new Pipeline
            {
                Status = PipelineStatus.Error,
                CiCdProjectId = config.ExternalProjectId,
                CiCdSystem = runner.SystemName,
                StartError = ex.Message,
                TestRunId = context.TestRunId,
                TestProjectId = context.ProjectId,
                TeamId = context.TeamId,
                TenantId = context.TenantId,
            };
            await _pipelineManager.AddAsync(principal, pipeline);
        }
    }

    private async Task AssignVariablesAsync(ClaimsPrincipal principal, TestExecutionContext context, CancellationToken cancellationToken = default)
    {
        // Create a copy of the variables. These variables are provided by the user and has higher prio than what is assigned
        var testSuiteVariables = context.Variables.ToDictionary();

        // Add variables from environment
        await _compiler.ResolveVariablesAsync(principal, context, cancellationToken);

        // Overwrite any variables..
        foreach (var variable in testSuiteVariables)
        {
            context.Variables[variable.Key] = variable.Value;
        }

        // Add some variables including token to authorize the pipeline to post back the result
        await AddDefaultAutomationVariablesAsync(context);
    }
}
