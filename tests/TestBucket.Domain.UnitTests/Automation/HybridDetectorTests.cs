using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using NSubstitute;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Testing.Markdown;
using TestBucket.Domain.Testing.Models;

using Xunit;

namespace TestBucket.Domain.UnitTests.Automation
{
    /// <summary>
    /// Unit tests for the <see cref="HybridDetector"/> class.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    [Component("Automation")]
    [Feature("Hybrid Tests")]
    public class HybridDetectorTests
    {
        /// <summary>
        /// Verifies that <see cref="HybridDetector.ProcessAsync"/> sets the execution type to Hybrid
        /// when the Markdown description contains a supported language.
        /// </summary>
        [Fact]
        public async Task ProcessAsync_ShouldSetExecutionTypeToHybrid_WhenMarkdownContainsSupportedLanguage()
        {
            // Arrange
            var mockRunner = Substitute.For<IMarkdownTestRunner>();
            mockRunner.GetSupportedLanguagesAsync(Arg.Any<ClaimsPrincipal>())
                      .Returns(Task.FromResult(new[] { "csharp", "python" }));

            var hybridDetector = new HybridDetector(new[] { mockRunner });

            var testCase = new TestCase
            {
                Name = "test1",
                Description = "```csharp\nConsole.WriteLine(\"Hello, World!\");\n```",
                ExecutionType = TestExecutionType.Manual
            };

            var principal = new ClaimsPrincipal();

            // Act
            await hybridDetector.ProcessAsync(principal, testCase);

            // Assert
            Assert.Equal(TestExecutionType.Hybrid, testCase.ExecutionType);
        }

        /// <summary>
        /// Verifies that <see cref="HybridDetector.ProcessAsync"/> sets the execution type to Manual
        /// when the Markdown description does not contain a supported language.
        /// </summary>
        [Fact]
        public async Task ProcessAsync_ShouldSetExecutionTypeToManual_WhenMarkdownDoesNotContainSupportedLanguage()
        {
            // Arrange
            var mockRunner = Substitute.For<IMarkdownTestRunner>();
            mockRunner.GetSupportedLanguagesAsync(Arg.Any<ClaimsPrincipal>())
                      .Returns(Task.FromResult(new[] { "java", "ruby" }));

            var hybridDetector = new HybridDetector(new[] { mockRunner });

            var testCase = new TestCase
            {
                Name = "test1",
                Description = "```csharp\nConsole.WriteLine(\"Hello, World!\");\n```",
                ExecutionType = TestExecutionType.Manual
            };

            var principal = new ClaimsPrincipal();

            // Act
            await hybridDetector.ProcessAsync(principal, testCase);

            // Assert
            Assert.Equal(TestExecutionType.Manual, testCase.ExecutionType);
        }

        /// <summary>
        /// Verifies that <see cref="HybridDetector.ProcessAsync"/> does not change the execution type
        /// when the Markdown description is null.
        /// </summary>
        [Fact]
        public async Task ProcessAsync_ShouldNotChangeExecutionType_WhenDescriptionIsNull()
        {
            // Arrange
            var mockRunner = Substitute.For<IMarkdownTestRunner>();
            var hybridDetector = new HybridDetector(new[] { mockRunner });

            var testCase = new TestCase
            {
                Name = "test1",
                Description = null,
                ExecutionType = TestExecutionType.Manual
            };

            var principal = new ClaimsPrincipal();

            // Act
            await hybridDetector.ProcessAsync(principal, testCase);

            // Assert
            Assert.Equal(TestExecutionType.Manual, testCase.ExecutionType);
        }
    }
}