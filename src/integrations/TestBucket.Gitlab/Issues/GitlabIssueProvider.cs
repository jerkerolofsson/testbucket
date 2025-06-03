using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using NGitLab;
using NGitLab.Models;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Issues.Models;
using TestBucket.Contracts.Issues.States;

namespace TestBucket.Gitlab.Issues;
public class GitlabIssueProvider : IExternalIssueProvider
{
    public string SystemName => ExtensionConstants.SystemName;

    private readonly ILogger<GitlabIssueProvider> _logger;

    public GitlabIssueProvider(ILogger<GitlabIssueProvider> logger)
    {
        _logger = logger;
    }

    public async Task<IssueDto?> GetIssueAsync(ExternalSystemDto config, string externalIssueId, CancellationToken cancellationToken)
    {
        if (long.TryParse(externalIssueId, out var issueId))
        {
            if (long.TryParse(config.ExternalProjectId, out var projectId))
            {
                var client = new GitLabClient(config.BaseUrl, config.AccessToken);
                var gitlabIssue = await client.Issues.GetAsync(projectId, issueId, cancellationToken);
                if (gitlabIssue is not null)
                {
                    return MapToDto(config, issueId, gitlabIssue);
                }
            }
        }
        return null;
    }

    public async Task<IReadOnlyList<IssueDto>> SearchAsync(ExternalSystemDto config, string? text, int offset, int count, CancellationToken cancellationToken)
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
                    if (offset <= 0)
                    {
                        issues.Add(MapToDto(config, gitlabIssue.Id, gitlabIssue));
                        if (issues.Count >= count)
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

    public static MappedIssueState MapState(string gitlabState)
    {
        return gitlabState.ToLower() switch
        {
            "open" => MappedIssueState.Open,
            "opened" => MappedIssueState.Open,
            "close" => MappedIssueState.Closed,
            "closed" => MappedIssueState.Closed,
            _ => MappedIssueState.Other
        };
    }

    public static string MapStateEvent(MappedIssueState mappedState)
    {
        return mappedState switch
        {
            MappedIssueState.Open => "open",
            MappedIssueState.Closed => "close",
            _ => "open"
        };
    }
    private static IssueDto MapToDto(ExternalSystemDto config, long id, Issue gitlabIssue)
    {
        var dto = new IssueDto()
        {
            Title = gitlabIssue.Title,
            Description = gitlabIssue.Description,
            ExternalSystemName = ExtensionConstants.SystemName,
            //ExternalId = id.ToString(),
            ExternalDisplayId = $"#{gitlabIssue.IssueId}",
            ExternalId = gitlabIssue.IssueId.ToString(),
            ExternalSystemId = config.Id,
            State = gitlabIssue.State,
            MappedState = MapState(gitlabIssue.State.ToLower()),
            Author = gitlabIssue.Author?.Name,
            Created = gitlabIssue.CreatedAt,
            Modified = gitlabIssue.UpdatedAt,
            Url = gitlabIssue.WebUrl,
            MilestoneName = gitlabIssue.Milestone?.Title,
            IssueType = gitlabIssue.IssueType,
            Labels = gitlabIssue.Labels,
        };
        switch(dto.MappedState)
        {
            case MappedIssueState.Open:
                dto.State = IssueStates.Open;
                break;
            case MappedIssueState.Closed:
                dto.State = IssueStates.Closed;
                break;
        }

        return dto;
    }

    public async Task<IReadOnlyList<IssueDto>> GetIssuesAsync(ExternalSystemDto config, DateTimeOffset? from, DateTimeOffset until, CancellationToken cancellationToken)
    {
        var query = new IssueQuery
        {
            UpdatedAfter = from?.UtcDateTime,
            UpdatedBefore = until.UtcDateTime
        };
        if(query.UpdatedAfter is not null)
        {
            query.UpdatedAfter = query.UpdatedAfter.Value.AddHours(-1);
        }

        var issues = new List<IssueDto>();
        if (long.TryParse(config.ExternalProjectId, out var projectId))
        {
            var client = new GitLabClient(config.BaseUrl, config.AccessToken);
            var gitlabIssues = client.Issues.GetAsync(projectId, query);
            if (gitlabIssues is not null)
            {
                await foreach (var gitlabIssue in gitlabIssues)
                {
                    issues.Add(MapToDto(config, gitlabIssue.Id, gitlabIssue));
                }
            }
        }
        return issues;
    }

    public async Task UpdateIssueAsync(ExternalSystemDto config, IssueDto issue, CancellationToken cancellationToken)
    {
        if (long.TryParse(issue.ExternalId, out var issueId))
        {
            if (long.TryParse(config.ExternalProjectId, out var projectId))
            {
                var client = new GitLabClient(config.BaseUrl, config.AccessToken);
                var gitlabIssue = await client.Issues.GetAsync(projectId, issueId, cancellationToken);
                if (gitlabIssue is not null)
                {
                    var issueEdit = new IssueEdit();
                    bool changed = false;
                    if (gitlabIssue.Description != issue.Description)
                    {
                        changed = true;
                        issueEdit.Description = issue.Description;
                    }
                    if (gitlabIssue.Title != issue.Title)
                    {
                        changed = true;
                        issueEdit.Title = issue.Title;
                    }

                    // Labels
                    // issueEdit.Labels is a comma separated

                    // State
                    var gitlabState = MapState(gitlabIssue.State.ToLower());
                    if (gitlabState != issue.MappedState && issue.MappedState is (MappedIssueState.Open or MappedIssueState.Closed))
                    {
                        changed = true;
                        issueEdit.State = MapStateEvent(issue.MappedState);
                    }
                    if (changed)
                    {
                        try
                        {
                            issueEdit.ProjectId = projectId;
                            issueEdit.IssueId = issueId;
                            await client.Issues.EditAsync(issueEdit, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to save issue edit");
                        }
                    }
                }
            }
        }
    }
}
