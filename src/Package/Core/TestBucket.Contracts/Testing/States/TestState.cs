using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.States;
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
