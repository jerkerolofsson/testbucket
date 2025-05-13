using Microsoft.Extensions.Logging;

using Octokit;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Issues.Models;
using TestBucket.Github.Mapping;
using TestBucket.Github.Models;

using static System.Net.Mime.MediaTypeNames;

namespace TestBucket.Github.Issues;
public class GithubIssues : GithubIntegrationBaseClient, IExternalIssueProvider
{
    public string SystemName => ExtensionConstants.SystemName;

    private readonly ILogger<GithubIssues> _logger;

    public GithubIssues(ILogger<GithubIssues> logger)
    {
        _logger = logger;
    }

    public async Task<IReadOnlyList<IssueDto>> SearchAsync(ExternalSystemDto system, string? text, int offset, int count, CancellationToken cancellationToken)
    {
        var ownerProject = GithubOwnerProject.Parse(system.ExternalProjectId);
        GitHubClient client = CreateClient(system);

        List<IssueDto> dtos = [];

        var request = new RepositoryIssueRequest { Filter = IssueFilter.All };
        var apiOptions = new ApiOptions { PageCount = count/ offset, PageSize = count };
        var issues = await client.Issue.GetAllForRepository(ownerProject.Owner, ownerProject.Project, request, apiOptions);

        foreach(var issue in issues)
        {
            if(text is null || (issue.Title is not null && issue.Title.Contains(text, StringComparison.InvariantCultureIgnoreCase)))
            {
                dtos.Add(issue.ToDto(system.Id));
            }
            else if (issue.Body is not null && issue.Body.Contains(text, StringComparison.InvariantCultureIgnoreCase))
            {
                dtos.Add(issue.ToDto(system.Id));
            }
            if(issues.Count >= count)
            {
                break;
            }
        }

        return dtos;
    }

    public async Task<IssueDto?> GetIssueAsync(ExternalSystemDto system, string externalIssueId, CancellationToken cancellationToken)
    {
        var ownerProject = GithubOwnerProject.Parse(system.ExternalProjectId);
        GitHubClient client = CreateClient(system);
        if (long.TryParse(externalIssueId, out var id))
        {
            var issue = await client.Issue.Get(ownerProject.Owner, ownerProject.Project, id);
            return issue.ToDto(system.Id);

        }
        return null;
    }

    public async Task<IReadOnlyList<IssueDto>> GetIssuesAsync(ExternalSystemDto system, DateTimeOffset? from, DateTimeOffset until, CancellationToken cancellationToken)
    {
        var ownerProject = GithubOwnerProject.Parse(system.ExternalProjectId);
        GitHubClient client = CreateClient(system);

        List<IssueDto> dtos = [];

        var request = new RepositoryIssueRequest { Filter = IssueFilter.All };
        if(from is not null)
        {
            request.Since = from.Value.UtcDateTime;
        }
        var issues = await client.Issue.GetAllForRepository(ownerProject.Owner, ownerProject.Project, request);

        foreach (var issue in issues)
        {
            dtos.Add(issue.ToDto(system.Id));
            
        }

        return dtos;
    }
}