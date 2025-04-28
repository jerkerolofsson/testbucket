using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Octokit;

using TestBucket.Contracts.Code.Models;
using TestBucket.Contracts.Integrations;
using TestBucket.Github.Mapping;
using TestBucket.Github.Models;

namespace TestBucket.Github;
public class GithubRepository : GithubIntegrationBaseClient, ICodeRepository
{
    public string SystemName => ExtensionConstants.SystemName;

    private readonly ILogger<GithubRepository> _logger;

    public GithubRepository(ILogger<GithubRepository> logger)
    {
        _logger = logger;
    }

    public async Task<RepositoryDto> GetRepository(ExternalSystemDto system, CancellationToken cancellationToken)
    {
        var ownerProject = GithubOwnerProject.Parse(system.ExternalProjectId);
        GitHubClient client = CreateClient(system);
        var repo = await client.Repository.Get(ownerProject.Owner, ownerProject.Project);
        return repo.ToDto();

    }

    public async Task<IReadOnlyList<CommitDto>> GetCommitsAsync(ExternalSystemDto system, string? sha, DateTimeOffset? since, DateTimeOffset? until, CancellationToken cancellationToken)
    {
        var request = new CommitRequest
        {
            Since = since,
            Until = until,
            Sha = sha,
        };

        var ownerProject = GithubOwnerProject.Parse(system.ExternalProjectId);
        GitHubClient client = CreateClient(system);
        var commits = await client.Repository.Commit.GetAll(ownerProject.Owner, ownerProject.Project, request);
        return commits.Select(x => x.ToDto()).ToList();
    }

    public async Task<CommitDto> GetCommitAsync(ExternalSystemDto system, string reference, CancellationToken cancellationToken)
    {
        var ownerProject = GithubOwnerProject.Parse(system.ExternalProjectId);
        GitHubClient client = CreateClient(system);
        GitHubCommit commit = await client.Repository.Commit.Get(ownerProject.Owner, ownerProject.Project, reference);
        return commit.ToDto();
    }
}