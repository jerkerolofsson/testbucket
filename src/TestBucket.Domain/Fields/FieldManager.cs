using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using Microsoft.Extensions.Caching.Memory;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields.Events;
using TestBucket.Domain.Fields.Helpers;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Projects.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Fields;
internal class FieldManager : IFieldManager
{
    private readonly IFieldRepository _repository;
    private readonly IMediator _mediator;
    private readonly IMemoryCache _memoryCache;

    private string GetCacheKey(TestRunField field) => "TestRunField:" + field.TenantId + field.TestRunId;

    public FieldManager(IFieldRepository repository, IMediator mediator, IMemoryCache memoryCache)
    {
        _repository = repository;
        _mediator = mediator;
        _memoryCache = memoryCache;
    }


    #region Issues

    /// <summary>
    /// Returns requirement fields, with default values for current field definitions
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="id"></param>
    /// <param name="fieldDefinitions"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<IssueField>> GetIssueFieldsAsync(ClaimsPrincipal principal, long id, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);

        var fields = (await _repository.GetIssueFieldsAsync(tenantId, id)).ToList();

        // Add missing fields
        foreach (var fieldDefinition in fieldDefinitions)
        {
            var field = fields.Where(x => x.FieldDefinitionId == fieldDefinition.Id).FirstOrDefault();
            if (field is null)
            {
                fields.Add(new IssueField
                {
                    FieldDefinition = fieldDefinition,
                    LocalIssueId = id,
                    FieldDefinitionId = fieldDefinition.Id
                });
            }
        }

        return fields;
    }
    public async Task UpsertIssueFieldAsync(ClaimsPrincipal principal, IssueField field)
    {
        var fieldDefinition = field.FieldDefinition ?? await _repository.GetDefinitionByIdAsync(field.FieldDefinitionId);
        ArgumentNullException.ThrowIfNull(fieldDefinition);
        principal.ThrowIfNoPermission(fieldDefinition);

        var oldFields = await _repository.GetIssueFieldsAsync(principal.GetTenantIdOrThrow(), field.LocalIssueId);
        var oldField = oldFields.FirstOrDefault(x => x.Id == field.Id);

        field.TenantId = principal.GetTenantIdOrThrow();
        await _repository.UpsertIssueFieldAsync(field);

        await _mediator.Publish(new IssueFieldChangedNotification(principal, field, oldField));
    }


    public async Task<IssueField?> GetIssueFieldAsync(ClaimsPrincipal principal, long projectId, long issueId, Predicate<IssueField> fieldPredicate, string value)
    {
        GetFieldsResponse response = await _mediator.Send(new GetFieldsRequest(principal, FieldTarget.Issue, projectId, issueId));
        var field = response.Fields.Cast<IssueField>().Where(x => fieldPredicate(x)).FirstOrDefault();
        return field;
    }

    public async Task<bool> SetIssueFieldAsync(ClaimsPrincipal principal, long projectId, long issueId, TraitType traitType, string value)
    {
        return await SetIssueFieldAsync(principal, projectId, issueId, (IssueField f) => f.FieldDefinition?.TraitType == traitType, value);
    }

    public async Task<bool> SetIssueFieldAsync(ClaimsPrincipal principal, long projectId, long issueId, Predicate<IssueField> fieldPredicate, string value)
    {
        GetFieldsResponse response = await _mediator.Send(new GetFieldsRequest(principal, FieldTarget.Issue, projectId, issueId));
        var field = response.Fields.Cast<IssueField>().Where(x => fieldPredicate(x)).FirstOrDefault();
        if (field is null)
        {
            return false;
        }

        if (field.FieldDefinition is not null)
        {
            FieldValueConverter.TryAssignValue(field.FieldDefinition, field, [value]);
            await UpsertIssueFieldAsync(principal, field);
            return true;
        }
        return false;
    }

    #endregion Issue


    #region Requirement
    public async Task UpsertRequirementFieldAsync(ClaimsPrincipal principal, RequirementField field)
    {
        var fieldDefinition = field.FieldDefinition ?? await _repository.GetDefinitionByIdAsync(field.FieldDefinitionId);
        ArgumentNullException.ThrowIfNull(fieldDefinition);
        principal.ThrowIfNoPermission(fieldDefinition);

        field.TenantId = principal.GetTenantIdOrThrow();
        await _repository.UpsertRequirementFieldAsync(field);

        await _mediator.Publish(new RequirementFieldChangedNotification(principal, field));
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

    #region Test Run


    public async Task<TestRunField?> GetTestRunFieldAsync(ClaimsPrincipal principal, long projectId, long testRunId, Predicate<TestRunField> fieldPredicate, string value)
    {
        GetFieldsResponse response = await _mediator.Send(new GetFieldsRequest(principal, FieldTarget.TestRun, projectId, testRunId));
        var field = response.Fields.Cast<TestRunField>().Where(x => fieldPredicate(x)).FirstOrDefault();
        return field;
    }

    public async Task<bool> SetTestRunFieldAsync(ClaimsPrincipal principal, long projectId, long testRunId, TraitType traitType, string value)
    {
        return await SetTestRunFieldAsync(principal, projectId, testRunId, (TestRunField f) => f.FieldDefinition?.TraitType == traitType, value);
    }

    public async Task<bool> SetTestRunFieldAsync(ClaimsPrincipal principal, long projectId, long testRunId, Predicate<TestRunField> fieldPredicate, string value)
    {
        GetFieldsResponse response = await _mediator.Send(new GetFieldsRequest(principal, FieldTarget.TestRun, projectId, testRunId));
        var field = response.Fields.Cast<TestRunField>().Where(x => fieldPredicate(x)).FirstOrDefault();
        if (field is null)
        {
            return false;
        }

        if (field.FieldDefinition is not null)
        {
            FieldValueConverter.TryAssignValue(field.FieldDefinition, field, [value]);
            await UpsertTestRunFieldAsync(principal, field);
            return true;
        }
        return false;
        
    }
    public async Task<IReadOnlyList<TestRunField>> GetTestRunFieldsAsync(ClaimsPrincipal principal, long testRunId, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read);

        string key = "TestRunField:" + tenantId + testRunId;
        return await _memoryCache.GetOrCreateAsync(key, async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromSeconds(15);

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
        }) ?? [];

    }
    #endregion

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

    public async Task UpsertTestRunFieldAsync(ClaimsPrincipal principal, TestRunField field)
    {
        _memoryCache.Remove(GetCacheKey(field)); // Clear cache for this field  


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

        var oldFields = await _repository.GetTestCaseFieldsAsync(principal.GetTenantIdOrThrow(), field.TestCaseId);
        var oldField = oldFields.FirstOrDefault(x => x.Id == field.Id);

        field.TenantId = principal.GetTenantIdOrThrow();
        await _repository.UpsertTestCaseFieldAsync(field);

        await _mediator.Publish(new TestCaseFieldChangedNotification(principal, field, oldField));
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
