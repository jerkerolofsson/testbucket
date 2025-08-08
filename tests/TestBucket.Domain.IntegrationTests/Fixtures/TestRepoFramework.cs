using System.Security.Claims;
using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Testing.TestCases.Search;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    /// <summary>
    /// Test fixture
    /// </summary>
    /// <param name="Fixture"></param>

    public class TestRepoFramework(ProjectFixture Fixture)
    {
        internal TestBucketApp App => Fixture.App;

        internal async Task<HashSet<long>> SearchTestIdsAsync(string text)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);

            var fieldDefinitions = await GetTestCaseFieldDefinitionsAsync(principal);

            SearchTestQuery query = SearchTestCaseQueryParser.Parse(text, fieldDefinitions, App.TimeProvider);

            HashSet<long> ids = [];
            var manager = Fixture.Services.GetRequiredService<ITestCaseManager>();
            await foreach (var testId in manager.SearchTestCaseIdsAsync(principal, query))
            {
                ids.Add(testId);
            }
            return ids;
        }

        private async Task<IReadOnlyList<Domain.Fields.Models.FieldDefinition>> GetTestCaseFieldDefinitionsAsync(System.Security.Claims.ClaimsPrincipal principal)
        {
            var fieldDefinitionManager = Fixture.Services.GetRequiredService<IFieldDefinitionManager>();
            var fieldDefinitions = await fieldDefinitionManager.GetDefinitionsAsync(principal, Fixture.ProjectId, FieldTarget.TestCase);
            return fieldDefinitions;
        }

        /// <summary>
        /// Sets milestone on a test case
        /// </summary>
        /// <param name="test"></param>
        /// <param name="milestoneName"></param>
        /// <returns></returns>
        internal async Task SetMilestoneAsync(TestCase test, string milestoneName)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var fieldDefinitions = await GetTestCaseFieldDefinitionsAsync(principal);
            var milestoneFieldDefinition = fieldDefinitions.Where(x => x.TraitType == TraitType.Milestone).First();

            var fieldManager = Fixture.Services.GetRequiredService<IFieldManager>();
            var fields = await fieldManager.GetTestCaseFieldsAsync(principal, test.Id, fieldDefinitions);
            var milestoneField = fields.Where(x => x.FieldDefinitionId == milestoneFieldDefinition.Id).First();
            milestoneField.StringValue = milestoneName;
            milestoneField.Inherited = false;
            await fieldManager.UpsertTestCaseFieldAsync(principal, milestoneField);
        }


        /// <summary>
        /// Adds a test case
        /// </summary>
        /// <returns></returns>
        internal async Task<TestCase> AddAsync(TestCase testCase, TestSuite? suite = null)
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);

            suite ??= await AddSuiteAsync();

            testCase.TestSuiteId = suite.Id;
            testCase.TestProjectId ??= Fixture.ProjectId;
            testCase.TeamId ??= Fixture.TeamId;

            var manager = Fixture.Services.GetRequiredService<ITestCaseManager>();
            await manager.AddTestCaseAsync(user, testCase);
            return testCase;
        }

        /// <summary>
        /// Adds a test case
        /// </summary>
        /// <returns></returns>
        internal async Task<TestCase> AddAsync(TestSuite? suite = null)
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);

            suite ??= await AddSuiteAsync();

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
            return await AddSuiteAsync(user);
        }
        internal async Task<TestSuite> AddSuiteAsync(ClaimsPrincipal user)
        {
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

        internal async Task UpdateSuiteAsync(TestSuite suite)
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            await UpdateSuiteAsync(user, suite);
        }

        internal async Task UpdateSuiteAsync(ClaimsPrincipal user, TestSuite suite)
        {
            var manager = Fixture.Services.GetRequiredService<ITestSuiteManager>();
            await manager.UpdateTestSuiteAsync(user, suite);
        }

        internal async Task DeleteSuiteAsync(ClaimsPrincipal user, TestSuite suite)
        {
            var manager = Fixture.Services.GetRequiredService<ITestSuiteManager>();
            await manager.DeleteTestSuiteByIdAsync(user, suite.Id);
        }

        internal async Task<TestCase?> GetTestCaseByIdAsync(long id)
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<ITestCaseManager>();
            return await manager.GetTestCaseByIdAsync(user, id);
        }
    }
}
