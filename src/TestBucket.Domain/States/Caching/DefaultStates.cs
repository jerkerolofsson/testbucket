using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Testing.States;

namespace TestBucket.Domain.States.Caching;
internal class DefaultStates
{
    /// <summary>
    /// Returns default states if no states has been configured for the project
    /// </summary>
    /// <returns></returns>
    internal static TestState[] GetDefaultTestCaseRunStates()
    {
        return [
                new() { Name = TestStates.NotStarted, IsInitial = true, MappedState = MappedTestState.NotStarted },
                new() { Name = TestStates.Assigned, MappedState = MappedTestState.Assigned },
                new() { Name = TestStates.Ongoing, MappedState = MappedTestState.Ongoing },
                new() { Name = TestStates.Completed, MappedState = MappedTestState.Completed, IsFinal = true },
            ];
    }

    /// <summary>
    /// Returns default requirement states
    /// </summary>
    /// <returns></returns>
    internal static RequirementState[] GetDefaultRequirementStates()
    {
        return [
                new() { Name = RequirementStates.Draft, MappedState = MappedRequirementState.Draft },
                new() { Name = RequirementStates.Accepted, MappedState = MappedRequirementState.Accepted },
                new() { Name = RequirementStates.Assigned, MappedState = MappedRequirementState.Assigned },
                new() { Name = RequirementStates.InProgress, MappedState = MappedRequirementState.InProgress },
                new() { Name = RequirementStates.Delivered, MappedState = MappedRequirementState.Delivered },
                new() { Name = RequirementStates.Completed, MappedState = MappedRequirementState.Completed },
                new() { Name = RequirementStates.Canceled, MappedState = MappedRequirementState.Canceled },
            ];
    }


    /// <summary>
    /// Returns default issue states
    /// </summary>
    /// <returns></returns>
    internal static IssueState[] GetDefaultIssueStates()
    {
        return [
                new() { Name = IssueStates.Open, MappedState = MappedIssueState.Open },
                new() { Name = IssueStates.Accepted, MappedState = MappedIssueState.Accepted },
                new() { Name = IssueStates.Assigned, MappedState = MappedIssueState.Assigned },
                new() { Name = IssueStates.Triaged, MappedState = MappedIssueState.Triaged },
                new() { Name = IssueStates.InProgress, MappedState = MappedIssueState.InProgress },
                new() { Name = IssueStates.Reviewed, MappedState = MappedIssueState.Reviewed },
                new() { Name = IssueStates.Closed, MappedState = MappedIssueState.Closed },
            ];
    }

}
