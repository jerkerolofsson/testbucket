
using Octokit;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Projects;
using TestBucket.Traits.Core;

namespace TestBucket.Github;

public class GithubProjectDataSource : IProjectDataSource
{
    public TraitType[] SupportedTraits => [TraitType.Milestone];

    public string SystemName => ExtensionConstants.SystemName;

    public async Task<string[]> GetFieldOptionsAsync(ExternalSystemDto system, TraitType trait, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(system.ExternalProjectId);

        var orgAndProject = system.ExternalProjectId.Split('/');
        if(orgAndProject.Length != 2)
        {
            throw new ArgumentException("Expected project ID to be in the format organization/project");
        }

        var tokenAuth = new Credentials(system.AccessToken);
        var client = new GitHubClient(new ProductHeaderValue("TestBucket"));
        client.Credentials = tokenAuth;

        switch(trait)
        {
            case TraitType.Milestone:
                var milestones = await client.Issue.Milestone.GetAllForRepository(orgAndProject[0], orgAndProject[1]);
                return milestones.Select(x => x.Title).ToArray();
        }

        return [];
    }
}
