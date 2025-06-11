using TestBucket.Contracts.Testing.Models;
using TestBucket.Formats.Dtos;
using TestBucket.Traits.Core;

namespace TestBucket.IntegrationTests.Features.Duplication;

[Feature("Duplication")]
[Component("Testing")]
[FunctionalTest]
[EnrichedTest]
[IntegrationTest]
public class DuplicateTestRunTests(TestBucketApp App)
{
    /// <summary>
    /// Verifies that a duplicated run has the same fields as the duplicated one
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DuplicateTestRun_WithTrait_BothHaveTheSameFields()
    {
        // Arrange
        var team = await App.Client.Teams.AddAsync(Guid.NewGuid().ToString());
        var project = await App.Client.Projects.AddAsync(team, Guid.NewGuid().ToString());
        var suite = await App.Client.TestRepository.AddSuiteAsync(team, project, Guid.NewGuid().ToString());

        // Add a run
        var inputRun = new TestRunDto { Team = team, Project = project, Name = "My run " + Guid.NewGuid().ToString() };
        inputRun.Traits.Add(new TestTrait { Name = "Milestone", ExportType = TraitExportType.Instance, Type = TraitType.Milestone, Value = "1.0" });
        var run = await App.Client.TestRuns.AddRunAsync(inputRun);

        // Act
        var duplicatedRun = await App.Client.TestRuns.DuplicateAsync(run.Slug);

        // Assert
        Assert.Equal(duplicatedRun.Milestone, run.Milestone);

        // Cleanup
        await App.Client.Projects.DeleteAsync(project);
        await App.Client.Teams.DeleteAsync(team);
    }

    /// <summary>
    /// Verifies that a duplicated run with tests contains the test
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DuplicateTestRun_WithTest_TestCaseRunIsDuplicated()
    {
        // Arrange
        var team = await App.Client.Teams.AddAsync(Guid.NewGuid().ToString());
        var project = await App.Client.Projects.AddAsync(team, Guid.NewGuid().ToString());
        var suite = await App.Client.TestRepository.AddSuiteAsync(team, project, Guid.NewGuid().ToString());

        // Add a test case
        var testCase = await App.Client.TestRepository.AddTestAsync(CreateTestCase(team, project, suite));

        // Add a run
        var inputRun = new TestRunDto { Team = team, Project = project, Name = "My run " + Guid.NewGuid().ToString() };
        var run = await App.Client.TestRuns.AddRunAsync(inputRun);

        // Add test case to run
        var testCaseRun = new TestCaseRunDto { TestCaseSlug = testCase.Slug, Name = "tc1" };
        await App.Client.TestRuns.AddTestCaseRunAsync(run.Slug, testCaseRun);

        // Act
        var duplicatedRun = await App.Client.TestRuns.DuplicateAsync(run.Slug);

        // Assert
        IReadOnlyList<TestCaseRunDto> testCaseRuns = await App.Client.TestRuns.GetTestCaseRunsAsync(run.Slug);
        Assert.Single(testCaseRuns);
        Assert.Equal(testCaseRun.Name, testCaseRuns[0].Name);

        // Cleanup
        await App.Client.Projects.DeleteAsync(project);
        await App.Client.Teams.DeleteAsync(team);
    }

    private TestCaseDto CreateTestCase(string team, string project, string suite)
    {
        return new TestCaseDto { TestCaseName = "Test To Duplicate " + Guid.NewGuid().ToString(), Description = "Text to duplicate", TenantId = App.Tenant, TeamSlug = team, ProjectSlug = project, TestSuiteSlug = suite };
    }
}
