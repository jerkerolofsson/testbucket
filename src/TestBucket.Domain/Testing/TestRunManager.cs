using System.Security.Claims;

using TestBucket.Domain.Fields;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Aggregates;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCaseRuns;
using TestBucket.Domain.Testing.Specifications.TestRuns;


namespace TestBucket.Domain.Testing;
internal class TestRunManager : ITestRunManager
{
    private readonly List<ITestRunObserver> _testRunObservers = new();

    private readonly ITestCaseRepository _testCaseRepo;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly IFieldManager _fieldManager;

    public TestRunManager(ITestCaseRepository testCaseRepo, IFieldDefinitionManager fieldDefinitionManager, IFieldManager fieldManager)
    {
        _testCaseRepo = testCaseRepo;
        _fieldDefinitionManager = fieldDefinitionManager;
        _fieldManager = fieldManager;
    }

    /// <summary>
    /// Adds an observer
    /// </summary>
    /// <param name="listener"></param>
    public void AddObserver(ITestRunObserver observer) => _testRunObservers.Add(observer);

    /// <summary>
    /// Removes an observer
    /// </summary>
    /// <param name="observer"></param>
    public void RemoveObserver(ITestRunObserver observer) => _testRunObservers.Remove(observer);

    /// <inheritdoc/>
    public async Task AddTestRunAsync(ClaimsPrincipal principal, TestRun testRun)
    {
        testRun.TenantId = principal.GetTenantIdOrThrow();

        testRun.Modified = testRun.Created = DateTimeOffset.UtcNow;
        testRun.CreatedBy = testRun.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");

        await _testCaseRepo.AddTestRunAsync(testRun);

        // Notify observers
        foreach (var observer in _testRunObservers.ToList())
        {
            await observer.OnRunCreatedAsync(testRun);
        }
    }

    /// <inheritdoc/>
    public async Task DeleteTestRunAsync(ClaimsPrincipal principal, TestRun testRun)
    {
        principal.GetTenantIdOrThrow(testRun);
        await _testCaseRepo.DeleteTestRunByIdAsync(testRun.Id);

        // Notify observers
        foreach (var observer in _testRunObservers.ToList())
        {
            await observer.OnRunDeletedAsync(testRun);
        }
    }

    /// <inheritdoc/>
    public Task DeleteTestCaseRunAsync(ClaimsPrincipal principal, TestCaseRun testCaseRun)
    {
        principal.GetTenantIdOrThrow(testCaseRun);
        //await _testCaseRepo.DeleteTestRunByIdAsync(testRun.Id);

        throw new NotImplementedException("todo");
    }

    /// <inheritdoc/>
    public async Task AddTestCaseRunAsync(ClaimsPrincipal principal, TestCaseRun testCaseRun)
    {
        testCaseRun.TenantId = principal.GetTenantIdOrThrow();

        testCaseRun.Modified = testCaseRun.Created = DateTimeOffset.UtcNow;
        testCaseRun.CreatedBy = testCaseRun.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");

        await _testCaseRepo.AddTestCaseRunAsync(testCaseRun);

        await CreateInheritedTestCaseRunFieldsAsync(principal, testCaseRun);

        // Notify observers
        foreach (var observer in _testRunObservers)
        {
            await observer.OnTestCaseRunCreatedAsync(testCaseRun);
        }
    }

    /// <summary>
    /// Adds fields with values inherited from TestRun and TestCase
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testCaseRun"></param>
    /// <returns></returns>
    private async Task CreateInheritedTestCaseRunFieldsAsync(ClaimsPrincipal principal, TestCaseRun testCaseRun)
    {
        // Add inherited fields from test run to the test case run
        var runFieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, testCaseRun.TestProjectId, FieldTarget.TestRun);
        var testCaseDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, testCaseRun.TestCaseId, FieldTarget.TestRun);

        var testRunFields = await _fieldManager.GetTestRunFieldsAsync(principal, testCaseRun.TestRunId, runFieldDefinitions);
        var testCaseFields = await _fieldManager.GetTestCaseFieldsAsync(principal, testCaseRun.TestCaseId, runFieldDefinitions);
        List<TestCaseRunField> testCaseRunFields = new();
        foreach (var field in testCaseFields)
        {
            AppendField(testCaseRun, testCaseRunFields, field);
        }
        foreach (var field in testRunFields)
        {
            AppendField(testCaseRun, testCaseRunFields, field);
        }
        if (testCaseRunFields.Count > 0)
        {
            await _fieldManager.SaveTestCaseRunFieldsAsync(principal, testCaseRunFields);
        }
    }

    /// <summary>
    /// Appends a field if it has a value and it targets TestCaseRun
    /// </summary>
    /// <param name="testCaseRun"></param>
    /// <param name="testCaseRunFields"></param>
    /// <param name="field"></param>
    private static void AppendField(TestCaseRun testCaseRun, List<TestCaseRunField> testCaseRunFields, FieldValue field)
    {
        ArgumentNullException.ThrowIfNull(field.FieldDefinition);
        if(!field.FieldDefinition.Inherit)
        {
            // Field inheritance is disabled
            return;
        }
        if(!field.HasValue())
        {
            // No action if the field has no value
            return;
        }

        if ((field.FieldDefinition.Target & FieldTarget.TestCaseRun) == FieldTarget.TestCaseRun && field.HasValue())
        {
            TestCaseRunField testCaseRunField = new TestCaseRunField
            {
                FieldDefinitionId = field.FieldDefinitionId,
                TestRunId = testCaseRun.TestRunId,
                TestCaseRunId = testCaseRun.Id
            };
            field.CopyTo(testCaseRunField);
            testCaseRunFields.RemoveAll(x => x.FieldDefinitionId == field.FieldDefinitionId);
            testCaseRunFields.Add(testCaseRunField);
        }
    }

    /// <inheritdoc/>
    public async Task SaveTestCaseRunAsync(ClaimsPrincipal principal, TestCaseRun testCaseRun)
    {
        principal.GetTenantIdOrThrow(testCaseRun);

        testCaseRun.Modified =  DateTimeOffset.UtcNow;
        testCaseRun.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");

        await _testCaseRepo.UpdateTestCaseRunAsync(testCaseRun);

        foreach(var observer in _testRunObservers)
        {
            await observer.OnTestCaseRunUpdatedAsync(testCaseRun);
        }
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TestRun>> SearchTestRunsAsync(ClaimsPrincipal principal, SearchTestRunQuery query)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        List<FilterSpecification<TestRun>> filters = TestRunFilterSpecificationBuilder.From(query);
        filters.Add(new FilterByTenant<TestRun>(tenantId));

        return await _testCaseRepo.SearchTestRunsAsync(filters, query.Offset, query.Count);
    }

    /// <inheritdoc/>
    public async Task<TestExecutionResultSummary> GetTestExecutionResultSummaryAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        List<FilterSpecification<TestCaseRun>> filters = TestCaseRunsFilterSpecificationBuilder.From(query);
        filters.Add(new FilterByTenant<TestCaseRun>(tenantId));
        return await _testCaseRepo.GetTestExecutionResultSummaryAsync(filters);
    }
    /// <inheritdoc/>
    public async Task<Dictionary<string, TestExecutionResultSummary>> GetTestExecutionResultSummaryByFieldAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query, long fieldDefinitionId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        List<FilterSpecification<TestCaseRun>> filters = TestCaseRunsFilterSpecificationBuilder.From(query);
        filters.Add(new FilterByTenant<TestCaseRun>(tenantId));
        return await _testCaseRepo.GetTestExecutionResultSummaryByFieldAsync(filters, fieldDefinitionId);
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TestCaseRun>> SearchTestCaseRunsAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        List<FilterSpecification<TestCaseRun>> filters = TestCaseRunsFilterSpecificationBuilder.From(query);
        filters.Add(new FilterByTenant<TestCaseRun>(tenantId));

        return await _testCaseRepo.SearchTestCaseRunsAsync(filters, query.Offset, query.Count);
    }
}
