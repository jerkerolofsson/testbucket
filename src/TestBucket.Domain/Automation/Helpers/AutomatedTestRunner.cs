using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using TestBucket.Domain.Automation.Hybrid;
using TestBucket.Domain.Testing.Models;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Automation.Helpers;
public class AutomatedTestRunner
{
    /// <summary>
    /// Runs a compiled automated test on a test runner and sets the result of the test case run
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="testCaseRun"></param>
    /// <param name="testCase"></param>
    /// <param name="principal"></param>
    /// <param name="testExecutionContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    internal static async Task<TestRunnerResult> RunAsync(IServiceScope scope, TestCaseRun testCaseRun, TestCase testCase, ClaimsPrincipal principal, TestExecutionContext testExecutionContext, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(testCase.RunnerLanguage))
        {
            throw new Exception("No runner language is defined for this test!");
        }
        if (string.IsNullOrEmpty(testExecutionContext.CompiledText))
        {
            throw new Exception("Test case script is missing");
        }

        var compiledCode = testExecutionContext.CompiledText;
        var markdownAutomationRunner = scope.ServiceProvider.GetRequiredService<IMarkdownAutomationRunner>();
        return await markdownAutomationRunner.EvalAsync(principal, testExecutionContext, testCase.RunnerLanguage, compiledCode);
    }
}
