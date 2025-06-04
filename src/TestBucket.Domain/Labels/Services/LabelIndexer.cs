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
using TestBucket.Domain.Labels.Models;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Tenants;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Labels.Services;

/// <summary>
/// Reads labels from external systems
/// </summary>
internal class LabelIndexer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public LabelIndexer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(90_000, stoppingToken);

        while(!stoppingToken.IsCancellationRequested)
        {
            var scope = _serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<LabelIndexer>>();
            var tenantRepository = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
            var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
            var projectManager = scope.ServiceProvider.GetRequiredService<IProjectManager>();
            var dataSources = scope.ServiceProvider.GetRequiredService<IEnumerable<IExternalProjectDataSource>>();
            var labelManager = scope.ServiceProvider.GetRequiredService<ILabelManager>();

            try
            {
                await foreach (var tenant in tenantRepository.EnumerateAsync(stoppingToken))
                {
                    var principal = Impersonation.Impersonate(tenant.Id);

                    await foreach(var project in projectRepository.EnumerateAsync(tenant.Id, stoppingToken))
                    {
                        var labels = await GetExternalLabelsAsync(principal, project.Id, projectManager, dataSources, stoppingToken);
                        foreach(var label in labels)
                        {
                            if(label.Title is null)
                            {
                                continue;
                            }
                            var existingLabel = await labelManager.GetLabelByNameAsync(principal, project.Id, label.Title);
                            if(existingLabel is null)
                            {
                                await labelManager.AddLabelAsync(principal, label);
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

    public async Task<IReadOnlyList<Label>> GetExternalLabelsAsync(ClaimsPrincipal principal, long testProjectId, IProjectManager projectManager, 
        IEnumerable<IExternalProjectDataSource> dataSources, CancellationToken cancellationToken)
    {
        if(testProjectId == 2)
        {

        }
        List<Label> labels = new();

        var integrations = (await projectManager.GetProjectIntegrationsAsync(principal, testProjectId)).ToArray();
        var dtos = integrations.Select(x => x.ToDto()).ToArray();
        foreach (var dataSource in dataSources)
        {
            if (dataSource.SupportedTraits.Contains(TraitType.Label))
            {
                var dto = dtos.Where(x => x.Name == dataSource.SystemName && x.Enabled).FirstOrDefault();
                if (dto is not null)
                {
                    try
                    {
                        if ((dto.EnabledCapabilities & ExternalSystemCapability.GetLabels) != ExternalSystemCapability.GetLabels)
                        {
                            continue;
                        }

                        var options = await dataSource.GetFieldOptionsAsync(dto, TraitType.Label, cancellationToken);
                        if (options.Length > 0)
                        {
                            foreach(var label in options)
                            {
                                labels.Add(new Label
                                {
                                    ReadOnly = true,
                                    Title = label.Title,
                                    Color = label.Color,
                                    Description = label.Description,
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
        return labels;
    }
}
