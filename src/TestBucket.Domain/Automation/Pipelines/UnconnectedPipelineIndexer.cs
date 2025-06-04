using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TestBucket.Domain.Automation.Pipelines.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Tenants;
using TestBucket.Domain.Tenants.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestSuites;

namespace TestBucket.Domain.Automation.Pipelines;

/// <summary>
/// Scans CI/CD systems for pipelines that are not started by us, and creating runs for them
/// </summary>
public class UnconnectedPipelineIndexer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UnconnectedPipelineIndexer> _logger;

    public UnconnectedPipelineIndexer(IServiceScopeFactory scopeFactory, ILogger<UnconnectedPipelineIndexer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            var scope = _scopeFactory.CreateScope();
            var tenantRepository = scope.ServiceProvider.GetRequiredService<ITenantRepository>();

            try
            {
                await foreach (var tenant in tenantRepository.EnumerateAsync(stoppingToken))
                {
                    await ProcessTenantAsync(scope, tenant, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing tenant");
            }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ProcessTenantAsync(IServiceScope scope, Tenant tenant, CancellationToken cancellationToken)
    {
        var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();

        await foreach(var project in projectRepository.EnumerateAsync(tenant.Id, cancellationToken))
        {
            await ProcessProjectAsync(scope,project, cancellationToken);
        }
    }

    private async Task ProcessProjectAsync(IServiceScope scope, TestProject project, CancellationToken cancellationToken)
    {
        var user = Impersonation.Impersonate(project.TenantId, project.Id);
        var testSuiteManager = scope.ServiceProvider.GetRequiredService<ITestSuiteManager>();

        await foreach(var testSuite in testSuiteManager.EnumerateAsync(user, cancellationToken))
        {
            if(testSuite.AddPipelinesStartedFromOutside == true)
            {
                await ProcessTestSuiteAsync(scope, user, testSuite, cancellationToken);
            }
        }
    }

    private async Task ProcessTestSuiteAsync(IServiceScope scope, ClaimsPrincipal user, TestSuite testSuite, CancellationToken cancellationToken)
    {
        if(testSuite.TestProjectId is null || testSuite.CiCdSystem is null)
        {
            return;
        }

        var testSuiteManager = scope.ServiceProvider.GetRequiredService<ITestSuiteManager>();
        var pipelineManager = scope.ServiceProvider.GetRequiredService<IPipelineManager>();
        var testrunManager = scope.ServiceProvider.GetRequiredService<ITestRunManager>();
        var runners = await pipelineManager.GetExternalPipelineRunnersAsync(user, testSuite.TestProjectId.Value);
        var runner = runners.Where(x => x.SystemName == testSuite.CiCdSystem).FirstOrDefault();

        if(runner is not null)
        {
            var configs = await pipelineManager.GetIntegrationConfigsAsync(user, testSuite.TestProjectId.Value);
            var config = configs.Where(x => x.Name == runner.SystemName).FirstOrDefault();

            if (config is not null)
            {
                var now = DateTimeOffset.UtcNow;
                DateOnly until = new DateOnly(now.Year, now.Month, now.Day);
                var from = until.AddDays(-3);
                var pipelines = await runner.GetPipelineRunsAsync(config, from, until, cancellationToken);

                // Add missing pipelines
                foreach(var pipeline in pipelines)
                {
                    if (pipeline.CiCdProjectId is null)
                    {
                        _logger.LogWarning("Pipeline returned from integration is missing the CiCdProjectId property");
                        continue;
                    }
                    if (pipeline.CiCdPipelineIdentifier is null)
                    {
                        _logger.LogWarning("Pipeline returned from integration is missing the CiCdPipelineIdentifier property");
                        continue;
                    }


                    var monitorUser = Impersonation.Impersonate(user =>
                    {
                        //project.TenantId, project.Id
                        user.TenantId = testSuite.TenantId;
                        user.ProjectId = testSuite.TestProjectId;
                        user.UserName = config.Name;
                        user.AddAllPermissions();
                    });

                    var existingPipeline = await pipelineManager.GetPipelineByExternalAsync(user, config.Name, pipeline.CiCdProjectId, pipeline.CiCdPipelineIdentifier);
                    if(existingPipeline is null)
                    {
                        var defaultName = $"{config.Name} #{pipeline.CiCdPipelineIdentifier}";
                        var runName = pipeline.DisplayTitle ?? defaultName;

                        // Create new run
                        var run = new TestRun { Name = runName, TestProjectId = testSuite.TestProjectId };
                        await testrunManager.AddTestRunAsync(monitorUser, run);

                        var newPipeline = new Pipeline
                        {
                            DisplayTitle = pipeline.DisplayTitle,
                            TestRunId = run.Id,
                            TestProjectId = run.TestProjectId,
                            TeamId = run.TeamId,
                            CiCdSystem = config.Name,
                            CiCdProjectId = pipeline.CiCdProjectId,
                            CiCdPipelineIdentifier = pipeline.CiCdPipelineIdentifier,
                            Duration = pipeline.Duration,
                        };

                        await pipelineManager.AddAsync(monitorUser, newPipeline);
                    }
                    else if(existingPipeline.Status != Contracts.Automation.PipelineStatus.Completed)
                    {
                        // Attach it


                        await pipelineManager.StartMonitorIfNotMonitoringAsync(monitorUser, existingPipeline);
                    }
                }
            }
        }
    }
}
