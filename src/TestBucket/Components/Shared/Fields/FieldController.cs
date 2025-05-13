using System.Diagnostics;
using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared;
namespace TestBucket.Components.Shared.Fields;

internal class FieldController : TenantBaseService
{
    private readonly IProjectManager _projectManager;
    private readonly IFieldRepository _repository;
    private readonly IFieldDefinitionManager _definitionManager;
    private readonly IFieldManager _manager;

    public FieldController(
        AuthenticationStateProvider authenticationStateProvider,
        IProjectManager projectManager,
        IFieldRepository repository,
        IFieldManager manager,
        IFieldDefinitionManager definitionManager) :
        base(authenticationStateProvider)
    {
        _projectManager = projectManager;
        _repository = repository;
        _manager = manager;
        _definitionManager = definitionManager;
    }

    #region Test Case
    /// <summary>
    /// Saves the test case fields
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    public async Task SaveTestCaseFieldsAsync(IEnumerable<TestCaseField> fields)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        foreach(var field in fields)
        {
            await _manager.UpsertTestCaseFieldAsync(principal, field);
        }
    }
    public async Task UpsertTestCaseFieldAsync(TestCaseField field)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpsertTestCaseFieldAsync(principal, field);
    }

    /// <summary>
    /// Returns fields for a specific test case
    /// </summary>
    /// <param name="id"></param>
    /// <param name="fieldDefinitions"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<TestCaseField>> GetTestCaseFieldsAsync(long id, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetTestCaseFieldsAsync(principal, id, fieldDefinitions);
    }
    #endregion Test Case

    #region Issues
    /// <summary>
    /// Returns fields for a specific requirement
    /// </summary>
    /// <param name="id"></param>
    /// <param name="fieldDefinitions"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<IssueField>> GetIssueFieldsAsync(long id, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetIssueFieldsAsync(principal, id, fieldDefinitions);
    }

    public async Task UpsertIssueFieldAsync(IssueField field)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpsertIssueFieldAsync(principal, field);
    }
    #endregion Issues

    #region Requirements
    /// <summary>
    /// Returns fields for a specific requirement
    /// </summary>
    /// <param name="id"></param>
    /// <param name="fieldDefinitions"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<RequirementField>> GetRequirementFieldsAsync(long id, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetRequirementFieldsAsync(principal, id, fieldDefinitions);
    }

    public async Task UpsertRequirementFieldAsync(RequirementField field)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpsertRequirementFieldAsync(principal, field);
    }
    #endregion Requirements

    #region Test Run
    public async Task SaveTestRunFieldsAsync(IEnumerable<TestRunField> fields)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        foreach (var field in fields)
        {
            await _manager.UpsertTestRunFieldAsync(principal, field);
        }
    }

    public async Task UpsertTestRunFieldAsync(TestRunField field)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpsertTestRunFieldAsync(principal, field);
    }
    public async Task<IReadOnlyList<TestRunField>> GetTestRunFieldsAsync(long testRunId, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetTestRunFieldsAsync(principal, testRunId, fieldDefinitions);
    }

    #endregion Test Run

    #region Test Case Run
    public async Task UpsertTestCaseRunFieldAsync(TestCaseRunField field)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpsertTestCaseRunFieldAsync(principal, field);
    }
    public async Task SaveTestCaseRunFieldsAsync(IEnumerable<TestCaseRunField> fields)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Write);

        foreach (var field in fields)
        {
            await _manager.UpsertTestCaseRunFieldAsync(principal, field);
        }
    }

    public async Task<IReadOnlyList<TestCaseRunField>> GetTestCaseRunFieldsAsync(long testRunId, long testCaseRunId, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read);

        return await _manager.GetTestCaseRunFieldsAsync(principal, testRunId, testCaseRunId, fieldDefinitions);
    }

    #endregion Test Case Run

    #region Field Definitions

    public async Task<IReadOnlyList<string>> SearchOptionsAsync(FieldDefinition field, string text, int count, CancellationToken cancellationToken)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _definitionManager.SearchOptionsAsync(principal, field, text, count, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetOptionsAsync(FieldDefinition field)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _definitionManager.GetOptionsAsync(principal, field);
    }

    /// <summary>
    /// Searches for field definitions
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FieldDefinition>> SearchDefinitionsAsync(SearchFieldQuery query)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        var a = Stopwatch.GetTimestamp();

        var fieldDefinitions = await _definitionManager.SearchAsync(principal, query);

        var e1 = Stopwatch.GetElapsedTime(a);

        // Assign options from external data source
        //foreach (var fieldDefinition in fieldDefinitions)
        //{
        //    if (fieldDefinition.TestProjectId is not null)
        //    {
        //        string[]? options = await _projectManager.GetFieldOptionsAsync(principal, fieldDefinition.TestProjectId.Value, fieldDefinition.TraitType, default);
        //        if (options is not null)
        //        {
        //            fieldDefinition.Options = options.ToList();
        //        }
        //    }
        //}

        var e2 = Stopwatch.GetElapsedTime(a);

        return fieldDefinitions;
    }

    /// <summary>
    /// Returns all field definitions for the specified project and target
    /// </summary>
    /// <param name="testProjectId"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FieldDefinition>> GetDefinitionsAsync(long testProjectId, FieldTarget target)
    {
        return await SearchDefinitionsAsync(new SearchFieldQuery
        {
            ProjectId = testProjectId,
            Target = target
        });
    }

    public async Task AddAsync(FieldDefinition fieldDefinition)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        fieldDefinition.TenantId = principal.GetTenantIdOrThrow();
        RemoveOptionsIfNotSelection(fieldDefinition);

        await _definitionManager.AddAsync(principal, fieldDefinition);
    }

    private void RemoveOptionsIfNotSelection(FieldDefinition fieldDefinition)
    {
        if(fieldDefinition.Type is not (Domain.Fields.Models.FieldType.SingleSelection or Domain.Fields.Models.FieldType.MultiSelection))
        {
            fieldDefinition.Options = null;
        }
    }

    public async Task DeleteAsync(FieldDefinition fieldDefinition)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _definitionManager.DeleteAsync(principal, fieldDefinition);
    }

    public async Task UpdateAsync(FieldDefinition fieldDefinition)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        RemoveOptionsIfNotSelection(fieldDefinition);
        await _definitionManager.UpdateAsync(principal, fieldDefinition);
    }

    internal async Task UpdateTestCaseFieldsAsync(long[] testCaseIds, long? projectId, FieldValue[] fieldValues)
    {
        var fieldDefinitions = await SearchDefinitionsAsync(new SearchFieldQuery { ProjectId = projectId, Target = FieldTarget.TestCase });
        foreach (var testCaseId in testCaseIds)
        {
            var testCaseFields = (await GetTestCaseFieldsAsync(testCaseId, fieldDefinitions)).ToList();

            foreach (var field in fieldValues)
            {
                // Remove old tag
                testCaseFields.RemoveAll(x => x.FieldDefinitionId == field.FieldDefinitionId);

                // Add new tag
                var newField = new TestCaseField
                {
                    TestCaseId = testCaseId,
                    FieldDefinitionId = field.FieldDefinitionId,
                };
                field.CopyTo(newField);
                testCaseFields.Add(newField);
            }
            await SaveTestCaseFieldsAsync(testCaseFields);
        }
    }

    #endregion Field Definitions
}
