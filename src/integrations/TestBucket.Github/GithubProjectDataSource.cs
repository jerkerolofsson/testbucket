using Microsoft.Extensions.Caching.Memory;
using TestBucket.Contracts.Integrations;
using TestBucket.Github.Models;
using TestBucket.Traits.Core;

namespace TestBucket.Github;

public class GithubProjectDataSource : GithubIntegrationBaseClient, IExternalProjectDataSource
{
    public TraitType[] SupportedTraits => [TraitType.Milestone, TraitType.Label];

    public string SystemName => ExtensionConstants.SystemName;
    private readonly IMemoryCache _memoryCache;

    public GithubProjectDataSource(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<GenericVisualEntity[]> GetFieldOptionsAsync(ExternalSystemDto system, TraitType trait, CancellationToken cancellationToken)
    {
        var key = system.BaseUrl + system.AccessToken + system.ExternalProjectId + trait.ToString();

        var result = await _memoryCache.GetOrCreateAsync(key, async (e) =>
        {
            e.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(30);

            var ownerProject = GithubOwnerProject.Parse(system.ExternalProjectId);
            var client = CreateClient(system);

            switch (trait)
            {
                case TraitType.Milestone:

                    var milestones = await client.Issue.Milestone.GetAllForRepository(ownerProject.Owner, ownerProject.Project);
                    return milestones.Select(x => new GenericVisualEntity { Title = x.Title, Description = x.Description }).ToArray();

                case TraitType.Label:
                    var labels = await client.Issue.Labels.GetAllForRepository(ownerProject.Owner, ownerProject.Project);
                    return labels.Select(x => new GenericVisualEntity { Title = x.Name, Description = x.Description, Color = "#" + x.Color }).ToArray();
            }

            return [];
        });

        return result ?? [];
    }
}
