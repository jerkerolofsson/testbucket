
using System.Security.Claims;

using TestBucket.Domain.Fields.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields;
public interface IFieldDefinitionManager
{
    Task AddAsync(ClaimsPrincipal principal, FieldDefinition field);
    Task DeleteAsync(ClaimsPrincipal principal, FieldDefinition fieldDefinition);
    Task<IReadOnlyList<FieldDefinition>> GetDefinitionsAsync(ClaimsPrincipal principal, long? testProjectId, FieldTarget target);
    Task<IReadOnlyList<FieldDefinition>> SearchAsync(ClaimsPrincipal principal, SearchFieldQuery query);
    Task UpdateAsync(ClaimsPrincipal principal, FieldDefinition fieldDefinition);
    Task UpsertTestCaseFieldsAsync(ClaimsPrincipal principal, TestCaseField field);
}