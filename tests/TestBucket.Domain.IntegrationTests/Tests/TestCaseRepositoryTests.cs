using TestBucket.Domain.Testing.TestRuns.Events;

namespace TestBucket.Domain.IntegrationTests.Tests
{
    /// <summary>
    /// Tests relaeted to the test repo
    /// </summary>
    /// <param name="Fixture"></param>
    [IntegrationTest]
    [EnrichedTest]
    [Component("Test Repository")]
    [FunctionalTest]
    public class TestCaseRepositoryTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that adding a test case creates a slug for the test case.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
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
        /// <summary>
        /// Verifies that adding a test case sets the ExternalDisplayId property.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
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
