

using TestBucket.Domain.Fields;
using TestBucket.Domain.Tenants.Models;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TestBucket.Components.Shared.Fields;

internal class FieldService : TenantBaseService
{
    private readonly IFieldRepository _repository;

    public FieldService(
        AuthenticationStateProvider authenticationStateProvider, 
        IFieldRepository repository) : 
        base(authenticationStateProvider)
    {
        _repository = repository;
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
    public async Task<IReadOnlyList<FieldDefinition>> SearchDefinitionsAsync(FieldTarget? target, SearchQuery query)
    {
        var tenantId = await GetTenantIdAsync();
        return await _repository.SearchAsync(tenantId, target, query);
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
