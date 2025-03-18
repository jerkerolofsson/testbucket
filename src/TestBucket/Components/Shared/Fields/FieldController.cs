

using TestBucket.Domain.Fields;
using TestBucket.Domain.Fields.Specifications;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Tenants.Models;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TestBucket.Components.Shared.Fields;

internal class FieldController : TenantBaseService
{
    private readonly IFieldRepository _repository;
    private readonly IFieldDefinitionManager _manager;

    public FieldController(
        AuthenticationStateProvider authenticationStateProvider,
        IFieldRepository repository,
        IFieldDefinitionManager manager) :
        base(authenticationStateProvider)
    {
        _repository = repository;
        _manager = manager;
    }

    #region Test Case
    public async Task SaveTestCaseFieldsAsync(IEnumerable<TestCaseField> fields)
    {
        var tenantId = await GetTenantIdAsync();
        foreach (var field in fields)
        {
            field.TenantId = tenantId;
        }
        await _repository.SaveTestCaseFieldsAsync(fields);
    }
    public async Task<IReadOnlyList<TestCaseField>> GetTestCaseFieldsAsync(long id, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var tenantId = await GetTenantIdAsync();
        var fields = (await _repository.GetTestCaseFieldsAsync(tenantId, id)).ToList();

        // Add missing fields
        foreach(var fieldDefinition in fieldDefinitions)
        {
            var field = fields.Where(x => x.FieldDefinitionId == fieldDefinition.Id).FirstOrDefault();
            if (field is null)
            {
                fields.Add(new TestCaseField 
                { 
                    FieldDefinition = fieldDefinition,
                    TestCaseId = id,
                    FieldDefinitionId = fieldDefinition.Id 
                });
            }
        }

        return fields;
    }

    #endregion Test Case

    #region Test Case Run
    public async Task SaveTestCaseRunFieldsAsync(IEnumerable<TestCaseRunField> fields)
    {
        var tenantId = await GetTenantIdAsync();
        foreach (var field in fields)
        {
            field.TenantId = tenantId;
        }
        await _repository.SaveTestCaseRunFieldsAsync(fields);
    }
    public async Task<IReadOnlyList<TestCaseRunField>> GetTestCaseRunFieldsAsync(long testRunId, long testCaseRunId, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var tenantId = await GetTenantIdAsync();
        var fields = (await _repository.GetTestCaseRunFieldsAsync(tenantId, testCaseRunId)).ToList();

        // Add missing fields
        foreach (var fieldDefinition in fieldDefinitions)
        {
            var field = fields.Where(x => x.FieldDefinitionId == fieldDefinition.Id).FirstOrDefault();
            if (field is null)
            {
                fields.Add(new TestCaseRunField
                {
                    FieldDefinition = fieldDefinition,
                    TestRunId = testRunId,
                    TestCaseRunId = testCaseRunId,
                    FieldDefinitionId = fieldDefinition.Id
                });
            }
        }

        return fields;
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
        var tenantId = await GetTenantIdAsync();
        var specifications = FieldSpecificationBuilder.From(query);
        var principal = await GetUserClaimsPrincipalAsync();

        return await _manager.SearchAsync(principal, specifications);
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
        fieldDefinition.TenantId = await GetTenantIdAsync();
        RemoveOptionsIfNotSelection(fieldDefinition);
        await _repository.AddAsync(fieldDefinition);
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
        var tenantId = await GetTenantIdAsync();
        if (tenantId != fieldDefinition.TenantId)
        {
            throw new InvalidOperationException("Tenant ID mismatch");
        }
        await _repository.DeleteAsync(fieldDefinition);
    }

    public async Task UpdateAsync(FieldDefinition fieldDefinition)
    {
        var tenantId = await GetTenantIdAsync();
        if(tenantId != fieldDefinition.TenantId)
        {
            throw new InvalidOperationException("Tenant ID mismatch");
        }
        RemoveOptionsIfNotSelection(fieldDefinition);
        await _repository.UpdateAsync(fieldDefinition);
    }
    #endregion Field Definitions
}
