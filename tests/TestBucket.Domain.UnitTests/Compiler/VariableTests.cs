using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Markdown;
using TestBucket.Domain.Testing.Compiler;

namespace TestBucket.Domain.UnitTests.Compiler
{
    [UnitTest]
    public class VariableTests
    {
        [Test]
        public async Task ReplaceVariables_WithSingleVariableInTemplate()
        {
            var variables = new Dictionary<string, string>
            {
                ["KEY1"] = "value1",
                ["KEY2"] = "value2",
            };
            var context = new TestExecutionContext { ProjectId = 0, TeamId = 0, TestRunId = 0 };
            var result = TestCompiler.ReplaceVariables(variables, "{{KEY1}}", context);

            await Assert.That(result).IsEqualTo("value1");
        }

        [Test]
        public async Task ReplaceVariables_TwoVariablesInTemplate()
        {
            var variables = new Dictionary<string, string>
            {
                ["KEY1"] = "value1",
                ["KEY2"] = "value2",
            };

            var context = new TestExecutionContext { ProjectId = 0, TeamId = 0, TestRunId = 0 };
            var result = TestCompiler.ReplaceVariables(variables, "{{KEY1}} {{KEY2}}", context);

            await Assert.That(result).IsEqualTo("value1 value2");
        }

        [Test]
        public async Task ReplaceVariables_WithLeadingSpace()
        {
            var variables = new Dictionary<string, string>
            {
                ["KEY1"] = "value1",
                ["KEY2"] = "value2",
            };
            var context = new TestExecutionContext { ProjectId = 0, TeamId = 0, TestRunId = 0 };
            var result = TestCompiler.ReplaceVariables(variables, " {{KEY1}} {{KEY2}}", context);

            await Assert.That(result).IsEqualTo(" value1 value2");
        }

        [Test]
        public async Task ReplaceVariables_WithTrailingSpace()
        {
            var variables = new Dictionary<string, string>
            {
                ["KEY1"] = "value1",
                ["KEY2"] = "value2",
            };
            var context = new TestExecutionContext { ProjectId = 0, TeamId = 0, TestRunId = 0 };
            var result = TestCompiler.ReplaceVariables(variables, "{{KEY1}} {{KEY2}} ", context);

            await Assert.That(result).IsEqualTo("value1 value2 ");
        }


        [Test]
        public async Task ReplaceVariables_WithUnknownVariable_ErrorCreated()
        {
            var variables = new Dictionary<string, string>
            {
                ["KEY1"] = "value1",
            };
            var context = new TestExecutionContext { ProjectId = 0, TeamId = 0, TestRunId = 0 };
            var result = TestCompiler.ReplaceVariables(variables, "{{KEY1}} {{KEY2}} ", context);

            await Assert.That(result).IsEqualTo("value1 {{KEY2}} ");
            await Assert.That(context.CompilerErrors.Count).IsEqualTo(1);
        }
    }
}
