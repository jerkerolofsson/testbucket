using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields;
internal class FieldManager : IFieldManager
{
    private readonly IFieldRepository _repository;

    public FieldManager(IFieldRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TestRunField>> GetTestRunFieldsAsync(ClaimsPrincipal principal, long testRunId, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        var fields = (await _repository.GetTestRunFieldsAsync(tenantId, testRunId)).ToList();

        // Add missing fields
        foreach (var fieldDefinition in fieldDefinitions)
        {
            var field = fields.Where(x => x.FieldDefinitionId == fieldDefinition.Id).FirstOrDefault();
            if (field is null)
            {
                fields.Add(new TestRunField
                {
                    FieldDefinition = fieldDefinition,
                    TestRunId = testRunId,
                    FieldDefinitionId = fieldDefinition.Id
                });
            }
        }

        return fields;
    }

    public async Task<IReadOnlyList<TestCaseRunField>> GetTestCaseRunFieldsAsync(ClaimsPrincipal principal, long testRunId, long testCaseRunId, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var tenantId = principal.GetTenantIdOrThrow();
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

    /// <summary>
    /// Returns test case fields, with default values for current field definitions
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="id"></param>
    /// <param name="fieldDefinitions"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<TestCaseField>> GetTestCaseFieldsAsync(ClaimsPrincipal principal, long id, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var tenantId = principal.GetTenantIdOrThrow();

        var fields = (await _repository.GetTestCaseFieldsAsync(tenantId, id)).ToList();

        // Add missing fields
        foreach (var fieldDefinition in fieldDefinitions)
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


    /// <summary>
    /// Returns requirement fields, with default values for current field definitions
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="id"></param>
    /// <param name="fieldDefinitions"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<RequirementField>> GetRequirementFieldsAsync(ClaimsPrincipal principal, long id, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var tenantId = principal.GetTenantIdOrThrow();

        var fields = (await _repository.GetRequirementFieldsAsync(tenantId, id)).ToList();

        // Add missing fields
        foreach (var fieldDefinition in fieldDefinitions)
        {
            var field = fields.Where(x => x.FieldDefinitionId == fieldDefinition.Id).FirstOrDefault();
            if (field is null)
            {
                fields.Add(new RequirementField
                {
                    FieldDefinition = fieldDefinition,
                    RequirementId = id,
                    FieldDefinitionId = fieldDefinition.Id
                });
            }
        }

        return fields;
    }

    public async Task SaveTestCaseFieldsAsync(ClaimsPrincipal principal, IEnumerable<TestCaseField> fields)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        foreach (var field in fields)
        {
            field.TenantId = tenantId;
        }
        await _repository.SaveTestCaseFieldsAsync(fields);
    }

    public async Task SaveTestCaseRunFieldsAsync(ClaimsPrincipal principal, IEnumerable<TestCaseRunField> fields)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        foreach (var field in fields)
        {
            field.TenantId = tenantId;
        }
        await _repository.SaveTestCaseRunFieldsAsync(fields);
    }

    public async Task SaveTestRunFieldsAsync(ClaimsPrincipal principal, IEnumerable<TestRunField> fields)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        foreach (var field in fields)
        {
            field.TenantId = tenantId;
        }
        await _repository.SaveTestRunFieldsAsync(fields);
    }
}
