using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Testing.Compiler;

namespace TestBucket.Domain.UnitTests.Compiler
{
    /// <summary>
    /// Contains unit tests for variable detection and replacement in <see cref="TestCompiler"/>.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    [Feature("Testing")]
    public class VariableTests
    {
        /// <summary>
        /// Verifies that <see cref="TestCompiler.FindVariables(string)"/> returns an empty list when the input string is empty.
        /// </summary>
        [Component("Compiler")]
        [Fact]
        public void FindVariables_WithEmptyString_EmptyListReturned()
        {
            var text = "";
            var result = TestCompiler.FindVariables(text);
            Assert.Empty(result);
        }

        /// <summary>
        /// Verifies that <see cref="TestCompiler.FindVariables(string)"/> returns an empty list when there are no variables in the input.
        /// </summary>
        [Component("Compiler")]
        [Fact]
        public void FindVariables_WithNoVariables_EmptyListReturned()
        {
            var text = "Hello World";
            var result = TestCompiler.FindVariables(text);
            Assert.Empty(result);
        }

        /// <summary>
        /// Verifies that a single variable on the first line is detected by <see cref="TestCompiler.FindVariables(string)"/>.
        /// </summary>
        [Component("Compiler")]
        [Fact]
        public void FindVariables_WithVariableOneFirstLine_SingleVariableReturned()
        {
            var text = "Hello {{world}}";
            var result = TestCompiler.FindVariables(text);
            Assert.Single(result);
            Assert.Equal("world", result.First());
        }

        /// <summary>
        /// Verifies that multiple occurrences of the same variable are only returned once by <see cref="TestCompiler.FindVariables(string)"/>.
        /// </summary>
        [Component("Compiler")]
        [Fact]
        public void FindVariables_WithTheSameVariableMultipleTimes_SingleVariableReturned()
        {
            var text = "Hello {{world}} {{world}}";
            var result = TestCompiler.FindVariables(text);
            Assert.Single(result);
            Assert.Equal("world", result.First());
        }

        /// <summary>
        /// Verifies that the same variable on different lines is only returned once by <see cref="TestCompiler.FindVariables(string)"/>.
        /// </summary>
        [Component("Compiler")]
        [Fact]
        public void FindVariables_WithTheSameVariableMultipleTimesOnDifferentLines_SingleVariableReturned()
        {
            var text = "Hello {{world}}\n{{world}} hello";
            var result = TestCompiler.FindVariables(text);
            Assert.Single(result);
            Assert.Equal("world", result.First());
        }

        /// <summary>
        /// Verifies that two different variables are both detected by <see cref="TestCompiler.FindVariables(string)"/>.
        /// </summary>
        [Component("Compiler")]
        [Fact]
        public void FindVariables_WithTwoVariables_TwoVariableReturned()
        {
            var text = "Hello {{world1}}\n{{world2}} hello";
            var result = TestCompiler.FindVariables(text);
            Assert.Equal(2, result.Count);
            Assert.Equal("world1", result.First());
            Assert.Equal("world2", result.Last());
        }

        /// <summary>
        /// Verifies that <see cref="TestCompiler.ReplaceVariables"/> replaces a single variable in the template with its value.
        /// </summary>
        [Component("Compiler")]
        [Fact]
        public void ReplaceVariables_WithSingleVariableInTemplate()
        {
            var variables = new Dictionary<string, string>
            {
                ["KEY1"] = "value1",
                ["KEY2"] = "value2",
            };
            var context = new TestExecutionContext { ProjectId = 0, TeamId = 0, TestRunId = 0, Guid = "1" };
            var result = TestCompiler.ReplaceVariables(variables, "{{KEY1}}", context);

            Assert.Equal("value1", result);
        }

        /// <summary>
        /// Verifies that <see cref="TestCompiler.ReplaceVariables"/> replaces two variables in the template with their values.
        /// </summary>
        [Component("Compiler")]
        [Fact]
        public void ReplaceVariables_TwoVariablesInTemplate_PlaceholdersReplacedWithValues()
        {
            var variables = new Dictionary<string, string>
            {
                ["KEY1"] = "value1",
                ["KEY2"] = "value2",
            };

            var context = new TestExecutionContext { ProjectId = 0, TeamId = 0, TestRunId = 0, Guid = "1" };
            var result = TestCompiler.ReplaceVariables(variables, "{{KEY1}} {{KEY2}}", context);

            Assert.Equal("value1 value2", result);
        }

        /// <summary>
        /// Verifies that leading spaces are preserved when replacing variables in the template.
        /// </summary>
        [Component("Compiler")]
        [Fact]
        public void ReplaceVariables_WithLeadingSpace_SpacesPreserved()
        {
            var variables = new Dictionary<string, string>
            {
                ["KEY1"] = "value1",
                ["KEY2"] = "value2",
            };
            var context = new TestExecutionContext { ProjectId = 0, TeamId = 0, TestRunId = 0, Guid = "1" };
            var result = TestCompiler.ReplaceVariables(variables, " {{KEY1}} {{KEY2}}", context);

            Assert.Equal(" value1 value2", result);
        }

        /// <summary>
        /// Verifies that trailing spaces are preserved when replacing variables in the template.
        /// </summary>
        [Component("Compiler")]
        [Fact]
        public void ReplaceVariables_WithTrailingSpace_SpacesPreserved()
        {
            var variables = new Dictionary<string, string>
            {
                ["KEY1"] = "value1",
                ["KEY2"] = "value2",
            };
            var context = new TestExecutionContext { ProjectId = 0, TeamId = 0, TestRunId = 0, Guid = "1" };
            var result = TestCompiler.ReplaceVariables(variables, "{{KEY1}} {{KEY2}} ", context);

            Assert.Equal("value1 value2 ", result);
        }

        /// <summary>
        /// Verifies that when an unknown variable is encountered, it is left as a placeholder and an error is added to the context.
        /// </summary>
        [Component("Compiler")]
        [Fact]
        public void ReplaceVariables_WithUnknownVariable_ErrorCreated()
        {
            var variables = new Dictionary<string, string>
            {
                ["KEY1"] = "value1",
            };
            var context = new TestExecutionContext { ProjectId = 0, TeamId = 0, TestRunId = 0, Guid = "1" };
            var result = TestCompiler.ReplaceVariables(variables, "{{KEY1}} {{KEY2}} ", context);

            Assert.Equal("value1 {{KEY2}} ", result);
            Assert.Single(context.CompilerErrors);
        }
    }
}