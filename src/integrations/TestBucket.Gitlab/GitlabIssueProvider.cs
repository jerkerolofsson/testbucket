using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NGitLab;
using NGitLab.Models;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Issues.Models;

namespace TestBucket.Gitlab;
public class GitlabIssueProvider : IExternalIssueProvider
{
    public string SystemName => ExtensionConstants.SystemName;

    public async Task<IssueDto?> GetIssueAsync(ExternalSystemDto config, string externalIssueId, CancellationToken cancellationToken)
    {
        if (long.TryParse(externalIssueId, out var id))
        {
            var client = new GitLabClient(config.BaseUrl, config.AccessToken);
            var gitlabIssue = await client.Issues.GetByIdAsync(id, cancellationToken);
            if (gitlabIssue is not null)
            {
                return MapToDto(config, id, gitlabIssue);
            }
        }
        return null;
    }

    private static IssueDto MapToDto(ExternalSystemDto config, long id, NGitLab.Models.Issue gitlabIssue)
    {
        return new IssueDto()
        {
            Title = gitlabIssue.Title,
            Description = gitlabIssue.Description,
            ExternalSystemName = ExtensionConstants.SystemName,
            ExternalId = id.ToString(),
            ExternalSystemId = config.Id,
            State = gitlabIssue.State,
            Author = gitlabIssue.Author?.Name,
            Created = gitlabIssue.CreatedAt,
            Modified = gitlabIssue.UpdatedAt,
            Url = gitlabIssue.WebUrl,
            MilestoneName = gitlabIssue.Milestone?.Title,
            IssueType = gitlabIssue.IssueType,
        };
    }

    public async Task<IReadOnlyList<IssueDto>> SearchAsync(ExternalSystemDto config, string text, int offset, int count, CancellationToken cancellationToken)
    {
        var query = new IssueQuery
        {
            Search = text,
        };

        var issues = new List<IssueDto>();
        if (long.TryParse(config.ExternalProjectId, out var projectId))
        {
            var client = new GitLabClient(config.BaseUrl, config.AccessToken);
            var gitlabIssues = client.Issues.GetAsync(projectId, query);
            if (gitlabIssues is not null)
            {
                await foreach (var gitlabIssue in gitlabIssues)
                {
                    if(offset <= 0)
                    {
                        issues.Add(MapToDto(config, gitlabIssue.Id, gitlabIssue));
                        if(issues.Count >= count)
                        {
                            break;
                        }
                    }
                    offset--;
                }
            }
        }
        return issues;
    }
}
