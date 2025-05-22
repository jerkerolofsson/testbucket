using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Testing.Compiler;

namespace TestBucket.Domain.UnitTests.Compiler
{
    /// <summary>
    /// Contains unit tests for <see cref="TestCompiler.CompileMarkupAsync"/> and related markup/template resolution logic.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    public class CompileMarkupTests
    {
        /// <summary>
        /// Returns a <see cref="TestCase"/> template by name for use in markup compilation tests.
        /// </summary>
        /// <param name="principal">The user principal requesting the template.</param>
        /// <param name="name">The name of the template to retrieve.</param>
        /// <returns>
        /// A <see cref="TestCase"/> instance if the template exists; otherwise, <c>null</c>.
        /// </returns>
        public static Task<TestCase?> GetTemplate(ClaimsPrincipal principal, string name)
        {
            TestCase? test = null;
            if (name == "MyTemplate")
            {
                test = new TestCase
                {
                    Name = "MyTemplate",
                    Description = """
                    Hello
                    @Body
                    """
                };
            }
            if (name == "Hello")
            {
                test = new TestCase
                {
                    Name = "Hello",
                    Description = "Hello"
                };
            }
            if (name == "RecursiveHello")
            {
                test = new TestCase
                {
                    Name = "RecursiveHello",
                    Description = "@include Hello"
                };
            }
            return Task.FromResult<TestCase?>(test);
        }

        /// <summary>
        /// Verifies that <see cref="TestCompiler.CompileMarkupAsync"/> correctly resolves a template using the <c>@template</c> directive.
        /// </summary>
        [Component("Compiler")]
        [Fact]
        public async Task CompileMarkupAsync_WithTemplate_TemplateResolved()
        {
            // Arrange
            var principal = Impersonation.Impersonate("1234");
            var context = new TestExecutionContext { Guid = Guid.NewGuid().ToString(), ProjectId = 0, TeamId = 0, TestRunId = 0 };
            var text = """
                @template MyTemplate
                World
                """;


            var expected = "Hello\nWorld";

            var result = await TestCompiler.CompileMarkupAsync(principal, context, text, GetTemplate);
            Assert.NotEmpty(result);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Verifies that <see cref="TestCompiler.CompileMarkupAsync"/> correctly resolves the <c>@include</c> directive.
        /// </summary>
        [Component("Compiler")]
        [Fact]
        public async Task CompileMarkupAsync_WithInclude_IncludeDirectiveResolved()
        {
            // Arrange
            var principal = Impersonation.Impersonate("1234");
            var context = new TestExecutionContext { Guid = Guid.NewGuid().ToString(), ProjectId = 0, TeamId = 0, TestRunId = 0 };
            var text = """
                @include Hello
                World
                """;

            var expected = "Hello\nWorld";

            var result = await TestCompiler.CompileMarkupAsync(principal, context, text, GetTemplate);
            Assert.NotEmpty(result);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Verifies that <see cref="TestCompiler.CompileMarkupAsync"/> correctly resolves recursive <c>@include</c> directives.
        /// </summary>
        [Component("Compiler")]
        [Fact]
        public async Task CompileMarkupAsync_WithRecursiveInclude_IncludeDirectiveResolved()
        {
            // Arrange
            var principal = Impersonation.Impersonate("1234");
            var context = new TestExecutionContext { Guid = Guid.NewGuid().ToString(), ProjectId = 0, TeamId = 0, TestRunId = 0 };
            var text = """
                @include RecursiveHello
                World
                """;

            // Note: This is perhaps a bug, but each @include lookup adds an extra \n
            var expected = "Hello\n\nWorld";

            var result = await TestCompiler.CompileMarkupAsync(principal, context, text, GetTemplate);
            Assert.NotEmpty(result);
            Assert.Equal(expected, result);
        }
    }
}