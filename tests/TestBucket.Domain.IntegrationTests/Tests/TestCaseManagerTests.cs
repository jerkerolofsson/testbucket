using TestBucket.Domain.Testing.TestRuns.Events;

namespace TestBucket.Domain.IntegrationTests.Tests
{
    /// <summary>
    /// Integration tests for the TestCaseManager, verifying test case behaviors and defaults.
    /// </summary>
    [IntegrationTest]
    [EnrichedTest]
    [Component("Testing")]
    [FunctionalTest]
    public class TestCaseManagerTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that adding an exploratory test case with no session duration sets the default duration to 60 minutes.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task AddExploratoryTestCase_WithNoSessionDuration_DefaultsTo60Minutes()
        {
            // Arrange
            var testCase = new TestCase { Name = "Test Case 1", Description = "Charter of test case.", ScriptType = ScriptType.Exploratory };

            // Act
            await Fixture.Tests.AddAsync(testCase);

            // Assert 
            TestCase? test = await Fixture.Tests.GetTestCaseByIdAsync(testCase.Id);
            Assert.NotNull(test);
            Assert.NotNull(test.SessionDuration);
            Assert.Equal(60, test.SessionDuration.Value);
        }

    }
}
