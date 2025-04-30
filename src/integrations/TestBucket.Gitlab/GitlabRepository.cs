
using Microsoft.Extensions.Logging;

using NGitLab.Models;
using NGitLab;

using TestBucket.Contracts.Code.Models;
using TestBucket.Contracts.Integrations;
using TestBucket.Gitlab;
using TestBucket.Gitlab.Mapping;

namespace TestBucket.Github;
public class GitlabRepository : IExternalCodeRepository
{
    public string SystemName => ExtensionConstants.SystemName;

    private readonly ILogger<GitlabRepository> _logger;

    public GitlabRepository(ILogger<GitlabRepository> logger)
    {
        _logger = logger;
    }

    public Task<RepositoryDto> GetRepositoryAsync(ExternalSystemDto config, CancellationToken cancellationToken)
    {
        return Task.FromResult(new RepositoryDto()
        {
            Url = config.BaseUrl ?? throw new Exception("Gitlab base url not defined"),
            ExternalId = config.ExternalProjectId
        });
    }

    public async Task<IReadOnlyList<CommitDto>> GetCommitsAsync(ExternalSystemDto config, string? sha, DateTimeOffset? since, DateTimeOffset? until, CancellationToken cancellationToken)
    {
        if (long.TryParse(config.ExternalProjectId, out var projectId))
        {
            if(sha is not null)
            {
                return [await GetCommitAsync(config, sha, cancellationToken)];
            }

            var request = new GetCommitsRequest
            {
                Since = since?.DateTime,
                Until = until?.DateTime,
            };

            var client = new GitLabClient(config.BaseUrl, config.AccessToken);
            var repository = client.GetRepository(projectId);
            List<CommitDto> commits = [];
            try
            {
                foreach (var gitlabCommit in repository.GetCommits(request))
                {
                    commits.Add(gitlabCommit.ToDto());
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error getting commits from Gitlab");
            }
            return commits;
        }
        else
        {
            _logger.LogWarning("Invalid Gitlab project ID: {ExternalProjectId}. Expected an int64", config.ExternalProjectId);
            throw new ArgumentException("Invalid Gitlab project ID");
        }
    }

    public Task<CommitDto> GetCommitAsync(ExternalSystemDto config, string reference, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(reference);
        if (long.TryParse(config.ExternalProjectId, out var projectId))
        {
            Sha1 sha1 = new Sha1(reference);
            var client = new GitLabClient(config.BaseUrl, config.AccessToken);
            var repository = client.GetRepository(projectId);
            var gitlabCommit = repository.GetCommit(sha1);
            var dto = gitlabCommit.ToDto();

            var files = new List<CommitFileDto>();
            try
            {
                foreach (var diff in repository.GetCommitDiff(sha1))
                {
                    files.Add(diff.ToDto(reference));
                }
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error fetching commit diff from Gitlab");
            }
            dto.Files = files;

            return Task.FromResult(dto);
        }
        else
        {
            _logger.LogWarning("Invalid Gitlab project ID: {ExternalProjectId}. Expected an int64", config.ExternalProjectId);
            throw new ArgumentException("Invalid Gitlab project ID");
        }
    }
}