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
        await Task.Delay(TimeSpan.FromSeconds(120));

        while (!stoppingToken.IsCancellationRequested)
        {
            var scope = _serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<MilestoneIndexer>>();
            var tenantRepository = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
            var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
            var projectManager = scope.ServiceProvider.GetRequiredService<IProjectManager>();
            var dataSources = scope.ServiceProvider.GetRequiredService<IEnumerable<IExternalProjectDataSource>>();
            var milestoneManager = scope.ServiceProvider.GetRequiredService<IMilestoneManager>();

            try
            {
                await foreach (var tenant in tenantRepository.EnumerateAsync(stoppingToken))
                {
                    var principal = Impersonation.Impersonate(tenant.Id);

                    await foreach(var project in projectRepository.EnumerateAsync(tenant.Id, stoppingToken))
                    {
                        var milestones = await GetExternalMilestonesAsync(principal, project.Id, projectManager, dataSources, stoppingToken);
                        foreach(var milestone in milestones)
                        {
                            if(milestone.Title is null)
                            {
                                continue;
                            }
                            var existingMilestone = await milestoneManager.GetMilestoneByNameAsync(principal, project.Id, milestone.Title);
                            if(existingMilestone is null)
                            {
                                await milestoneManager.AddMilestoneAsync(principal, milestone);
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
        IEnumerable<IExternalProjectDataSource> dataSources, CancellationToken cancellationToken)
    {
        List<Milestone> milestones = new();

        var integrations = (await projectManager.GetProjectIntegrationsAsync(principal, testProjectId)).ToArray();
        var dtos = integrations.Select(x => x.ToDto()).ToArray();
        foreach (var dataSource in dataSources)
        {
            if (dataSource.SupportedTraits.Contains(TraitType.Milestone))
            {
                var dto = dtos.Where(x => x.Name == dataSource.SystemName && x.Enabled).FirstOrDefault();
                if (dto is not null)
                {
                    try
                    {
                      
                        if ((dto.EnabledCapabilities & ExternalSystemCapability.GetMilestones) != ExternalSystemCapability.GetMilestones)
                        {
                            continue;
                        }

                        var options = await dataSource.GetFieldOptionsAsync(dto, TraitType.Milestone, cancellationToken);
                        if (options.Length > 0)
                        {
                            foreach(var name in options)
                            {
                                milestones.Add(new Milestone
                                {
                                    Title = name.Title,
                                    Description = name.Description,
                                    TestProjectId = testProjectId,
                                    ExternalSystemId = dto.Id,
                                    ExternalSystemName = dataSource.SystemName,
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
