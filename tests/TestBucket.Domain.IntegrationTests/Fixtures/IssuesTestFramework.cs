using Mediator;

using TestBucket.Contracts;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.Requests;
using TestBucket.Domain.Issues.Search;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    /// <summary>
    /// Test fixture
    /// </summary>
    /// <param name="Fixture"></param>
    public class IssuesTestFramework(ProjectFixture Fixture)
    {
        internal async Task<LocalIssue> AddIssueAsync()
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<IIssueManager>();

            var issue = new LocalIssue
            {
                Title = Guid.NewGuid().ToString(),
                TestProjectId = Fixture.ProjectId,
                TeamId = Fixture.TeamId,
            };
            await manager.AddLocalIssueAsync(principal, issue);
            return issue;
        }

        internal async Task<LocalIssue?> GetIssueByIdAsync(long id)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<IIssueManager>();
            return await manager.GetIssueByIdAsync(principal, id);
        }

        internal async Task UpdateIssueAsync(LocalIssue issue)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<IIssueManager>();
            await manager.UpdateLocalIssueAsync(principal, issue);
        }
        internal async Task<PagedResult<LocalIssue>> SearchAsync(string text, int offset, int count)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<IIssueManager>();
            return await manager.SearchLocalIssuesAsync(principal, Fixture.ProjectId, text, offset, count);
        }
        internal async Task<PagedResult<LocalIssue>> SearchAsync(SearchIssueQuery query, int offset, int count)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<IIssueManager>();
            return await manager.SearchLocalIssuesAsync(principal, query, offset, count);
        }

        internal async Task<LocalIssue?> FindLocalIssueFromExternalAsync(long testProjectId, long? externalSystemId, string? externalId)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<IIssueManager>();
            return await manager.FindLocalIssueFromExternalAsync(principal, testProjectId, externalSystemId, externalId);
        }

        internal async Task CloseIssueAsync(LocalIssue issue)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var mediator = Fixture.Services.GetRequiredService<IMediator>();
            await mediator.Send(new CloseIssueRequest(principal, issue));
        }
    }
}
