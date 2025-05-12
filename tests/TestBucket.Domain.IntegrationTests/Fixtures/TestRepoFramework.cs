using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.TestAccounts;
using TestBucket.Domain.TestAccounts.Models;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Testing.TestRuns;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    public class TestRepoFramework(ProjectFixture Fixture)
    {
        internal async Task<TestCase> AddAsync()
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);

            var suite = await AddSuiteAsync();

            var test = new TestCase
            {
                Name = Guid.NewGuid().ToString(),
                TestSuiteId = suite.Id,
                TestProjectId = Fixture.ProjectId,
                TeamId = Fixture.TeamId
            };

            var manager = Fixture.Services.GetRequiredService<ITestCaseManager>();
            await manager.AddTestCaseAsync(user, test);
            return test;
        }
        internal async Task<TestSuite> AddSuiteAsync()
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);

            var suite = new TestSuite
            {
                Name = Guid.NewGuid().ToString(),
                TestProjectId = Fixture.ProjectId,
                TeamId = Fixture.TeamId
            };

            var manager = Fixture.Services.GetRequiredService<ITestSuiteManager>();
            await manager.AddTestSuiteAsync(user, suite);
            return suite;
        }
    }
}
