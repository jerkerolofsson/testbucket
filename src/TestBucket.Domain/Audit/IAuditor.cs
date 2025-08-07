using TestBucket.Domain.Audit.Models;

namespace TestBucket.Domain.Audit;
/// <summary>
/// Provides methods for creating and logging audit entries for entity changes.
/// </summary>
public interface IAuditor
{
    /// <summary>
    /// Creates an audit entry for the specified entity changes.
    /// </summary>
    /// <param name="properties">The properties that were changed.</param>
    /// <param name="testProjectId">The ID of the test project.</param>
    /// <param name="entityId">The ID of the entity.</param>
    /// <param name="oldEntity">The original entity before changes.</param>
    /// <param name="newEntity">The updated entity after changes.</param>
    /// <returns>An <see cref="AuditEntry"/> representing the change, or null if no entry is created.</returns>
    AuditEntry? CreateEntry(string[] properties, long testProjectId, long entityId, object oldEntity, object newEntity);
    Task<IReadOnlyList<AuditEntry>> GetEntriesAsync<T>(ClaimsPrincipal principal, long entityId);

    /// <summary>
    /// Asynchronously logs an audit entry for the specified entity changes.
    /// </summary>
    /// <param name="properties">The properties that were changed.</param>
    /// <param name="testProjectId">The ID of the test project.</param>
    /// <param name="entityId">The ID of the entity.</param>
    /// <param name="oldEntity">The original entity before changes.</param>
    /// <param name="newEntity">The updated entity after changes.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task LogAsync(string[] properties, long testProjectId, long entityId, object oldEntity, object newEntity);
}