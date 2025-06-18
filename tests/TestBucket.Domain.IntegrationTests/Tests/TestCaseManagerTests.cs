using TestBucket.Domain.Testing.TestRuns.Events;

namespace TestBucket.Domain.IntegrationTests.Tests
{
    [IntegrationTest]
    [EnrichedTest]
    [Component("Testing")]
    [FunctionalTest]
    public class TestCaseManagerTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
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
