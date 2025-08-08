using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.TestAccounts;
using TestBucket.Domain.TestAccounts.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    /// <summary>
    /// Test fixture
    /// </summary>
    /// <param name="Fixture"></param>

    public class TestRunFramework(ProjectFixture Fixture)
    {
        internal async Task AddAsync(TestRun run)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<ITestRunManager>();
            run.Open = true;
            await manager.AddTestRunAsync(principal, run);
        }

        internal async Task<TestRun> AddAsync()
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<ITestRunManager>();

            var run = new TestRun { Name = Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, TeamId = Fixture.TeamId, Open = true };
            await manager.AddTestRunAsync(user, run);
            return run;
        }

        internal async Task<TestCaseRun> AddTestAsync(TestRun run, TestCase testCase)
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            //var testCaseRun = new TestCaseRun
            //{
            //    Name = testCase.Name,
            //    TestRunId = run.Id,
            //    TeamId = run.TeamId,
            //    TestProjectId = run.TestProjectId,
            //    TestCaseId = testCase.Id
            //};

            var manager = Fixture.Services.GetRequiredService<ITestRunManager>();
            return await manager.AddTestCaseRunAsync(user, run, testCase);
        }

        internal async Task<TestRun?> GetByIdAsync(long id)
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<ITestRunManager>();
            return await manager.GetTestRunByIdAsync(user, id);
        }

        internal async Task SetMilestoneAsync(TestRun run, string milestoneName)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var fieldDefinitionManager = Fixture.Services.GetRequiredService<IFieldDefinitionManager>();
            var fieldDefinitions = await fieldDefinitionManager.GetDefinitionsAsync(principal, Fixture.ProjectId);
            var milestoneFieldDefinition = fieldDefinitions.Where(x => x.TraitType == TraitType.Milestone).First();

            var fieldManager = Fixture.Services.GetRequiredService<IFieldManager>();
            var fields = await fieldManager.GetTestRunFieldsAsync(principal, run.Id, fieldDefinitions);
            var milestoneField = fields.Where(x => x.FieldDefinitionId == milestoneFieldDefinition.Id).First();
            milestoneField.StringValue = milestoneName;
            milestoneField.Inherited = false;
            await fieldManager.UpsertTestRunFieldAsync(principal, milestoneField);
        }

        internal async Task SetMilestoneAsync(TestCaseRun run, string milestoneName)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var fieldDefinitionManager = Fixture.Services.GetRequiredService<IFieldDefinitionManager>();
            var fieldDefinitions = await fieldDefinitionManager.GetDefinitionsAsync(principal, Fixture.ProjectId);
            var milestoneFieldDefinition = fieldDefinitions.Where(x => x.TraitType == TraitType.Milestone).First();

            var fieldManager = Fixture.Services.GetRequiredService<IFieldManager>();
            var fields = await fieldManager.GetTestCaseRunFieldsAsync(principal, run.TestRunId, run.Id, fieldDefinitions);
            var milestoneField = fields.Where(x => x.FieldDefinitionId == milestoneFieldDefinition.Id).First();
            milestoneField.Inherited = false;
            milestoneField.StringValue = milestoneName;
            await fieldManager.UpsertTestCaseRunFieldAsync(principal, milestoneField);
        }

        internal async Task<string?> GetMilestoneAsync(TestRun run)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var fieldDefinitionManager = Fixture.Services.GetRequiredService<IFieldDefinitionManager>();
            var fieldDefinitions = await fieldDefinitionManager.GetDefinitionsAsync(principal, Fixture.ProjectId);
            var milestoneFieldDefinition = fieldDefinitions.Where(x => x.TraitType == TraitType.Milestone).First();

            var fieldManager = Fixture.Services.GetRequiredService<IFieldManager>();
            var fields = await fieldManager.GetTestRunFieldsAsync(principal, run.Id, fieldDefinitions);
            var milestoneField = fields.Where(x => x.FieldDefinitionId == milestoneFieldDefinition.Id).First();
            return milestoneField.StringValue;
        }


        internal async Task<string?> GetMilestoneAsync(TestCaseRun testCaseRun)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var fieldDefinitionManager = Fixture.Services.GetRequiredService<IFieldDefinitionManager>();
            var fieldDefinitions = await fieldDefinitionManager.GetDefinitionsAsync(principal, Fixture.ProjectId);
            var milestoneFieldDefinition = fieldDefinitions.Where(x => x.TraitType == TraitType.Milestone).First();

            var fieldManager = Fixture.Services.GetRequiredService<IFieldManager>();
            var fields = await fieldManager.GetTestCaseRunFieldsAsync(principal, testCaseRun.TestRunId, testCaseRun.Id, fieldDefinitions);
            var milestoneField = fields.Where(x => x.FieldDefinitionId == milestoneFieldDefinition.Id).First();
            return milestoneField.StringValue;
        }

        internal async Task<TestCaseRunField> GetMilestoneFieldAsync(TestCaseRun testCaseRun)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var fieldDefinitionManager = Fixture.Services.GetRequiredService<IFieldDefinitionManager>();
            var fieldDefinitions = await fieldDefinitionManager.GetDefinitionsAsync(principal, Fixture.ProjectId);
            var milestoneFieldDefinition = fieldDefinitions.Where(x => x.TraitType == TraitType.Milestone).First();

            var fieldManager = Fixture.Services.GetRequiredService<IFieldManager>();
            var fields = await fieldManager.GetTestCaseRunFieldsAsync(principal, testCaseRun.TestRunId, testCaseRun.Id, fieldDefinitions);
            var milestoneField = fields.Where(x => x.FieldDefinitionId == milestoneFieldDefinition.Id).First();
            return milestoneField;
        }

        internal async Task<TestRun?> GetRunByIdAsync(long id)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            return await GetRunByIdAsync(principal, id);
        }

        internal async Task<TestRun?> GetRunByIdAsync(ClaimsPrincipal principal, long id)
        {
            var manager = Fixture.Services.GetRequiredService<ITestRunManager>();
            return await manager.GetTestRunByIdAsync(principal, id);
        }
    }
}
