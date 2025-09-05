using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Tenants;
using TestBucket.Integrations;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Milestones.Services;

/// <summary>
/// Reads milestones from external systems
/// </summary>
internal class MilestoneIndexer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public MilestoneIndexer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(120), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var scope = _serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<MilestoneIndexer>>();
            var tenantRepository = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
            var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
            var projectManager = scope.ServiceProvider.GetRequiredService<IProjectManager>();
            //var dataSources = scope.ServiceProvider.GetRequiredService<IEnumerable<IExternalProjectDataSource>>();
            var milestoneProviders = scope.ServiceProvider.GetRequiredService<IEnumerable<IExternalMilestoneProvider>>();
            var milestoneManager = scope.ServiceProvider.GetRequiredService<IMilestoneManager>();

            try
            {
                await foreach (var tenant in tenantRepository.EnumerateAsync(stoppingToken))
                {
                    var principal = Impersonation.Impersonate(tenant.Id);

                    await foreach(var project in projectRepository.EnumerateAsync(tenant.Id, stoppingToken))
                    {
                        var milestones = await GetExternalMilestonesAsync(principal, project.Id, projectManager, milestoneProviders, stoppingToken);
                        foreach(var milestone in milestones)
                        {
                            if(milestone.Title is null)
                            {
                                continue;
                            }

                            // Try to get by external ID first..
                            Milestone? existingMilestone = null;
                            if (milestone.ExternalId is not null && milestone.ExternalSystemName is not null)
                            {
                                existingMilestone = await milestoneManager.GetMilestoneByExternalIdAsync(principal, project.Id, milestone.ExternalSystemName, milestone.ExternalId);
                            }
                            existingMilestone ??= await milestoneManager.GetMilestoneByNameAsync(principal, project.Id, milestone.Title);

                            if (existingMilestone is null)
                            {
                                await milestoneManager.AddMilestoneAsync(principal, milestone);
                            }
                            else
                            {
                                // Update the milestone
                                existingMilestone.Description = milestone.Description;
                                existingMilestone.EndDate = milestone.EndDate;
                                existingMilestone.State = milestone.State;
                                if(milestone.StartDate is not null)
                                {
                                    existingMilestone.StartDate = milestone.StartDate;
                                }
                                await milestoneManager.UpdateMilestoneAsync(principal, existingMilestone);
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    public async Task<IReadOnlyList<Milestone>> GetExternalMilestonesAsync(ClaimsPrincipal principal, long testProjectId, IProjectManager projectManager, 
        IEnumerable<IExternalMilestoneProvider> milestoneProviders, CancellationToken cancellationToken)
    {
        List<Milestone> milestones = new();

        var integrations = (await projectManager.GetProjectIntegrationsAsync(principal, testProjectId)).ToArray();
        var dtos = integrations.Select(x => x.ToDto()).ToArray();
        foreach (var integration in dtos)
        {
            if ((integration.EnabledCapabilities & ExternalSystemCapability.GetMilestones) == ExternalSystemCapability.GetMilestones)
            {
                var provider = milestoneProviders.FirstOrDefault(x => x.SystemName == integration.Provider);
                if (provider is not null)
                {
                    try
                    {
                        var modified = await provider.GetMilestonesAsync(integration, null, DateTimeOffset.UtcNow, cancellationToken);
                        //var options = await dataSource.GetFieldOptionsAsync(dto, TraitType.Milestone, cancellationToken);
                        if (modified.Count > 0)
                        {
                            foreach(var milestone in modified)
                            {
                                milestones.Add(new Milestone
                                {
                                    Title = milestone.Title,
                                    Description = milestone.Description,
                                    EndDate = milestone.DueDate,
                                    TestProjectId = testProjectId,
                                    ExternalId = milestone.Id.ToString(),
                                    ExternalSystemName = integration.Provider,

                                });
                            }
                        }
                    }
                    catch { }
                }
            }
        }
        return milestones;

    }
}
