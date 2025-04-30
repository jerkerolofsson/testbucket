using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.ExtensionManagement;
internal class ExtensionFieldCompletionsAggregator : IFieldCompletionsProvider
{
    private readonly IReadOnlyList<IExternalProjectDataSource> _projectDataSources;
    private readonly IProjectManager _projectManager;

    public ExtensionFieldCompletionsAggregator(
        IProjectManager projectManager,
        IEnumerable<IExternalProjectDataSource> projectDataSources)
    {
        _projectManager = projectManager;
        _projectDataSources = projectDataSources.ToList();

    }

    public async Task<IReadOnlyList<string>> GetFieldOptionsAsync(ClaimsPrincipal principal, long testProjectId, TraitType traitType, CancellationToken cancellationToken)
    {
        var integrations = (await _projectManager.GetProjectIntegrationsAsync(principal, testProjectId)).ToArray();
        var dtos = integrations.Select(x => x.ToDto()).ToArray();
        foreach (var dataSource in _projectDataSources)
        {
            if (dataSource.SupportedTraits.Contains(traitType))
            {
                var dto = dtos.Where(x => x.Name == dataSource.SystemName && x.Enabled).FirstOrDefault();
                if (dto is not null)
                {
                    try
                    {
                        // Verify that the capability is enabled for the data source
                        if (traitType == TraitType.Release &&
                            (dto.EnabledCapabilities & ExternalSystemCapability.GetReleases) != ExternalSystemCapability.GetReleases)
                        {
                            continue;
                        }
                        if (traitType == TraitType.Milestone && (dto.EnabledCapabilities & ExternalSystemCapability.GetMilestones) != ExternalSystemCapability.GetMilestones)
                        {
                            continue;
                        }

                        var options = await dataSource.GetFieldOptionsAsync(dto, traitType, cancellationToken);
                        if (options.Length > 0)
                        {
                            return options;
                        }
                    }
                    catch { }
                }
            }
        }
        return [];
    }
    public async Task<IReadOnlyList<string>> GetOptionsAsync(ClaimsPrincipal principal, FieldDataSourceType type, long projectId, CancellationToken cancellationToken)
    {
        switch(type)
        {
            case FieldDataSourceType.Milestones:
                return await GetFieldOptionsAsync(principal, projectId, TraitType.Milestone, cancellationToken);
            case FieldDataSourceType.Releases:
                return await GetFieldOptionsAsync(principal, projectId, TraitType.Release, cancellationToken);
        }
        return [];
    }

    public async Task<IReadOnlyList<string>> SearchOptionsAsync(ClaimsPrincipal principal, FieldDataSourceType type, long projectId, string text, int count, CancellationToken cancellationToken)
    {
        var options = await GetOptionsAsync(principal, type, projectId, cancellationToken);
        return options.Take(count).ToList();
    }
}
