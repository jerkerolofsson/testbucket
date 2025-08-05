using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestCases.Templates;
internal class DotHttpMcpTemplate : ITestCaseTemplate
{
    public string Name => "dothttp.mcp";

    public string? Icon => TbIcons.Filled.ModelContextProtocol;

    public ValueTask ApplyAsync(TestCase test)
    {
        test.RunnerLanguage = "http";
        test.ExecutionType = TestExecutionType.Automated;
        test.ScriptType = ScriptType.ScriptedDefault;

        test.Description = $$"""
            # @name {{test.Name}}
            # @verify mcp success
            CALL http://localhost:32223#browser_navigate MCP/SSE

            {
                "url": "https://www.github.com"
            }
            """;

        return ValueTask.CompletedTask;
    }
}
