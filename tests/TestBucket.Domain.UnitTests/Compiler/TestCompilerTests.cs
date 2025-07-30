using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TestBucket.Domain.Testing.Compiler;
using TestBucket.Contracts.Testing.Models;

namespace TestBucket.Domain.UnitTests.Compiler
{
    /// <summary>
    /// Contains unit tests for the <see cref="TestCompiler"/> class.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    [Feature("Testing")]
    [Component("Compiler")]
    public class TestCompilerTests
    {
        private readonly TestCompiler _compiler;

        /// <summary>
        /// Initialized compiler
        /// </summary>
        public TestCompilerTests()
        {
            // Mock dependencies can be initialized here if needed.
            _compiler = new TestCompiler(null!, null!, null!, null!);
        }

        /// <summary>
        /// Verifies that <see cref="TestCompiler.FindVariables(string)"/> returns an empty list when the input string is empty.
        /// </summary>
        [Fact]
        public void FindVariables_WithEmptyString_ReturnsEmptyList()
        {
            // Arrange
            var input = string.Empty;

            // Act
            var result = TestCompiler.FindVariables(input);

            // Assert
            Assert.Empty(result);
        }

        /// <summary>
        /// Verifies that <see cref="TestCompiler.FindVariables(string)"/> detects variables correctly.
        /// </summary>
        [Fact]
        public void FindVariables_WithVariables_ReturnsCorrectVariables()
        {
            // Arrange
            var input = "Hello {{world}} and {{user}}!";

            // Act
            var result = TestCompiler.FindVariables(input);

            // Assert
            Assert.Contains("world", result);
            Assert.Contains("user", result);
            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies that <see cref="TestCompiler.ReplaceVariables(Dictionary{string, string}, string, TestExecutionContext)"/> replaces variables correctly.
        /// </summary>
        [Fact]
        public void ReplaceVariables_WithValidVariables_ReplacesCorrectly()
        {
            // Arrange
            var variables = new Dictionary<string, string>
            {
                { "world", "Earth" },
                { "user", "John" }
            };
            var input = "Hello {{world}} and {{user}}!";
            var context = new TestExecutionContext
            {
                Guid = Guid.NewGuid().ToString(),
                TestRunId = 1,
                ProjectId = 1,
                TeamId = 1
            };

            // Act
            var result = TestCompiler.ReplaceVariables(variables, input, context);

            // Assert
            Assert.Equal("Hello Earth and John!", result);
        }

        /// <summary>
        /// Verifies that <see cref="TestCompiler.CompileMarkupAsync"/> compiles markup correctly.
        /// </summary>
        [Fact]
        public async Task CompileMarkupAsync_WithTemplate_CompilesCorrectly()
        {
            // Arrange
            var principal = new ClaimsPrincipal();
            var context = new TestExecutionContext() { Guid = "123", ProjectId = 1, TeamId = 1, TestRunId = 1 };
            var source = "@template MyTemplate\nBody content";

            Task<TestCase?> MockFindTestCase(ClaimsPrincipal _, string name)
            {
                TestCase? result = null;
                if (name == "MyTemplate")
                {
                    result = new TestCase { Name = "MyTemplate", Description = "Template: @Body" };
                }
                return Task.FromResult(result);
            }

            // Act
            var result = await TestCompiler.CompileMarkupAsync(principal, context, source, MockFindTestCase);

            // Assert
            Assert.Equal("Template: Body content", result);
        }
    }
}
