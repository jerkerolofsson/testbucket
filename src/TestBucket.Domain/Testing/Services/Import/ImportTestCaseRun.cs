using System.Diagnostics;
using System.Text;

using Mediator;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Files;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.States;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCases;
using TestBucket.Formats;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Testing.Services.Import;

public record class ImportTestCaseRunRequest(ClaimsPrincipal Principal, TestRun Run, TestCaseRunDto TestCaseRun, ImportHandlingOptions Options) : IRequest<TestCaseRun>;

public class ImportTestCaseRunHandler : IRequestHandler<ImportTestCaseRunRequest, TestCaseRun>
{
    private readonly IStateService _stateService;
    private readonly ITestCaseRepository _testCaseRepository;
    private readonly IFileRepository _fileRepository;
    private readonly ITestCaseManager _testCaseManager;

    public ImportTestCaseRunHandler(
        IStateService stateService,
        ITestCaseRepository testCaseRepository,
        IFieldDefinitionManager fieldDefinitionManager,
        IFileRepository fileRepository,
        ITestSuiteManager testSuiteManager,
        ITestRunManager testRunManager,
        ITestCaseManager testCaseManager,
        IFieldManager fieldManager,
        IProjectManager projectManager,
        ITeamManager teamManager)
    {
        _stateService = stateService;
        _testCaseRepository = testCaseRepository;
        _fileRepository = fileRepository;
        _testCaseManager = testCaseManager;
    }

    public async ValueTask<TestCaseRun> Handle(ImportTestCaseRunRequest request, CancellationToken cancellationToken)
    {
        var principal = request.Principal;
        var tenantId = principal.GetTenantIdOrThrow();
        ArgumentNullException.ThrowIfNull(request.TestCaseRun.TestCaseSlug);
        var testRun = request.Run;
        var testCase = await _testCaseManager.GetTestCaseBySlugAsync(principal, request.TestCaseRun.TestCaseSlug);
        if (testCase is null)
        {
            throw new ArgumentException("Invalid test case slug");
        }
        if (testRun.TestProjectId is null)
        {
            throw new InvalidDataException("Test run has no project");
        }


        var testName = testCase.Name ?? "-";
        var completedState = await _stateService.GetProjectFinalStateAsync(principal, testRun.TestProjectId.Value);
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

        await _testCaseRepository.AddTestCaseRunAsync(testCaseRun);

        // todo: Set fields from DTO

        return testCaseRun;
    }
}