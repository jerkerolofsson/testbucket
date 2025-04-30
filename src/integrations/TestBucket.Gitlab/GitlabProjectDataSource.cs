using Microsoft.Extensions.Caching.Memory;

using NGitLab;
using NGitLab.Models;

using TestBucket.Contracts.Integrations;
using TestBucket.Traits.Core;

namespace TestBucket.Gitlab;

public class GitlabProjectDataSource : IExternalProjectDataSource
{
    /// <summary>
    /// The fields that this class can provide
    /// </summary>
    public TraitType[] SupportedTraits => [TraitType.Milestone, TraitType.Release];

    private readonly IMemoryCache _memoryCache;

    /// <summary>
    /// Name matching the ExternalSystem record
    /// </summary>
    public string SystemName => "GitLab";

    public GitlabProjectDataSource(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Reads the specified options for a trait
    /// </summary>
    /// <param name="system">Gitlab configuration</param>
    /// <param name="trait">The field we should resolve</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string[]> GetFieldOptionsAsync(ExternalSystemDto system, TraitType trait, CancellationToken cancellationToken)
    {
        if (system is not null &&
            system.ExternalProjectId is not null &&
            system.BaseUrl is not null &&
            system.AccessToken is not null &&
            long.TryParse(system.ExternalProjectId, out long projectId))
        {
            var key = system.BaseUrl + system.AccessToken + system.ExternalProjectId + trait.ToString();

            var result = await _memoryCache.GetOrCreateAsync(key, async (e) =>
            {
                e.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(30);
                return await Task.Run(() =>
                {
                    var client = new GitLabClient(system.BaseUrl, system.AccessToken);

                    switch (trait)
                    {
                        case TraitType.Milestone:
                            var milestoneClient = client.GetMilestone(projectId);
                            var milestones = milestoneClient.Get(new MilestoneQuery() { });

                            //var milestones = await client.Projects.GetMilestonesAsync(system.ExternalProjectId);
                            return milestones.Select(x => x.Title).ToArray();

                        case TraitType.Release:
                            var releasesClient = client.GetReleases(projectId);
                            var releases = releasesClient.GetAsync();

                            //var milestones = await client.Projects.GetMilestonesAsync(system.ExternalProjectId);
                            return releases.Select(x => x.Name).ToArray();
                    }
                    return [];
                });
            });

            return result ?? [];
        }

        return [];
    }

}
