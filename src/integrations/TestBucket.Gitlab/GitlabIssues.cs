
using Microsoft.Extensions.Caching.Memory;

using NGitLab;
using NGitLab.Models;

using TestBucket.Contracts.Integrations;
using TestBucket.Traits.Core;

namespace TestBucket.Gitlab;

public class GitlabIssues
{
    /// <summary>
    /// Gets a issue from github
    /// </summary>
    /// <param name="systems"></param>
    /// <param name="externalId"></param>
    /// <returns></returns>
    public async Task GetIssueAsync(ExternalSystemDto[] systems, string externalId, CancellationToken cancellationToken)
    {
        var system = systems.Where(x => x.Name == "GitLab").FirstOrDefault();
        if (system is not null &&
            !string.IsNullOrEmpty(system.ExternalProjectId) &&
            long.TryParse(system.ExternalProjectId, out long projectId) &&
            long.TryParse(externalId, out long id))
            
        {
            var client = new GitLabClient(system.BaseUrl, system.AccessToken);
            var githubIssue = await client.Issues.GetByIdAsync(id, cancellationToken);
            if(githubIssue is not null)
            {
            }
        }
    }
}
