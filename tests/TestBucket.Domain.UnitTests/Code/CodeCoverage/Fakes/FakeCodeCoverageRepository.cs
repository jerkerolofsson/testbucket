using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TestBucket.Domain.Code.CodeCoverage;
using TestBucket.Domain.Code.CodeCoverage.Models;

namespace TestBucket.Domain.UnitTests.Code.CodeCoverage.Fakes;

internal class FakeCodeCoverageRepository : ICodeCoverageRepository
{
    // In-memory store for groups, keyed by (tenantId, projectId, groupType, groupName)
    private readonly ConcurrentDictionary<(string TenantId, long ProjectId, CodeCoverageGroupType GroupType, string GroupName), CodeCoverageGroup> _groups
        = new();

    public Task AddGroupAsync(CodeCoverageGroup group)
    {
        if (group == null)
            throw new ArgumentNullException(nameof(group));

        var key = (group.TenantId ?? string.Empty, group.TestProjectId ?? 0, group.Group, group.Name);
        if (!_groups.TryAdd(key, group))
            throw new InvalidOperationException("Group already exists.");

        return Task.CompletedTask;
    }

    public Task<CodeCoverageGroup?> GetGroupAsync(string tenantId, long projectId, CodeCoverageGroupType groupType, string groupName)
    {
        var key = (tenantId ?? string.Empty, projectId, groupType, groupName);
        _groups.TryGetValue(key, out var group);
        return Task.FromResult(group);
    }

    public Task UpdateGroupAsync(CodeCoverageGroup group)
    {
        if (group == null)
            throw new ArgumentNullException(nameof(group));

        var key = (group.TenantId ?? string.Empty, group.TestProjectId ?? 0, group.Group, group.Name);
        if (!_groups.ContainsKey(key))
            throw new KeyNotFoundException("Group not found.");

        _groups[key] = group;
        return Task.CompletedTask;
    }
}