using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts;
using TestBucket.Data.Migrations;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
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
    }
}
