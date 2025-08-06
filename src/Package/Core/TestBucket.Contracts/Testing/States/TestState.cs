using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.States;

namespace TestBucket.Contracts.Testing.States;

/// <summary>
/// User defined state of a test case
/// </summary>
public class TestState : BaseState
{
  
    /// <summary>
    /// Known state
    /// </summary>
    public MappedTestState MappedState { get; set; } = MappedTestState.NotStarted;

    public override string? GetMappedState()
    {
        return MappedState.ToString();
    }

    public override bool SetMappedState(string name)
    {
        if(Enum.TryParse<MappedTestState>(name, true, out var state))
        {
            MappedState = state;
            return true;
        }
        MappedState = MappedTestState.Other;
        return false;
    }

    public override string[] GetMappedStates()
    {
        return 
            [
                TestCaseStates.Draft,
                TestCaseStates.Review,
                TestCaseStates.Ongoing,
                TestCaseStates.Completed,
                TestCaseRunStates.Assigned,
                TestCaseRunStates.NotStarted,
            ];
    }

    public override bool Equals(object? obj)
    {
        if(obj is TestState state)
        {
            return (state.MappedState == MappedState && MappedState != MappedTestState.Other) ||
                (state.MappedState == MappedTestState.Other && state.Name == Name);
        }
        return false;
    }

    public override int GetHashCode() => MappedState.GetHashCode();

}
