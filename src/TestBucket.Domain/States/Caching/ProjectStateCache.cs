using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.States.Models;

namespace TestBucket.Domain.States.Caching;

/// <summary>
/// This is a cache for states of entities, such as issue state, requirement state, requirement type etc.
/// </summary>
public class ProjectStateCache
{
    private readonly ConcurrentDictionary<long, ProjectStateCacheEntry> _cache = [];


    /// <summary>
    /// This creates a consolidated list of states for a project, based on the tenant, team and project specific definitions.
    /// </summary>
    /// <param name="projectDefinition"></param>
    /// <param name="teamDefinition"></param>
    /// <param name="tenantDefinition"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public ProjectStateCacheEntry Update(StateDefinition? projectDefinition, StateDefinition? teamDefinition, StateDefinition? tenantDefinition, long projectId)
    {
        if(projectDefinition is null && teamDefinition is null && tenantDefinition is null)
        {
            return new ProjectStateCacheEntry
            {
                IssueStates = DefaultStates.GetDefaultIssueStates().ToList(),
                RequirementStates = DefaultStates.GetDefaultRequirementStates().ToList(),
                TestCaseRunStates = DefaultStates.GetDefaultTestCaseRunStates().ToList(),
                TestCaseStates = DefaultStates.GetDefaultTestCaseStates().ToList()
            };
        }

        var entry = new ProjectStateCacheEntry
        {
            IssueStates = [],
            RequirementStates = [],
            TestCaseRunStates = [],
            TestCaseStates = []
        };

        if (tenantDefinition is not null)
        {
            UpdatedEntry(tenantDefinition, entry);
        }
        if (teamDefinition is not null)
        {
            UpdatedEntry(teamDefinition, entry);
        }
        if (projectDefinition is not null)
        {
            UpdatedEntry(projectDefinition, entry);
        }

        _cache[projectId] = entry;

        return entry;
    }

    private static void UpdatedEntry(StateDefinition definition, ProjectStateCacheEntry mergedCachedEntryForProject)
    {
        foreach (var state in definition.IssueStates)
        {
            var existing = mergedCachedEntryForProject.IssueStates.FirstOrDefault(s => s.Name?.Equals(state.Name, StringComparison.OrdinalIgnoreCase) == true);
            if (existing is null)
            {
                mergedCachedEntryForProject.IssueStates.Add(state);
            }
            else
            {
                // Override with project specific state
                mergedCachedEntryForProject.IssueStates.Remove(existing);
                mergedCachedEntryForProject.IssueStates.Add(state);
            }
        }

        foreach (var state in definition.TestCaseRunStates)
        {
            var existing = mergedCachedEntryForProject.TestCaseRunStates.FirstOrDefault(s => s.Name?.Equals(state.Name, StringComparison.OrdinalIgnoreCase) == true);
            if (existing is null)
            {
                mergedCachedEntryForProject.TestCaseRunStates.Add(state);
            }
            else
            {
                // Override with project specific state
                mergedCachedEntryForProject.TestCaseRunStates.Remove(existing);
                mergedCachedEntryForProject.TestCaseRunStates.Add(state);
            }
        }
        foreach (var state in definition.TestCaseStates)
        {
            var existing = mergedCachedEntryForProject.TestCaseStates.FirstOrDefault(s => s.Name?.Equals(state.Name, StringComparison.OrdinalIgnoreCase) == true);
            if (existing is null)
            {
                mergedCachedEntryForProject.TestCaseStates.Add(state);
            }
            else
            {
                // Override with project specific state
                mergedCachedEntryForProject.TestCaseStates.Remove(existing);
                mergedCachedEntryForProject.TestCaseStates.Add(state);
            }
        }
        foreach (var state in definition.RequirementStates)
        {
            var existing = mergedCachedEntryForProject.RequirementStates.FirstOrDefault(s => s.Name?.Equals(state.Name, StringComparison.OrdinalIgnoreCase) == true);
            if (existing is null)
            {
                mergedCachedEntryForProject.RequirementStates.Add(state);
            }
            else
            {
                // Override with project specific state
                mergedCachedEntryForProject.RequirementStates.Remove(existing);
                mergedCachedEntryForProject.RequirementStates.Add(state);
            }
        }
    }

    public bool TryGetValue(long projectId, [NotNullWhen(true)] out ProjectStateCacheEntry? entry)
    {
        entry = null;

        if(_cache.TryGetValue(projectId, out var value))
        {
            entry = value;
            return true;
        }

        return false;
    }

    internal void Clear()
    {
        _cache.Clear();
    }
}
