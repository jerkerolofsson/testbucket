using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Testing.States;

namespace TestBucket.Domain.States.Caching;

/// <summary>
/// This is a cache for states of entities, such as issue state, requirement state, requirement type etc.
/// </summary>
public class ProjectStateCache
{
    private readonly ConcurrentDictionary<long, ProjectStateCacheEntry> _cache = [];


    public ProjectStateCacheEntry Update(TestProject project)
    {
        var entry = new ProjectStateCacheEntry
        {
            IssueStates = project.IssueStates ?? [],
            RequirementStates = project.RequirementStates ?? DefaultStates.GetDefaultRequirementStates(),
            TestStates = project.TestStates ?? DefaultStates.GetDefaultTestCaseRunStates(),
        };

        _cache[project.Id] = entry;

        return entry;
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
}
