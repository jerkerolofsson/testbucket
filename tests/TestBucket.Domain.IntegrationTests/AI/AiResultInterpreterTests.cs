using TestBucket.Domain.AI.Runner;

namespace TestBucket.Domain.IntegrationTests.AI
{
    /// <summary>
    /// Test cases that evaluate fictional LLM output from after running a test, to determine if the test passed or failed.
    /// </summary>
    [IntegrationTest]
    [FunctionalTest]
    [EnrichedTest]
    [Component("AI")]
    [Feature("Classification")]
    [Feature("AI Runner")]
    public class AiResultInterpreterTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Tests the interpretation of AI Runner output when the result string is "FAILED".
        /// </summary>
        [Fact]
        public async Task InterpretAiRunnerAsync_WithFailedString_ResultIsFailed()
        {
            var principal = Fixture.App.SiteAdministrator;

            // Act
            var resultInterpreter = Fixture.Services.GetRequiredService<AiResultInterpreter>();
            var result = await resultInterpreter.InterpretAiRunnerAsync(Fixture.App.SiteAdministrator, Fixture.ProjectId, "FAILED");

            // Assert
            Assert.Equal(Contracts.Testing.Models.TestResult.Failed, result);
        }

        /// <summary>
        /// Tests the interpretation of AI Runner output when the result string contains an error message.
        /// </summary>
        [Fact]
        public async Task InterpretAiRunnerAsync_WithErrorString_ResultIsFailed()
        {
            var principal = Fixture.App.SiteAdministrator;

            // Act
            var resultInterpreter = Fixture.Services.GetRequiredService<AiResultInterpreter>();
            var result = await resultInterpreter.InterpretAiRunnerAsync(Fixture.App.SiteAdministrator, Fixture.ProjectId, "There was an error");

            // Assert
            Assert.Equal(Contracts.Testing.Models.TestResult.Failed, result);
        }

        /// <summary>
        /// Tests the interpretation of AI Runner output when the result string is "PASSED".
        /// </summary>
        [Fact]
        public async Task InterpretAiRunnerAsync_WithPassedString_ResultIsPassed()
        {
            var principal = Fixture.App.SiteAdministrator;

            // Act
            var resultInterpreter = Fixture.Services.GetRequiredService<AiResultInterpreter>();
            var result = await resultInterpreter.InterpretAiRunnerAsync(Fixture.App.SiteAdministrator, Fixture.ProjectId, "PASSED");

            // Assert
            Assert.Equal(Contracts.Testing.Models.TestResult.Passed, result);
        }

        /// <summary>
        /// Tests the interpretation of AI Runner output when the result string contains a Playwright-related error message.
        /// </summary>
        /// <param name="errorMessage">The error message to test.</param>
        [Theory]
        [InlineData("Failed to launch browser")]
        [InlineData("Invalid configuration")]
        [InlineData("Unhandled exception")]
        [InlineData("Timeout exceeded")]
        [InlineData("Port already in use")]
        public async Task InterpretAiRunnerAsync_WithPlaywrightError_ResultIsFailed(string errorMessage)
        {
            var principal = Fixture.App.SiteAdministrator;

            // Act
            var resultInterpreter = Fixture.Services.GetRequiredService<AiResultInterpreter>();
            var result = await resultInterpreter.InterpretAiRunnerAsync(Fixture.App.SiteAdministrator, Fixture.ProjectId, errorMessage);

            // Assert
            Assert.Equal(Contracts.Testing.Models.TestResult.Failed, result);
        }
    }
}