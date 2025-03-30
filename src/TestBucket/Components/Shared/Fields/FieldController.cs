
using TestBucket.Components.Tests.Controls;
using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Fields.Specifications;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Shared;
using TestBucket.Domain.Testing.Models;
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
    public async Task SaveTestCaseFieldsAsync(IEnumerable<TestCaseField> fields)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.SaveTestCaseFieldsAsync(principal, fields);

    }
    public async Task<IReadOnlyList<TestCaseField>> GetTestCaseFieldsAsync(long id, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetTestCaseFieldsAsync(principal, id, fieldDefinitions);
    }

    #endregion Test Case

    #region Test Run
    public async Task SaveTestRunFieldsAsync(IEnumerable<TestRunField> fields)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.SaveTestRunFieldsAsync(principal, fields);
    }
    public async Task<IReadOnlyList<TestRunField>> GetTestRunFieldsAsync(long testRunId, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetTestRunFieldsAsync(principal, testRunId, fieldDefinitions);
    }

    #endregion Test Run

    #region Test Case Run
    public async Task SaveTestCaseRunFieldsAsync(IEnumerable<TestCaseRunField> fields)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.SaveTestCaseRunFieldsAsync(principal, fields);
    }

    public async Task<IReadOnlyList<TestCaseRunField>> GetTestCaseRunFieldsAsync(long testRunId, long testCaseRunId, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetTestCaseRunFieldsAsync(principal, testRunId, testCaseRunId, fieldDefinitions);
    }

    #endregion Test Case Run

    #region Field Definitions

    /// <summary>
    /// Searches for field definitions
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FieldDefinition>> SearchDefinitionsAsync(SearchFieldQuery query)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        var fieldDefinitions = await _definitionManager.SearchAsync(principal, query);

        // Assign options from external data source
        foreach (var fieldDefinition in fieldDefinitions)
        {
            if (fieldDefinition.TestProjectId is not null)
            {
                string[]? options = await _projectManager.GetFieldOptionsAsync(principal, fieldDefinition.TestProjectId.Value, fieldDefinition.TraitType, default);
                if (options is not null)
                {
                    fieldDefinition.Options = options.ToList();
                }
            }
        }

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
    #endregion Field Definitions
}
