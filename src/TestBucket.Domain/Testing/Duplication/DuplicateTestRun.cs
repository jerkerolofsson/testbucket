using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;
using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields.Handlers;

using TestBucket.Domain.Fields;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Progress;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.Testing.Duplication;

public record class DuplicateTestRunRequest(ClaimsPrincipal Principal, TestRun Run) : IRequest<TestRun>;


public class DuplicateTestRunHandler : IRequestHandler<DuplicateTestRunRequest, TestRun>
{
    private readonly IProgressManager _progressManager;
    private readonly ITestRunManager _testRunManager;
    private readonly IFieldManager _fieldManager;

    public DuplicateTestRunHandler(ITestRunManager testRunManager, IFieldManager fieldManager, IProgressManager progressManager)
    {
        _testRunManager = testRunManager;
        _fieldManager = fieldManager;
        _progressManager = progressManager;
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

        await using var progress = _progressManager.CreateProgressTask("Working");

        await progress.ReportStatusAsync("Duplicating run", 0);
        TestRun runCopy = await DuplicateTestRunAsync(principal, run);
        await DuplicateTestCaseRunsAsync(principal, run, runCopy, progress);

        return runCopy;
    }

    private async Task DuplicateTestCaseRunsAsync(ClaimsPrincipal principal, TestRun run, TestRun runCopy, ProgressTask progress)
    {
        var pageSize = 50;
        var offset = 0;
        while (true)
        {
            var query = new SearchTestCaseRunQuery { TestRunId = run.Id, Count = pageSize, Offset = offset };
            var result = await _testRunManager.SearchTestCaseRunsAsync(principal, query);

            if (result.TotalCount > 0)
            {
                await progress.ReportStatusAsync("Duplicating tests", 100.0 * offset / (double)result.TotalCount);
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
