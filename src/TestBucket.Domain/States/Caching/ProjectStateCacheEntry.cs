using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Testing.States;

namespace TestBucket.Domain.States.Caching;

public class ProjectStateCacheEntry
{
    public required List<TestState> TestCaseStates { get; set; }
    public required List<TestState> TestCaseRunStates { get; set; }
    public required List<RequirementState> RequirementStates { get; set; }
    public required List<IssueState> IssueStates { get; set; }

}
