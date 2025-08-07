using Mediator;

using TestBucket.Domain.Fields;
using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Aggregates;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCaseRuns;
using TestBucket.Domain.Testing.Specifications.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Events;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.Testing.TestRuns;
internal class TestRunManager : ITestRunManager
{
    private readonly List<ITestRunObserver> _testRunObservers = new();

    private readonly ITestCaseRepository _testCaseRepo;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly IFieldManager _fieldManager;
    private readonly IMediator _mediator;
    private readonly TimeProvider _timeProvider;

    public TestRunManager(ITestCaseRepository testCaseRepo, IFieldDefinitionManager fieldDefinitionManager, IFieldManager fieldManager, IMediator mediator, TimeProvider timeProvider)
    {
        _testCaseRepo = testCaseRepo;
        _fieldDefinitionManager = fieldDefinitionManager;
        _fieldManager = fieldManager;
        _mediator = mediator;
        _timeProvider = timeProvider;
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
    public async Task<TestCaseRun?> GetTestCaseRunByIdAsync(ClaimsPrincipal principal, long id)
    {
        return await _testCaseRepo.GetTestCaseRunByIdAsync(principal.GetTenantIdOrThrow(), id);
    }

    /// <inheritdoc/>
    public async Task<TestRun?> GetTestRunByIdAsync(ClaimsPrincipal principal, long id)
    {
        return await _testCaseRepo.GetTestRunByIdAsync(principal.GetTenantIdOrThrow(), id);
    }

    /// <inheritdoc/>
    public async Task<TestRun?> GetTestRunBySlugAsync(ClaimsPrincipal principal, long? projectId, string slug)
    {
        return await _testCaseRepo.GetTestRunBySlugAsync(principal.GetTenantIdOrThrow(), projectId, slug);
    }

    /// <inheritdoc/>
    public async Task AddTestRunAsync(ClaimsPrincipal principal, TestRun testRun)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Write);

        testRun.TenantId = principal.GetTenantIdOrThrow();
        testRun.Modified = testRun.Created = _timeProvider.GetUtcNow();
        testRun.CreatedBy = testRun.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");

        await _testCaseRepo.AddTestRunAsync(testRun);

        // Notify observers
        foreach (var observer in _testRunObservers.ToList())
        {
            await observer.OnRunCreatedAsync(testRun);
        }
    }

    /// <inheritdoc/>
    public async Task ArchiveTestRunAsync(ClaimsPrincipal principal, TestRun testRun)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Write);
        principal.GetTenantIdOrThrow(testRun);
        testRun.Archived = true;
        await SaveTestRunAsync(principal, testRun);
    }

    /// <inheritdoc/>
    public async Task DeleteTestRunAsync(ClaimsPrincipal principal, TestRun testRun)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Delete);

        principal.GetTenantIdOrThrow(testRun);
        await _testCaseRepo.DeleteTestRunAsync(testRun);

        // Notify observers
        foreach (var observer in _testRunObservers.ToList())
        {
            await observer.OnRunDeletedAsync(testRun);
        }
    }

    /// <inheritdoc/>
    public async Task DeleteTestCaseRunAsync(ClaimsPrincipal principal, TestCaseRun testCaseRun)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Delete);
        principal.ThrowIfEntityTenantIsDifferent(testCaseRun);

        await _testCaseRepo.DeleteTestCaseRunAsync(testCaseRun);

    }

    /// <inheritdoc/>
    public async Task<TestCaseRun> AddTestCaseRunAsync(ClaimsPrincipal principal, TestRun testRun, TestCase testCase)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Write);

        if (testRun.TestProjectId is null)
        {
            throw new ArgumentException("TestRun must belong to a project!");
        }

        var testCaseRun = new TestCaseRun
        {
            Name = testCase.Name,
            TestCaseId = testCase.Id,
            TestRunId = testRun.Id,
            TestProjectId = testRun.TestProjectId.Value,
            State = "Not Started",
            Result = TestResult.NoRun,
            ScriptType = testCase.ScriptType,
            AssignedToUserName = testRun.AssignTestCaseRunsTo
        };
        if (testCase.ScriptType == ScriptType.Exploratory)
        {
            testCaseRun.Charter = testCase.Description;
            if (testCase.SessionDuration is not null)
            {
                // From minutes to milliseconds
                testCaseRun.Estimate = testCase.SessionDuration.Value * 1000 * 60;
            }
        }
        await AddTestCaseRunAsync(principal, testCaseRun);

        if(testCaseRun.AssignedToUserName is not null)
        {
            await _mediator.Publish(new TestCaseRunAssignmentChangedNotification(principal, testCaseRun, null));
        }

        return testCaseRun;
    }

    /// <inheritdoc/>
    public async Task AddTestCaseRunAsync(ClaimsPrincipal principal, TestCaseRun testCaseRun)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Write);

        testCaseRun.TenantId = principal.GetTenantIdOrThrow();
        testCaseRun.Modified = testCaseRun.Created = _timeProvider.GetUtcNow();
        testCaseRun.CreatedBy = testCaseRun.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");

        await _testCaseRepo.AddTestCaseRunAsync(testCaseRun);

        await _mediator.Publish(new TestCaseRunSavedNotification(principal, testCaseRun));

        // Notify observers
        foreach (var observer in _testRunObservers)
        {
            await observer.OnTestCaseRunCreatedAsync(testCaseRun);
        }
    }

    ///// <summary>
    ///// Appends a field if it has a value and it targets TestCaseRun
    ///// </summary>
    ///// <param name="testCaseRun"></param>
    ///// <param name="testCaseRunFields"></param>
    ///// <param name="field"></param>
    //private static void AppendField(TestCaseRun testCaseRun, List<TestCaseRunField> testCaseRunFields, FieldValue field)
    //{
    //    ArgumentNullException.ThrowIfNull(field.FieldDefinition);
    //    if(!field.FieldDefinition.Inherit)
    //    {
    //        // Field inheritance is disabled
    //        return;
    //    }
    //    if(!field.HasValue())
    //    {
    //        // No action if the field has no value
    //        return;
    //    }

    //    if ((field.FieldDefinition.Target & FieldTarget.TestCaseRun) == FieldTarget.TestCaseRun && field.HasValue())
    //    {
    //        TestCaseRunField testCaseRunField = new TestCaseRunField
    //        {
    //            FieldDefinitionId = field.FieldDefinitionId,
    //            TestRunId = testCaseRun.TestRunId,
    //            TestCaseRunId = testCaseRun.Id
    //        };
    //        field.CopyTo(testCaseRunField);
    //        testCaseRunFields.RemoveAll(x => x.FieldDefinitionId == field.FieldDefinitionId);
    //        testCaseRunFields.Add(testCaseRunField);
    //    }
    //}


    /// <inheritdoc/>
    public async Task SaveTestRunAsync(ClaimsPrincipal principal, TestRun testRun)
    {
        var tenantId = principal.ThrowIfEntityTenantIsDifferent(testRun);
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Write);

        var existingRun = await _testCaseRepo.GetTestRunByIdAsync(tenantId,testRun.Id);
        if(existingRun is null)
        {
            throw new ArgumentException("Existing run was not found");
        }

        testRun.Modified = _timeProvider.GetUtcNow();
        testRun.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");

        await _testCaseRepo.UpdateTestRunAsync(testRun);

        if(existingRun.FolderId != testRun.FolderId)
        {
            foreach (var observer in _testRunObservers)
            {
                await observer.OnRunMovedAsync(testRun);
            }

        }

        foreach (var observer in _testRunObservers)
        {
            await observer.OnRunUpdatedAsync(testRun);
        }
    }

    /// <inheritdoc/>
    public async Task SaveTestCaseRunAsync(ClaimsPrincipal principal, TestCaseRun testCaseRun, bool informObservers = true)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.GetTenantIdOrThrow(testCaseRun);
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Write);

        var existing = await _testCaseRepo.GetTestCaseRunByIdAsync(tenantId, testCaseRun.Id);
        bool assignmentChanged = existing?.AssignedToUserName != testCaseRun.AssignedToUserName;
        bool wasUnassigned = assignmentChanged && string.IsNullOrEmpty(testCaseRun.AssignedToUserName);
        bool wasAssigned = assignmentChanged && !string.IsNullOrEmpty(testCaseRun.AssignedToUserName);

        testCaseRun.Modified = _timeProvider.GetUtcNow();
        testCaseRun.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");
        testCaseRun.TestCaseRunFields = await _fieldManager.GetTestCaseRunFieldsAsync(principal, testCaseRun.TestRunId, testCaseRun.Id, []);

        // Update the state if it has been assigned or unassigned
        if(existing is not null)
        {
            await _mediator.Publish(new TestCaseRunUpdatingNotification(principal, existing, testCaseRun));
        }

        // Save in DB
        await _testCaseRepo.UpdateTestCaseRunAsync(testCaseRun);

        // Notifications and events
        await _mediator.Publish(new TestCaseRunSavedNotification(principal, testCaseRun));
        if(assignmentChanged)
        {
            await _mediator.Publish(new TestCaseRunAssignmentChangedNotification(principal, testCaseRun, existing?.AssignedToUserName));
        }
        if (informObservers)
        {
            foreach (var observer in _testRunObservers)
            {
                await observer.OnTestCaseRunUpdatedAsync(testCaseRun);
            }
        }
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TestRun>> SearchTestRunsAsync(ClaimsPrincipal principal, SearchTestRunQuery query)
    {
        var tenantId = principal.GetTenantIdOrThrow(); 
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read);

        List<FilterSpecification<TestRun>> filters = TestRunFilterSpecificationBuilder.From(query);
        filters.Add(new FilterByTenant<TestRun>(tenantId));

        return await _testCaseRepo.SearchTestRunsAsync(filters, query.Offset, query.Count);
    }

    /// <inheritdoc/>
    public async Task<TestExecutionResultSummary> GetTestExecutionResultSummaryAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read);

        List<FilterSpecification<TestCaseRun>> filters = TestCaseRunsFilterSpecificationBuilder.From(query);
        filters.Add(new FilterByTenant<TestCaseRun>(tenantId));
        return await _testCaseRepo.GetTestExecutionResultSummaryAsync(filters);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<DateOnly, TestExecutionResultSummary>> GetTestExecutionResultSummaryByDayAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read);

        List<FilterSpecification<TestCaseRun>> filters = TestCaseRunsFilterSpecificationBuilder.From(query);
        filters.Add(new FilterByTenant<TestCaseRun>(tenantId));
        return await _testCaseRepo.GetTestExecutionResultSummaryByDayAsync(filters);
    }

    /// <inheritdoc/>
    public async Task<InsightsData<TestResult, int>> GetInsightsTestResultsAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read);

        List<FilterSpecification<TestCaseRun>> filters = TestCaseRunsFilterSpecificationBuilder.From(query);
        filters.Add(new FilterByTenant<TestCaseRun>(tenantId));
        return await _testCaseRepo.GetInsightsTestResultsAsync(filters);
    }

    /// <inheritdoc/>
    public async Task<InsightsData<TestResult, int>> GetInsightsLatestTestResultsAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read);

        List<FilterSpecification<TestCaseRun>> filters = TestCaseRunsFilterSpecificationBuilder.From(query);
        filters.Add(new FilterByTenant<TestCaseRun>(tenantId));
        return await _testCaseRepo.GetInsightsLatestTestResultsAsync(filters);
    }

    public async Task<InsightsData<string, int>> GetInsightsTestCaseRunCountByAsigneeAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read);

        List<FilterSpecification<TestCaseRun>> filters = TestCaseRunsFilterSpecificationBuilder.From(query);
        return await _testCaseRepo.GetInsightsTestCaseRunCountByAssigneeAsync(filters);
    }

    public async Task<InsightsData<string, int>> GetInsightsTestResultsByFieldAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query, long fieldDefinitionId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read);

        List<FilterSpecification<TestCaseRun>> filters = TestCaseRunsFilterSpecificationBuilder.From(query);
        return await _testCaseRepo.GetInsightsTestResultsByFieldAsync(filters, fieldDefinitionId);
    }


    /// <inheritdoc/>
    public async Task<InsightsData<DateOnly, int>> GetInsightsTestResultsByDayAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read);

        List<FilterSpecification<TestCaseRun>> filters = TestCaseRunsFilterSpecificationBuilder.From(query);
        filters.Add(new FilterByTenant<TestCaseRun>(tenantId));
        return await _testCaseRepo.GetInsightsTestResultsByDayAsync(filters);
    }


    /// <inheritdoc/>
    public async Task<Dictionary<string, TestExecutionResultSummary>> GetTestExecutionResultSummaryByFieldAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query, long fieldDefinitionId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read);

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

    public async Task<TestRun> DuplicateTestRunAsync(ClaimsPrincipal principal, TestRun run)
    {
        return await DuplicateTestRunAsync(principal, run, "");
    }

    public async Task<TestRun> DuplicateTestRunAsync(ClaimsPrincipal principal, TestRun run, string filter)
    {
        return await _mediator.Send(new Duplication.DuplicateTestRunRequest(principal, run, filter));
    }
}
