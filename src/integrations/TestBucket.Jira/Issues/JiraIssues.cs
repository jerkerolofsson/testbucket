using System.Text;
using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Issues.Models;
using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Issues.Types;
using TestBucket.Jira.Client;
using TestBucket.Jira.Converters;
using TestBucket.Jira.Models;

namespace TestBucket.Jira.Issues;
internal class JiraIssues : IExternalIssueProvider
{
    public string SystemName => ExtensionConstants.SystemName;
    private readonly ILogger<JiraIssues> _logger;

    public JiraIssues(ILogger<JiraIssues> logger)
    {
        _logger = logger;
    }

    private async Task<JiraOauth2Client> CreateJiraClientAsync(ExternalSystemDto system)
    {
        if (string.IsNullOrEmpty(system.BaseUrl))
            throw new ArgumentException("Base URL is required for Jira integration", nameof(system));

        if (string.IsNullOrEmpty(system.AccessToken))
            throw new ArgumentException("Access token is required for Jira integration", nameof(system));

        return await JiraOauth2Client.CreateAsync(system.BaseUrl, system.AccessToken);
    }

    public async Task<IssueDto?> GetIssueAsync(ExternalSystemDto system, string externalIssueId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Getting issue {IssueId} from Jira", externalIssueId);
            var jira = await CreateJiraClientAsync(system);
            var issue = await jira.Issues.GetIssueAsync(externalIssueId);

            if (issue == null)
            {
                _logger.LogWarning("Issue {IssueId} not found in Jira", externalIssueId);
                return null;
            }

            _logger.LogDebug("Successfully retrieved issue {IssueId} from Jira", externalIssueId);
            //return MapJiraIssueToDto(issue, system);
            return null;
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
            var jira = await CreateJiraClientAsync(system);

            IssueStateMapping stateMapping = new DefaultStateMap();

            // Build JQL query for date range
            var jqlBuilder = new StringBuilder();
            jqlBuilder.Append($"project = \"{system.ExternalProjectId}\" AND ");
            jqlBuilder.Append($"issuetype = Bug");

            if (from.HasValue)
            {
                jqlBuilder.Append($" AND updated >= \"{from.Value:yyyy-MM-dd HH:mm}\"");
            }

            jqlBuilder.Append($" AND updated <= \"{until:yyyy-MM-dd HH:mm}\"");

            var jql = jqlBuilder.ToString();
            _logger.LogInformation("Executing JQL query: {JQL}", jql);

            List<IssueDto> issues = [];
            await foreach(var jiraIssue in jira.Issues.GetIssuesFromJqlAsync(jql, 0, 10000, cancellationToken))
            {
                issues.Add(MapJiraIssueToDto(jiraIssue, system, stateMapping));
            }
            return issues;
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
            var jira = await CreateJiraClientAsync(system);
            IssueStateMapping stateMapping = new DefaultStateMap();

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

            List<IssueDto> issues = [];
            await foreach (var jiraIssue in jira.Issues.GetIssuesFromJqlAsync(jql, offset, count, cancellationToken))
            {
                issues.Add(MapJiraIssueToDto(jiraIssue, system, stateMapping));
            }
            return issues;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching issues in Jira with text: {Text}", text);
            return [];
        }
    }
    public async Task CreateIssueAsync(ExternalSystemDto externalSystemDto, IssueDto issueDto, CancellationToken cancellationToken)
    {
        try
        {
            var jira = await CreateJiraClientAsync(externalSystemDto);

            // Create a new issue
            await CreateIssueAsync(jira, externalSystemDto, issueDto, cancellationToken);
        }
        catch (Exception)
        {
            // Log exception in real implementation
            throw;
        }
    }

    private async Task CreateIssueAsync(JiraOauth2Client jira, ExternalSystemDto externalSystemDto, IssueDto issueDto, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(externalSystemDto.ExternalProjectId);

            // Get the project from the integration configuration
            var projects = await jira.Projects.SeaarchProjectsAsync(externalSystemDto.ExternalProjectId, 1, null);
            if(projects is null || projects.total == 0)
            {
                throw new ArgumentException($"No Jira project with name '{externalSystemDto.ExternalProjectId}' was found!");
            }
            Project? project = projects.values?.FirstOrDefault();
            if (project is null)
            {
                throw new ArgumentException($"No Jira project with name '{externalSystemDto.ExternalProjectId}' was found!");
            }

            var issueTypes = await jira.Issues.GetIssueTypesAsync(cancellationToken);
            var issueType = issueTypes.FirstOrDefault(x => x.name?.Equals(issueDto.IssueType, StringComparison.OrdinalIgnoreCase) == true)
                            ?? issueTypes.FirstOrDefault(x => x.name?.Equals("Bug", StringComparison.OrdinalIgnoreCase) == true);

            if (issueType is null)
            {
                throw new Exception($"Issue type '{issueDto.IssueType}' not found in Jira project '{externalSystemDto.ExternalProjectId}'");
            }

            var updateBean = new JiraIssueUpdateBean();
            updateBean.fields.project = project;
            updateBean.fields.issuetype = issueType;
            if (issueDto.Title is not null)
            {
                updateBean.SetSummary(issueDto.Title);
            }
            if (issueDto.Description is not null)
            {
                updateBean.fields.description = ContentConverter.FromMarkdown(issueDto.Description);
            }

            if (issueDto.State is not null)
            {
                var stateName = MapStateToJiraStateName(new IssueState(issueDto.State, issueDto.MappedState), new DefaultStateMap());
            }

            var response = await jira.Issues.CreateIssueAsync(updateBean, cancellationToken);
            issueDto.ExternalDisplayId = response.key;
            issueDto.ExternalId = response.key;
        }
        catch (Exception)
        {
            // Log exception in real implementation
            throw;
        }
    }

    public async Task UpdateIssueAsync(ExternalSystemDto externalSystemDto, IssueDto issueDto, CancellationToken cancellationToken)
    {
        try
        {
            var jira = await CreateJiraClientAsync(externalSystemDto);

            if (!string.IsNullOrEmpty(issueDto.ExternalId))
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

    //private async Task CreateNewIssueAsync(Atlassian.Jira.Jira jira, ExternalSystemDto system, IssueDto issueDto, CancellationToken cancellationToken)
    //{
    //    var issue = jira.CreateIssue(system.ExternalProjectId);
    //    issue.Type = issueDto.IssueType ?? "Task";
    //    issue.Summary = issueDto.Title ?? "";
    //    issue.Description = issueDto.Description;

    //    if (!string.IsNullOrEmpty(issueDto.AssignedTo))
    //    {
    //        issue.Assignee = issueDto.AssignedTo;
    //    }

    //    await issue.SaveChangesAsync();

    //    // Update the DTO with the created issue ID
    //    issueDto.ExternalId = issue.Key?.Value;
    //    issueDto.ExternalDisplayId = issue.Key?.Value;
    //}

    private async Task UpdateExistingIssueAsync(JiraOauth2Client jira, IssueDto issueDto, CancellationToken cancellationToken)
    {
        var issue = await jira.Issues.GetIssueAsync(issueDto.ExternalId!);

        if (issue?.fields != null)
        {
            bool changed = false;

            JiraIssueUpdateBean update = new();

            if (issueDto.Title != issue.fields.summary && issueDto.Title != null)
            {
                update.SetSummary(issueDto.Title);
                changed = true;
            }
          
            if (issueDto.Labels is not null)
            {
                var labels = issue.fields?.labels ?? [];

                foreach (var label in issueDto.Labels)
                {
                    var labelWithoutSpaces = label.Replace(' ', '-');

                    if (!labels.Contains(labelWithoutSpaces))
                    {
                        update.AddLabel(labelWithoutSpaces);
                    }
                }
                foreach (var label in labels)
                {
                    var labelWithoutSpaces = label.Replace(' ', '-');
                    if (!issueDto.Labels.Contains(labelWithoutSpaces))
                    {
                        update.RemoveLabel(labelWithoutSpaces);
                    }
                }
                //update.fields.labels = issueDto.Labels;
                changed = true;
            }

            if (issue.fields is not null)
            {
                var state = MapJiraStatusToMappedState(issue.fields.status?.name, new DefaultStateMap());
                if (issueDto.MappedState != state.MappedState)
                {
                    // todo..
                }
            }

            if (changed && !string.IsNullOrEmpty(issue.key))
            {
                await jira.Issues.UpdateIssueAsync(issue.key, update, cancellationToken);
            }
        }
    }

    internal static IssueDto MapJiraIssueToDto(JiraIssue jiraIssue, ExternalSystemDto system, IssueStateMapping stateMapping)
    {
        var dto = new IssueDto
        {
            ExternalId = jiraIssue.key,
            ExternalDisplayId = jiraIssue.key,
            ExternalSystemName = ExtensionConstants.SystemName,
            ExternalSystemId = system.Id,
            Title = jiraIssue.fields?.summary,
            IssueType = jiraIssue.fields?.issuetype?.name,
            Author = jiraIssue.fields?.reporter?.emailAddress,
            AssignedTo = jiraIssue.fields?.assignee?.emailAddress,
            Created = jiraIssue.fields?.created?.ToUniversalTime(),
            Modified = jiraIssue.fields?.updated?.ToUniversalTime(),
            State = jiraIssue.fields?.status?.name,
            Labels = jiraIssue.fields?.labels,
        };
        if(dto.Labels is not null)
        {
            // Jira issue labels cannot contain spaces
            for(int i=0; i<dto.Labels.Length; i++)
            {
                dto.Labels[i] = dto.Labels[i].Replace('-', ' ');
            }
        }

        dto.Description = ContentConverter.ToMarkdown(jiraIssue.fields?.description);

        //    // Map Jira status to internal MappedIssueState
        if (jiraIssue.fields?.status is not null)
        {
            var state = MapJiraStatusToMappedState(jiraIssue.fields.status.name, stateMapping);
            dto.MappedState = state.MappedState;
            dto.State = state.Name;
        }
        if (jiraIssue.fields?.issuetype?.name is not null)
        {
            if(jiraIssue.fields.issuetype.name == "Bug")
            {
                dto.IssueType = IssueTypes.Issue;
            }
        }

        return dto;
    }


    internal static string? MapStateToJiraStateName(IssueState? state, IssueStateMapping stateMapping)
    {
        if(stateMapping.Count == 0)
        {
            return null;
        }

        if (state == null)
        {
            return stateMapping.First().Key;
        }

        foreach(var pair in stateMapping)
        {
            if(pair.Value.Equals(state))
            {
                return pair.Key;
            }
        }

        return stateMapping.First().Key;
    }

    internal static IssueState MapJiraStatusToMappedState(string? jiraStatus, IssueStateMapping stateMapping)
    {
        if (string.IsNullOrWhiteSpace(jiraStatus))
        {
            return new IssueState(IssueStates.Other, MappedIssueState.Other);
        }

        if(stateMapping.TryGetValue(jiraStatus, out var result))
        {
            return result;
        }

        return new IssueState(jiraStatus, MappedIssueState.Other);
    }
}
