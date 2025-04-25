using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Testing.States;

/// <summary>
/// User defined state of a test case
/// </summary>
public class TestState
{
    /// <summary>
    /// Name of the state
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Known state
    /// </summary>
    public MappedTestState MappedState { get; set; } = MappedTestState.NotStarted;

    /// <summary>
    /// Flag that indicates that this state is the initial state (e.g. Not Started)
    /// </summary>
    public bool IsInitial { get; set; }

    /// <summary>
    /// Flag that indicates that this state is the final state (e.g. Completed)
    /// </summary>
    public bool IsFinal { get; set; }

    public override bool Equals(object? obj)
    {
        if(obj is TestState state)
        {
            return state.MappedState == MappedState ||
                (state.MappedState == MappedTestState.Other && state.Name == Name);
        }
        return false;
    }

    public override int GetHashCode() => MappedState.GetHashCode();

}
