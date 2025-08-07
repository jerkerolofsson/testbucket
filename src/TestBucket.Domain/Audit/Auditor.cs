using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Audit.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Audit;
internal class Auditor : IAuditor
{
    private readonly IAuditRepository _repository;
    private readonly TimeProvider _timeProvider;

    public Auditor(IAuditRepository repository, TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<IReadOnlyList<AuditEntry>> GetEntriesAsync<T>(ClaimsPrincipal principal, long entityId)
    {
        string entityType = typeof(T).Name;
        if (entityType == typeof(TestCase).Name)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Read);
        }
        else if (entityType == typeof(Requirement).Name)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Read);
        }
        else
        {
            throw new NotImplementedException();
        }

        return await _repository.GetEntriesAsync(entityType, entityId);
    }

    public async Task LogAsync(string[] properties, long testProjectId, long entityId, object oldEntity, object newEntity)
    {
        var entry = CreateEntry(properties, testProjectId, entityId, oldEntity, newEntity);
        if (entry is not null)
        {
            await _repository.AddEntryAsync(entry);
        }
    }

    public AuditEntry? CreateEntry(string[] properties, long testProjectId, long entityId, object oldEntity, object newEntity)
    {
        if (properties is null || properties.Length == 0)
        {
            return null;
        }

        Dictionary<string, object?> oldProperties = GetProperties(properties, oldEntity);
        Dictionary<string, object?> newProperties = GetProperties(properties, newEntity);

        // Remove entries where the value is the same in both oldProperties and newProperties
        var keysToRemove = oldProperties.Keys
            .Where(key => newProperties.ContainsKey(key) && Equals(oldProperties[key], newProperties[key]))
            .ToList();
        foreach (var key in keysToRemove)
        {
            oldProperties.Remove(key);
            newProperties.Remove(key);
        }

        bool changed = oldProperties.Count != newProperties.Count || oldProperties.Count > 0;

        if (changed)
        {
            return new AuditEntry
            {
                Created = _timeProvider.GetUtcNow(),
                EntityType = newEntity.GetType().Name,
                NewValues = newProperties,
                OldValues = oldProperties,
                TestProjectId = testProjectId,
                EntityId = entityId
            };
        }

        return null;
    }

    private Dictionary<string, object?> GetProperties(string[] properties, object entity)
    {
        var result = new Dictionary<string, object?>();

        var type = entity.GetType();
        foreach (var propName in properties)
        {
            var propInfo = type.GetProperty(propName);
            if (propInfo != null)
            {
                var value = propInfo.GetValue(entity);
                result[propName] = value;
            }
        }
        return result;
    }
}
