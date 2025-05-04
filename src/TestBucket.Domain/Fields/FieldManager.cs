using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Fields.Events;
using TestBucket.Domain.Fields.Helpers;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields;
internal class FieldManager : IFieldManager
{
    private readonly IFieldRepository _repository;
    private readonly IMediator _mediator;

    public FieldManager(IFieldRepository repository, IMediator mediator)
    {
        _repository = repository;
        _mediator = mediator;
    }

    #region Requirement

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
        principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);

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

    #endregion Requirement

    public async Task<IReadOnlyList<TestRunField>> GetTestRunFieldsAsync(ClaimsPrincipal principal, long testRunId, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read);

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
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read);


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
        principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Read);

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


    public async Task UpsertRequirementFieldAsync(ClaimsPrincipal principal, RequirementField field)
    {
        var fieldDefinition = field.FieldDefinition ?? await _repository.GetDefinitionByIdAsync(field.FieldDefinitionId);
        principal.ThrowIfNoPermission(fieldDefinition);

        field.TenantId = principal.GetTenantIdOrThrow();
        await _repository.UpsertRequirementFieldAsync(field);

        await _mediator.Publish(new RequirementFieldChangedNotification(principal, field));
    }
    public async Task UpsertTestRunFieldAsync(ClaimsPrincipal principal, TestRunField field)
    {
        var fieldDefinition = field.FieldDefinition ?? await _repository.GetDefinitionByIdAsync(field.FieldDefinitionId);
        principal.ThrowIfNoPermission(fieldDefinition);

        field.TenantId = principal.GetTenantIdOrThrow();
        await _repository.UpsertTestRunFieldAsync(field);

        await _mediator.Publish(new TestRunFieldChangedNotification(principal, field));
    }
    public async Task UpsertTestCaseFieldAsync(ClaimsPrincipal principal, TestCaseField field)
    {
        var fieldDefinition = field.FieldDefinition ?? await _repository.GetDefinitionByIdAsync(field.FieldDefinitionId);
        principal.ThrowIfNoPermission(fieldDefinition);

        field.TenantId = principal.GetTenantIdOrThrow();
        await _repository.UpsertTestCaseFieldAsync(field);

        await _mediator.Publish(new TestCaseFieldChangedNotification(principal, field));
    }
    public async Task UpsertTestCaseRunFieldAsync(ClaimsPrincipal principal, TestCaseRunField field)
    {
        var fieldDefinition = field.FieldDefinition ?? await _repository.GetDefinitionByIdAsync(field.FieldDefinitionId);
        principal.ThrowIfNoPermission(fieldDefinition);

        field.TenantId = principal.GetTenantIdOrThrow();
        await _repository.UpsertTestCaseRunFieldAsync(field);

        await _mediator.Publish(new TestCaseRunFieldChangedNotification(principal, field));
    }
}
