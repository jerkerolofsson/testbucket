using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements;
using TestBucket.Traits.Xunit;
using Xunit;
using TestBucket.Domain.IntegrationTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using TestBucket.Domain.Fields;

namespace TestBucket.Domain.IntegrationTests.Features.FieldInheritance
{
    [IntegrationTest]
    [EnrichedTest]
    [FunctionalTest]
    [Feature("Field Inheritance")]
    public class TestRunFieldInheritanceTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [TestDescription("""
            Verifies that an inherited TestCaseRun field is updated when a TestRun filed is upodated

            # Steps
            1. Create a test run
            2. Set the milestone field to A
            3. Add a test case run
            4. The milestone for the test case run should be A
            5. Change the field on the run to B
            6. The milestone for the test case run should be updated to B
            """)]
        public async Task ChangeTestRunField_WithTestCaseRun_TestCaseRunIsUpdated()
        {
            using var scope = Fixture.Services.CreateScope();
            var testCase = await Fixture.Tests.AddAsync();

            var run = await Fixture.Runs.AddAsync();
            await Fixture.Runs.SetMilestoneAsync(run, "A");

            // Add run
            var testCaseRun = await Fixture.Runs.AddTestAsync(run, testCase);

            // Assert 1
            Assert.Equal("A", await Fixture.Runs.GetMilestoneAsync(testCaseRun));

            // Act
            await Fixture.Runs.SetMilestoneAsync(run, "B");

            // Assert 2
            Assert.Equal("B", await Fixture.Runs.GetMilestoneAsync(testCaseRun));
        }

        [Fact]
        [TestDescription("""
            Verifies that the inherited flag is true when adding a new TestCaseRun when there is an inherited field
            that is targetting both the TestRun and the TestCaseRun

            # Steps
            1. Create a test run
            2. Set the milestone field to A
            3. Add a test case run
            4. The milestone for the test case run should be A and the inherited flag should be equal to true
            """)]
        public async Task AddTestCaseRun_ToTestRunWithInheritedField_InheritedIsTrue()
        {
            using var scope = Fixture.Services.CreateScope();
            var testCase = await Fixture.Tests.AddAsync();

            // Add a run
            var run = await Fixture.Runs.AddAsync();
            await Fixture.Runs.SetMilestoneAsync(run, "A");

            // Add a test to the run (it should get the inherited field)
            var testCaseRun = await Fixture.Runs.AddTestAsync(run, testCase);

            // Assert 1
            var field = await Fixture.Runs.GetMilestoneFieldAsync(testCaseRun);
            Assert.Equal("A", field.StringValue);
            Assert.True(field.Inherited);
        }


        [Fact]
        [TestDescription("""
            Verifies that the field is not updated for a TestCaseRun when the flag is true

            # Steps
            1. Create a test run
            2. Set the milestone field to A
            3. Add a test case run
            4. Set the milestone to B for the test case run
            5. Set the mileston field to C for the test run
            6. Verify that the milestone for the test case run is still B
            """)]
        public async Task UpdateTestRunField_WhenTestCaseRunIsManuallyModified_TestCaseRunFieldIsNotUpdated()
        {
            using var scope = Fixture.Services.CreateScope();
            var testCase = await Fixture.Tests.AddAsync();

            // Add run
            var run = await Fixture.Runs.AddAsync();
            await Fixture.Runs.SetMilestoneAsync(run, "A");

            // Add a test
            var testCaseRun = await Fixture.Runs.AddTestAsync(run, testCase);

            // Manually change the field on the TestCaseRun
            await Fixture.Runs.SetMilestoneAsync(testCaseRun, "B");

            // Update the field on the run
            await Fixture.Runs.SetMilestoneAsync(run, "C");

            // Get the field and verify that it is still B
            var field = await Fixture.Runs.GetMilestoneFieldAsync(testCaseRun);
            Assert.Equal("B", field.StringValue);
            Assert.False(field.Inherited);
        }
    }
}