using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestCases.Templates;
internal class PythonTemplate : ITestCaseTemplate
{
    public string Name => "python";

    public string? Icon => TbIcons.ProgrammingLanguages.Python;

    public ValueTask ApplyAsync(TestCase test)
    {
        test.RunnerLanguage = "python";
        test.ExecutionType = TestExecutionType.Automated;
        test.ScriptType = ScriptType.ScriptedDefault;

        test.Description = $"""
            print("Hello, World")
            """;

        return ValueTask.CompletedTask;
    }
}
