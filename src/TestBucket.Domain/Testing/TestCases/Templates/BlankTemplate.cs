using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestCases.Templates;
internal class BlankTemplate : ITestCaseTemplate
{
    public string Name => "blank";

    public string? Icon => null;

    public ValueTask ApplyAsync(TestCase test)
    {
        test.RunnerLanguage = null;
        test.ExecutionType = TestExecutionType.Manual;
        test.ScriptType = ScriptType.ScriptedDefault;

        return ValueTask.CompletedTask;
    }
}
