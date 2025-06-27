using Mediator;

namespace TestBucket.Domain.Export.Events;

/// <summary>
/// Import for the specified tenantid and projectid. 
/// If the imported resource contains tenant id and project id, it will be used instead of these.
/// </summary>
/// <param name="TenantId"></param>
/// <param name="ProjectId"></param>
public record class ImportNotification(string? TenantId, string? ProjectId) : INotification
{
}
