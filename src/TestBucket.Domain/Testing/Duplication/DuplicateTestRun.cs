using Mediator;

using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Localization;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Progress;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.Testing.Duplication;

public record class DuplicateTestRunRequest(ClaimsPrincipal Principal, TestRun Run, string Query) : IRequest<TestRun>;


public class DuplicateTestRunHandler : IRequestHandler<DuplicateTestRunRequest, TestRun>
{
    private readonly IProgressManager _progressManager;
    private readonly IAppLocalization _loc;
    private readonly ITestRunManager _testRunManager;
    private readonly IFieldManager _fieldManager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    public DuplicateTestRunHandler(
        IAppLocalization loc,
        ITestRunManager testRunManager, IFieldManager fieldManager, IProgressManager progressManager, IFieldDefinitionManager fieldDefinitionManager)
    {
        _loc = loc;
        _testRunManager = testRunManager;
        _fieldManager = fieldManager;
        _progressManager = progressManager;
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public async ValueTask<TestRun> Handle(DuplicateTestRunRequest request, CancellationToken cancellationToken)
    {
        var principal = request.Principal;
        var run = request.Run;
        ArgumentNullException.ThrowIfNull(run.TestProjectId);
        principal.ThrowIfEntityTenantIsDifferent(run);
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Write);
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Read);
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Write);
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read);

        await using var progress = _progressManager.CreateProgressTask(_loc.Shared["duplicating-test-run"]);

        await progress.ReportStatusAsync(_loc.Shared["duplicating-test-run"], 0);
        TestRun runCopy = await DuplicateTestRunAsync(principal, run);
        await DuplicateTestCaseRunsAsync(principal, run, runCopy, request.Query, progress);

        return runCopy;
    }

    private async Task DuplicateTestCaseRunsAsync(ClaimsPrincipal principal, TestRun run, TestRun runCopy, string queryText, ProgressTask progress)
    {
        var pageSize = 50;
        var offset = 0;
        var fields = await _fieldDefinitionManager.GetDefinitionsAsync(principal, run.TestProjectId, FieldTarget.TestCaseRun);
        var query = SearchTestCaseRunQueryParser.Parse(queryText, fields);
        query.TestRunId = run.Id;
        while (true)
        {
            query.Count = pageSize;
            query.Offset = offset;

            //var query = new SearchTestCaseRunQuery { TestRunId = run.Id, Count = pageSize, Offset = offset };
            var result = await _testRunManager.SearchTestCaseRunsAsync(principal, query);

            if (result.TotalCount > 0)
            {
                await progress.ReportStatusAsync(_loc.Shared["duplicating-test-run"], 100.0 * offset / (double)result.TotalCount);
            }
            foreach (var testCaseRun in result.Items)
            {
                var testCaseRunCopy = testCaseRun.Duplicate();
                testCaseRunCopy.TestRunId = runCopy.Id;
                await _testRunManager.AddTestCaseRunAsync(principal, testCaseRunCopy);
            }

            if (result.Items.Length != pageSize)
            {
                break;
            }
            offset += result.Items.Length;
        }
    }

    private async Task<TestRun> DuplicateTestRunAsync(ClaimsPrincipal principal, TestRun run)
    {
        TestRun runCopy = run.Duplicate();

        // A duplicate run should always be open
        runCopy.Open = true;

        await _testRunManager.AddTestRunAsync(principal, runCopy);

        // Copy fields from the previous run
        var fields = run.TestRunFields?.ToList() ?? await _fieldManager.GetTestRunFieldsAsync(principal, run.Id, []);
        foreach (var field in fields)
        {
            var fieldCopy = new TestRunField { FieldDefinitionId = field.Id, TestRunId = runCopy.Id };
            field.CopyTo(fieldCopy);
            await _fieldManager.UpsertTestRunFieldAsync(principal, fieldCopy);
        }

        return runCopy;
    }
}
