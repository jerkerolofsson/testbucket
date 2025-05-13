using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Testing.States;

namespace TestBucket.Domain.States.Caching;

public class ProjectStateCacheEntry
{
    public required IReadOnlyList<TestState> TestStates { get; set; }
    public required IReadOnlyList<RequirementState> RequirementStates { get; set; }
    public required IReadOnlyList<IssueState> IssueStates { get; set; }

}
