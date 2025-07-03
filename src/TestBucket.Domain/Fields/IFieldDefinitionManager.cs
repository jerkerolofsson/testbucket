
using System.Security.Claims;

using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields;
public interface IFieldDefinitionManager
{
    /// <summary>
    /// Returns a list of options for the field. 
    /// This may come from an external data source, or defined on the field itself
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    Task<IReadOnlyList<GenericVisualEntity>> GetOptionsAsync(ClaimsPrincipal principal, FieldDefinition field);

    /// <summary>
    /// Adds a field 
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    Task AddAsync(ClaimsPrincipal principal, FieldDefinition field);

    /// <summary>
    /// Deletes a field
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="fieldDefinition"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Searches for fields
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<IReadOnlyList<FieldDefinition>> SearchAsync(ClaimsPrincipal principal, SearchFieldQuery query);

    /// <summary>
    /// Updates a field
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="fieldDefinition"></param>
    /// <returns></returns>
    Task UpdateAsync(ClaimsPrincipal principal, FieldDefinition fieldDefinition);

    /// <summary>
    /// Searches for options for a field. This is used to provide autocomplete functionality in the UI.
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="field"></param>
    /// <param name="text"></param>
    /// <param name="count"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<GenericVisualEntity>> SearchOptionsAsync(ClaimsPrincipal principal, FieldDefinition field, string text, int count, CancellationToken cancellationToken);
    void ClearTenantCache(string tenantId);
    Task ClearCacheAsync(string tenantId, FieldDataSourceType dataSourceType);
}