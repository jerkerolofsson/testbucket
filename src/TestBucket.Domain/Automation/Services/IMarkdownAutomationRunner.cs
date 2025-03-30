using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Automation.Services;

/// <summary>
/// Runs test cases where the script is embedded in markdown
/// </summary>
public interface IMarkdownAutomationRunner
{
    /// <summary>
    /// Runs the code but does not import the results
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="context"></param>
    /// <param name="language"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    Task<TestRunnerResult> EvalAsync(ClaimsPrincipal principal, TestExecutionContext context, string language, string code);

    /// <summary>
    /// Runs the specified code using a runner identified by the language.
    /// Imports the result to the specified test run
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="context"></param>
    /// <param name="language"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">A runner was not found, or the test run has been deleted</exception>
    Task RunAsync(ClaimsPrincipal principal, TestExecutionContext context, string language, string code);

    /// <summary>
    /// Returns true if there is a runner supporting the language
    /// </summary>
    /// <param name="language"></param>
    /// <returns></returns>
    bool SupportsLanguage(string language);
}
