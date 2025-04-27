
using System.Security.Claims;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields;
public interface IFieldDefinitionManager
{
    Task AddAsync(ClaimsPrincipal principal, FieldDefinition field);
    Task DeleteAsync(ClaimsPrincipal principal, FieldDefinition fieldDefinition);
    
    /// <summary>
    /// Returns all fields for a project
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testProjectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<FieldDefinition>> GetDefinitionsAsync(ClaimsPrincipal principal, long? testProjectId);

    /// <summary>
    /// Returns project fields that match a specific target entity
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testProjectId"></param>
    /// <param name="target">The target entity</param>
    /// <returns></returns>
    Task<IReadOnlyList<FieldDefinition>> GetDefinitionsAsync(ClaimsPrincipal principal, long? testProjectId, FieldTarget target);
    Task<IReadOnlyList<FieldDefinition>> SearchAsync(ClaimsPrincipal principal, SearchFieldQuery query);
    Task UpdateAsync(ClaimsPrincipal principal, FieldDefinition fieldDefinition);
    Task UpsertTestCaseFieldAsync(ClaimsPrincipal principal, TestCaseField field);
    Task UpsertTestRunFieldAsync(ClaimsPrincipal principal, TestRunField field);
}