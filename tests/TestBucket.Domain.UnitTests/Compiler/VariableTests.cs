using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Testing.Compiler;

namespace TestBucket.Domain.UnitTests.Compiler
{
    [UnitTest]
    public class VariableTests
    {
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
