using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestCases.Templates;
internal class PowershellTemplate : ITestCaseTemplate
{
    public string Name => "pwsh";

    public string? Icon => TbIcons.ProgrammingLanguages.PowerShell;

    public ValueTask ApplyAsync(TestCase test)
    {
        test.RunnerLanguage = "pwsh";
        test.ExecutionType = TestExecutionType.Automated;
        test.ScriptType = ScriptType.ScriptedDefault;

        test.Description = $"""
            Write-Host "Hello, World"
            """;

        return ValueTask.CompletedTask;
    }
}
