using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// Returns default reqirement states
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

}
