using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.States.Models;

namespace TestBucket.Domain.States.Caching;

/// <summary>
/// Contains the default states for entities
/// </summary>
public class DefaultStates
{
    /// <summary>
    /// Returns default states if no states has been configured for the project
    /// </summary>
    /// <returns></returns>
    internal static IReadOnlyList<TestState> GetDefaultTestCaseStates()
    {
        return [
                new() { Name = TestCaseStates.Draft, MappedState = MappedTestState.Ongoing, IsInitial = true },
                new() { Name = TestCaseStates.Review, MappedState = MappedTestState.Review },
                new() { Name = TestCaseStates.Completed, MappedState = MappedTestState.Completed, IsFinal = true },
            ];
    }

    /// <summary>
    /// Returns default states if no states has been configured for the project
    /// </summary>
    /// <returns></returns>
    internal static IReadOnlyList<TestState> GetDefaultTestCaseRunStates()
    {
        return [
                new() { Name = TestCaseRunStates.NotStarted, MappedState = MappedTestState.NotStarted, IsInitial = true },
                new() { Name = TestCaseRunStates.Assigned, MappedState = MappedTestState.Assigned },
                new() { Name = TestCaseRunStates.Ongoing, MappedState = MappedTestState.Ongoing },
                new() { Name = TestCaseRunStates.Completed, MappedState = MappedTestState.Completed, IsFinal = true },
            ];
    }

    /// <summary>
    /// Returns default requirement states
    /// </summary>
    /// <returns></returns>
    internal static IReadOnlyList<RequirementState> GetDefaultRequirementStates()
    {
        return [
                new() { Name = RequirementStates.Draft, MappedState = MappedRequirementState.Draft, IsInitial = true },
                new() { Name = RequirementStates.Accepted, MappedState = MappedRequirementState.Accepted },
                new() { Name = RequirementStates.Assigned, MappedState = MappedRequirementState.Assigned },
                new() { Name = RequirementStates.InProgress, MappedState = MappedRequirementState.InProgress },
                new() { Name = RequirementStates.Delivered, MappedState = MappedRequirementState.Delivered },
                new() { Name = RequirementStates.Completed, MappedState = MappedRequirementState.Completed, IsFinal = true },
                new() { Name = RequirementStates.Canceled, MappedState = MappedRequirementState.Canceled },
            ];
    }


    /// <summary>
    /// Returns default issue states
    /// </summary>
    /// <returns></returns>
    internal static IReadOnlyList<IssueState> GetDefaultIssueStates()
    {
        return [
                new() { Name = IssueStates.Open, MappedState = MappedIssueState.Open, IsInitial = true },
                new() { Name = IssueStates.Triage, MappedState = MappedIssueState.Triage },
                new() { Name = IssueStates.Triaged, MappedState = MappedIssueState.Triaged },
                new() { Name = IssueStates.Accepted, MappedState = MappedIssueState.Accepted },
                new() { Name = IssueStates.Assigned, MappedState = MappedIssueState.Assigned },
                new() { Name = IssueStates.InProgress, MappedState = MappedIssueState.InProgress },
                new() { Name = IssueStates.Reviewed, MappedState = MappedIssueState.Reviewed },
                new() { Name = IssueStates.Closed, MappedState = MappedIssueState.Closed, IsFinal = true },
            ];
    }

    public static StateDefinition CreateDefaultProjectDefinition(string tenantId, long projectId)
    {
        return new StateDefinition
        {
            TenantId = tenantId,
            TeamId = null,
            TestProjectId = projectId,
            IssueStates = [],
            RequirementStates = [],
            TestCaseRunStates = [],
            TestCaseStates = []
        };
    }
    public static StateDefinition CreateDefaultTeamDefinition(string tenantId, long teamId)
    {
        return new StateDefinition
        {
            TenantId = tenantId,
            TeamId = teamId,
            TestProjectId = null,
            IssueStates = [],
            RequirementStates = [],
            TestCaseRunStates = [],
            TestCaseStates = []
        };
    }
    public static StateDefinition CreateDefaultTenantDefinition(string tenantId)
    {
        return new StateDefinition
        {
            TenantId = tenantId,
            TeamId = null,
            TestProjectId = null,
            IssueStates = DefaultStates.GetDefaultIssueStates().ToList(),
            RequirementStates = DefaultStates.GetDefaultRequirementStates().ToList(),
            TestCaseRunStates = DefaultStates.GetDefaultTestCaseRunStates().ToList(),
            TestCaseStates = DefaultStates.GetDefaultTestCaseStates().ToList()
        };
    }

}
