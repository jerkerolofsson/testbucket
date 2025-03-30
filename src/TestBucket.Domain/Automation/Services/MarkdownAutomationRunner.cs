using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Progress;
using TestBucket.Domain.Testing;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Automation.Services;

/// <summary>
/// Runs test cases where the script is embedded in markdown
/// </summary>
internal class MarkdownAutomationRunner : IMarkdownAutomationRunner
{
    private readonly List<IMarkdownTestRunner> _runners;
    private readonly HashSet<string> _supportedLanguages;
    private readonly ITextTestResultsImporter _importer;
    private readonly IProgressManager _progressManager;

    public MarkdownAutomationRunner(IEnumerable<IMarkdownTestRunner> runners, ITextTestResultsImporter importer, IProgressManager progressManager)
    {
        _runners = runners.ToList();
        _supportedLanguages = new HashSet<string>(_runners.Select(x => x.Language).Distinct());
        _importer = importer;
        _progressManager = progressManager;
    }

    /// <summary>
    /// Returns true if there is a runner supporting the language
    /// </summary>
    /// <param name="language"></param>
    /// <returns></returns>
    public bool SupportsLanguage(string language)
    {
        return _supportedLanguages.Contains(language);
    }

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
    public async Task RunAsync(ClaimsPrincipal principal, TestExecutionContext context, string language, string code)
    {
        var runner = _runners.Where(x => x.Language == language).FirstOrDefault();
        if(runner is null)
        {
            throw new InvalidOperationException("Test runner not found");
        }

        await using var task = _progressManager.CreateProgressTask($"Running {language}..");
        await task.ReportStatusAsync("Evaluating code..", 10);
        var result =await runner.RunAsync(context,code, task.CancellationToken);

        await task.ReportStatusAsync("Importing result..", 90);
        await ImportResultAsync(principal, context, result);
    }

    /// <summary>
    /// Runs the code but does not import the results
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="context"></param>
    /// <param name="language"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<TestRunnerResult> EvalAsync(ClaimsPrincipal principal, TestExecutionContext context, string language, string code)
    {
        var runner = _runners.Where(x => x.Language == language).FirstOrDefault();
        if (runner is null)
        {
            throw new InvalidOperationException("Test runner not found");
        }

        await using var task = _progressManager.CreateProgressTask($"Running {language}..");
        await task.ReportStatusAsync("Evaluating code..", 10);
        return await runner.RunAsync(context, code, task.CancellationToken);
    }

    /// <summary>
    /// Imports the results
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="context"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    private async Task ImportResultAsync(ClaimsPrincipal principal, TestExecutionContext context, TestRunnerResult result)
    {
        if(string.IsNullOrEmpty(result.Result))
        {
            throw new InvalidOperationException("A result was returned from the runner but the Result data is missing");
        }
        var runId = context.TestRunId;
        await _importer.ImportTextAsync(principal, context.TeamId, context.ProjectId, result.Format, result.Result, new Formats.ImportHandlingOptions
        {
            TestCaseId = context.TestCaseId,
            TestRunId = runId
        });
    }
}
