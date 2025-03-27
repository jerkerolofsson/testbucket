
using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Testing.Models;
using DotHttpTest;
using DotHttpTest.Runner;

namespace TestBucket.ApiTests;

public class DotHttpMarkdownTestRunner : IMarkdownTestRunner
{
    public string Language => "http";

    public Task<TestRunnerResult> RunAsync(TestExecutionContext context, string code)
    {
        throw new NotImplementedException();
    }
}
