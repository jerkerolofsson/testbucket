using TestBucket.Domain.Testing.TestRuns.Events;

namespace TestBucket.Domain.IntegrationTests.Tests
{
    [IntegrationTest]
    [EnrichedTest]
    [Component("Test Repository")]
    [FunctionalTest]
    public class TestCaseRepositoryTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [FunctionalTest]
        public async Task AddTestCase_SlugCreated()
        {
            // Arrange
            var testCase = new TestCase { Name = "Test Case 1", Description = "Charter of test case.", ScriptType = ScriptType.Exploratory };

            // Act
            await Fixture.Tests.AddAsync(testCase);

            // Assert 
            TestCase? test = await Fixture.Tests.GetTestCaseByIdAsync(testCase.Id);
            Assert.NotNull(test);
            Assert.NotNull(test.Slug);
        }
        [Fact]
        [FunctionalTest]
        public async Task AddTestCase_ExternalDisplayIdSet()
        {
            // Arrange
            var testCase = new TestCase { Name = "Test Case 1", Description = "Charter of test case.", ScriptType = ScriptType.Exploratory };

            // Act
            await Fixture.Tests.AddAsync(testCase);

            // Assert 
            TestCase? test = await Fixture.Tests.GetTestCaseByIdAsync(testCase.Id);
            Assert.NotNull(test);
            Assert.NotNull(test.ExternalDisplayId);
        }
    }
}
