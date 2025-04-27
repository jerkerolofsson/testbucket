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
        var testCase = await _testCaseManager.GetTestCaseBySlugAsync(principal, request.TestCaseRun.TestCaseSlug);
        if (testCase is null)
        {
            throw new ArgumentException("Invalid test case slug");
        }
        if (testRun.TestProjectId is null)
        {
            throw new InvalidDataException("Test run has no project");
        }


        var testName = testCaseRunDto.Name ?? testCase.Name ?? "-";
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

        await _testRunManager.AddTestCaseRunAsync(principal, testCaseRun);

        // Set fields from DTO (overriding any inherited values)
        var fieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, testRun.TestProjectId.Value, FieldTarget.TestCaseRun);
        var fields = await _fieldManager.GetTestCaseRunFieldsAsync(principal, testRun.Id, testCaseRun.Id, fieldDefinitions);

        foreach(var field in fields)
        {
            var fieldDefinition = field.FieldDefinition;
            if(fieldDefinition is null)
            {
                continue;
            }
            var values = testCaseRunDto.Traits.Where(x => x.Type == fieldDefinition.TraitType).Select(x => x.Value).ToArray();
            if (FieldValueConverter.TryAssignValue(fieldDefinition, field, values))
            {
                await _fieldManager.UpsertTestCaseRunFieldAsync(principal, field);
            }
        }

        return testCaseRun;
    }
}