using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Issues.Models;

namespace TestBucket.Jira.Issues;
internal class JiraIssues : IExternalIssueProvider
{
    public string SystemName => ExtensionConstants.SystemName;

    public async Task<IssueDto?> GetIssueAsync(ExternalSystemDto system, string externalIssueId, CancellationToken cancellationToken)
    {
        await Task.Delay(0);
        return null;
    }

    public async Task<IReadOnlyList<IssueDto>> GetIssuesAsync(ExternalSystemDto system, DateTimeOffset? from, DateTimeOffset until, CancellationToken cancellationToken)
    {
        await Task.Delay(0);
        return [];
    }

    public async Task<IReadOnlyList<IssueDto>> SearchAsync(ExternalSystemDto system, string? text, int offset, int count, CancellationToken cancellationToken)
    {
        await Task.Delay(0);
        return [];
    }

    public async Task UpdateIssueAsync(ExternalSystemDto externalSystemDto, IssueDto issueDto, CancellationToken cancellationToken)
    {
        await Task.Delay(0);
    }
}
