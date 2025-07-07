using System.Text;
using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Issues.Models;
using TestBucket.Contracts.Issues.States;

namespace TestBucket.Jira.Issues;
internal class JiraIssues : IExternalIssueProvider
{
    public string SystemName => ExtensionConstants.SystemName;
    private readonly ILogger<JiraIssues> _logger;

    public JiraIssues(ILogger<JiraIssues> logger)
    {
        _logger = logger;
    }

    private Atlassian.Jira.Jira CreateJiraClient(ExternalSystemDto system)
    {
        if (string.IsNullOrEmpty(system.BaseUrl))
            throw new ArgumentException("Base URL is required for Jira integration", nameof(system));

        if (string.IsNullOrEmpty(system.AccessToken))
            throw new ArgumentException("Access token is required for Jira integration", nameof(system));

        _logger.LogDebug("Creating Jira client for base URL: {BaseUrl}", system.BaseUrl);

        var settings = new Atlassian.Jira.JiraRestClientSettings
        {
            EnableUserPrivacyMode = true
        };

        // Use OAuth2 token authentication
        return Atlassian.Jira.Jira.CreateRestClient(
            system.BaseUrl,
            string.Empty, // username not needed for token auth
            "abc"+system.AccessToken, // OAuth2 access token
            settings);
    }

    public async Task<IssueDto?> GetIssueAsync(ExternalSystemDto system, string externalIssueId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Getting issue {IssueId} from Jira", externalIssueId);
            var jira = CreateJiraClient(system);
            var issue = await jira.Issues.GetIssueAsync(externalIssueId);

            if (issue == null)
            {
                _logger.LogWarning("Issue {IssueId} not found in Jira", externalIssueId);
                return null;
            }

            _logger.LogDebug("Successfully retrieved issue {IssueId} from Jira", externalIssueId);
            return MapJiraIssueToDto(issue, system);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving issue {IssueId} from Jira", externalIssueId);
            return null;
        }
    }

    public async Task<IReadOnlyList<IssueDto>> GetIssuesAsync(ExternalSystemDto system, DateTimeOffset? from, DateTimeOffset until, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Getting issues from Jira for date range: {From} to {Until}", from, until);
            var jira = CreateJiraClient(system);

            var totalCount = jira.Issues.Queryable.Where(x=>x.Type == "Bug").Count();

            // Build JQL query for date range
            var jqlBuilder = new StringBuilder();
            //jqlBuilder.Append($"project = \"{system.ExternalProjectId}\" AND");
            jqlBuilder.Append($"issuetype = Bug");

            if (from.HasValue)
            {
                jqlBuilder.Append($" AND updated >= \"{from.Value:yyyy-MM-dd}\"");
            }

            jqlBuilder.Append($" AND updated <= \"{until:yyyy-MM-dd}\"");

            var jql = jqlBuilder.ToString();
            _logger.LogInformation("Executing JQL query: {JQL}", jql);

            var issues = await jira.Issues.GetIssuesFromJqlAsync(jql);
            _logger.LogInformation("Retrieved {Count} issues from Jira using JQL query", issues.Count());

            return issues.Select(issue => MapJiraIssueToDto(issue, system)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving issues from Jira for date range: {From} to {Until}", from, until);
            return [];
        }
    }

    public async Task<IReadOnlyList<IssueDto>> SearchAsync(ExternalSystemDto system, string? text, int offset, int count, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Searching issues in Jira with text: {Text}, offset: {Offset}, count: {Count}", text, offset, count);
            var jira = CreateJiraClient(system);

            // Build JQL query for text search
            var jqlBuilder = new List<string>();
            jqlBuilder.Add($"project = \"{system.ExternalProjectId}\"");
            jqlBuilder.Add($"AND issuetype = Bug");

            if (!string.IsNullOrWhiteSpace(text))
            {
                jqlBuilder.Add($"text ~ \"{text}\"");
            }

            var jql = string.Join(" AND ", jqlBuilder);
            _logger.LogDebug("Executing search JQL query: {JQL}", jql);
          
            var issues = await jira.Issues.GetIssuesFromJqlAsync(
                jql,
                maxIssues: count,
                startAt: offset);

            _logger.LogInformation("Retrieved {Count} issues from Jira search", issues.Count());
            return issues.Select(issue => MapJiraIssueToDto(issue, system)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching issues in Jira with text: {Text}", text);
            return [];
        }
    }

    public async Task UpdateIssueAsync(ExternalSystemDto externalSystemDto, IssueDto issueDto, CancellationToken cancellationToken)
    {
        try
        {
            var jira = CreateJiraClient(externalSystemDto);

            if (string.IsNullOrEmpty(issueDto.ExternalId))
            {
                // Create new issue
                await CreateNewIssueAsync(jira, externalSystemDto, issueDto, cancellationToken);
            }
            else
            {
                // Update existing issue
                await UpdateExistingIssueAsync(jira, issueDto, cancellationToken);
            }
        }
        catch (Exception)
        {
            // Log exception in real implementation
            throw;
        }
    }

    private async Task CreateNewIssueAsync(Atlassian.Jira.Jira jira, ExternalSystemDto system, IssueDto issueDto, CancellationToken cancellationToken)
    {
        var issue = jira.CreateIssue(system.ExternalProjectId);
        issue.Type = issueDto.IssueType ?? "Task";
        issue.Summary = issueDto.Title ?? "";
        issue.Description = issueDto.Description;

        if (!string.IsNullOrEmpty(issueDto.AssignedTo))
        {
            issue.Assignee = issueDto.AssignedTo;
        }

        await issue.SaveChangesAsync();

        // Update the DTO with the created issue ID
        issueDto.ExternalId = issue.Key?.Value;
        issueDto.ExternalDisplayId = issue.Key?.Value;
    }

    private async Task UpdateExistingIssueAsync(Atlassian.Jira.Jira jira, IssueDto issueDto, CancellationToken cancellationToken)
    {
        var issue = await jira.Issues.GetIssueAsync(issueDto.ExternalId!);

        if (issue != null)
        {
            if (issueDto.Title != null)
                issue.Summary = issueDto.Title;

            if (issueDto.Description != null)
                issue.Description = issueDto.Description;

            if (issueDto.AssignedTo != null)
                issue.Assignee = issueDto.AssignedTo;

            await issue.SaveChangesAsync();
        }
    }

    private static IssueDto MapJiraIssueToDto(Atlassian.Jira.Issue jiraIssue, ExternalSystemDto system)
    {
        var dto = new IssueDto
        {
            ExternalId = jiraIssue.Key?.Value,
            ExternalDisplayId = jiraIssue.Key?.Value,
            ExternalSystemName = ExtensionConstants.SystemName,
            ExternalSystemId = system.Id,
            Title = jiraIssue.Summary,
            Description = jiraIssue.Description,
            IssueType = jiraIssue.Type?.Name,
            Author = jiraIssue.Reporter,
            AssignedTo = jiraIssue.Assignee,
            Created = jiraIssue.Created,
            Modified = jiraIssue.Updated,
            State = jiraIssue.Status?.Name,
            Labels = jiraIssue.Labels?.ToArray()
        };

        // Map Jira status to internal MappedIssueState
        dto.MappedState = MapJiraStatusToMappedState(jiraIssue.Status?.Name);

        // Build URL if we have base URL and issue key
        if (!string.IsNullOrEmpty(system.BaseUrl) && !string.IsNullOrEmpty(jiraIssue.Key?.Value))
        {
            dto.Url = $"{system.BaseUrl.TrimEnd('/')}/browse/{jiraIssue.Key.Value}";
        }

        return dto;
    }

    private static MappedIssueState MapJiraStatusToMappedState(string? jiraStatus)
    {
        if (string.IsNullOrEmpty(jiraStatus))
            return MappedIssueState.Other;

        return jiraStatus.ToLowerInvariant() switch
        {
            "open" or "new" or "to do" or "todo" => MappedIssueState.Open,
            "in progress" or "in-progress" or "progress" => MappedIssueState.InProgress,
            "done" or "completed" or "finished" or "resolved" => MappedIssueState.Completed,
            "closed" => MappedIssueState.Closed,
            "cancelled" or "canceled" => MappedIssueState.Canceled,
            "review" or "in review" or "code review" => MappedIssueState.Reviewed,
            "accepted" => MappedIssueState.Accepted,
            "assigned" => MappedIssueState.Assigned,
            _ => MappedIssueState.Other
        };
    }
}
