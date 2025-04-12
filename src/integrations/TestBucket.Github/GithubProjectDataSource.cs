
using Octokit;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Projects;
using TestBucket.Github.Models;
using TestBucket.Traits.Core;

namespace TestBucket.Github;

public class GithubProjectDataSource : IProjectDataSource
{
    public TraitType[] SupportedTraits => [TraitType.Milestone];

    public string SystemName => ExtensionConstants.SystemName;

    public async Task<string[]> GetFieldOptionsAsync(ExternalSystemDto system, TraitType trait, CancellationToken cancellationToken)
    {
        var ownerProject = GithubOwnerProject.Parse(system.ExternalProjectId);

        var tokenAuth = new Credentials(system.AccessToken);
        var client = new GitHubClient(new ProductHeaderValue("TestBucket"));
        client.Credentials = tokenAuth;

        switch(trait)
        {
            case TraitType.Milestone:
                var milestones = await client.Issue.Milestone.GetAllForRepository(ownerProject.Owner, ownerProject.Project);
                return milestones.Select(x => x.Title).ToArray();
        }

        return [];
    }
}
