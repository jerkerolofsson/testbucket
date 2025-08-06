using Mediator;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields;
using TestBucket.Domain.States;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Formats;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Testing.Services.Import;

public record class ImportTestCaseRunRequest(ClaimsPrincipal Principal, TestRun Run, TestCaseRunDto TestCaseRun, ImportHandlingOptions Options) : IRequest<TestCaseRun>;

public class ImportTestCaseRunHandler : IRequestHandler<ImportTestCaseRunRequest, TestCaseRun>
{
    private readonly IStateService _stateService;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly ITestRunManager _testRunManager;
    private readonly ITestCaseManager _testCaseManager;
    private readonly IFieldManager _fieldManager;

    public ImportTestCaseRunHandler(
        IStateService stateService,
        IFieldDefinitionManager fieldDefinitionManager,
        ITestRunManager testRunManager,
        ITestCaseManager testCaseManager,
        IFieldManager fieldManager)
    {
        _stateService = stateService;
        _fieldDefinitionManager = fieldDefinitionManager;
        _testCaseManager = testCaseManager;
        _fieldManager = fieldManager;
        _testRunManager = testRunManager;
    }

    public async ValueTask<TestCaseRun> Handle(ImportTestCaseRunRequest request, CancellationToken cancellationToken)
    {
        var principal = request.Principal;
        var tenantId = principal.GetTenantIdOrThrow();
        ArgumentNullException.ThrowIfNull(request.TestCaseRun.TestCaseSlug);
        var testRun = request.Run;
        var testCaseRunDto = request.TestCaseRun;
        var testCase = await _testCaseManager.GetTestCaseBySlugAsync(principal, request.Run.TestProjectId, request.TestCaseRun.TestCaseSlug);
        if (testCase is null)
        {
            throw new ArgumentException("Invalid test case slug");
        }
        if (testRun.TestProjectId is null)
        {
            throw new InvalidDataException("Test run has no project");
        }


        var testName = testCaseRunDto.Name ?? testCase.Name ?? "-";
        var completedState = await _stateService.GetTestCaseRunFinalStateAsync(principal, testRun.TestProjectId.Value);
        var testCaseRun = new TestCaseRun()
        {
            Name = testName,
            TenantId = tenantId,
            TestRunId = testRun.Id,
            TestProjectId = testRun.TestProjectId.Value,
            Result = TestResult.NoRun,
            TestCaseId = testCase.Id,
            State = completedState.Name
        };

        await _testRunManager.AddTestCaseRunAsync(principal, testCaseRun);

        // Set fields from DTO (overriding any inherited values)
        var fieldImporter = new TestCaseRunFieldImporter(principal, _fieldManager, _fieldDefinitionManager);
        await fieldImporter.ImportAsync(testCaseRunDto, testCaseRun);

        return testCaseRun;
    }
}