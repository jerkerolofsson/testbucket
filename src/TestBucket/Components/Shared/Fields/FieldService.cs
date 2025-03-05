

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

    public async Task<IReadOnlyList<FieldDefinition>> SearchDefinitionsAsync(SearchQuery query)
    {
        var tenantId = await GetTenantIdAsync();
        return await _repository.SearchAsync(tenantId, query);
    }

    public async Task AddAsync(FieldDefinition fieldDefinition)
    {
        fieldDefinition.TenantId = await GetTenantIdAsync();
        await _repository.AddAsync(fieldDefinition);    
    }
}
