using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Testing.Compiler;

namespace TestBucket.Domain.UnitTests.Compiler
{
    [UnitTest]
    [EnrichedTest]
    public class VariableTests
    {
        [Component("Compiler")]
        [Fact]
        public void FindVariables_WithEmptyString_EmptyListReturned()
        {
            var text = "";
            var result = TestCompiler.FindVariables(text);
            Assert.Empty(result);
        }

        [Component("Compiler")]
        [Fact]
        public void FindVariables_WithNoVariables_EmptyListReturned()
        {
            var text = "Hello World";
            var result = TestCompiler.FindVariables(text);
            Assert.Empty(result);
        }

        [Component("Compiler")]
        [Fact]
        public void FindVariables_WithVariableOneFirstLine_SingleVariableReturned()
        {
            var text = "Hello {{world}}";
            var result = TestCompiler.FindVariables(text);
            Assert.Single(result);
            Assert.Equal("world", result.First());
        }

        [Component("Compiler")]
        [Fact]
        public void FindVariables_WithTheSameVariableMultipleTimes_SingleVariableReturned()
        {
            var text = "Hello {{world}} {{world}}";
            var result = TestCompiler.FindVariables(text);
            Assert.Single(result);
            Assert.Equal("world", result.First());
        }
        [Component("Compiler")]
        [Fact]
        public void FindVariables_WithTheSameVariableMultipleTimesOnDifferentLines_SingleVariableReturned()
        {
            var text = "Hello {{world}}\n{{world}} hello";
            var result = TestCompiler.FindVariables(text);
            Assert.Single(result);
            Assert.Equal("world", result.First());
        }

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
