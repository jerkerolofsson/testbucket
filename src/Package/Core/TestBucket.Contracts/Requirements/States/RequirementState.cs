using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.States;
using TestBucket.Contracts.Testing.States;

namespace TestBucket.Contracts.Requirements.States;

/// <summary>
/// User defined state of a requirement
/// </summary>
public class RequirementState : BaseState
{

    /// <summary>
    /// Mapped internal state
    /// </summary>
    public MappedRequirementState MappedState { get; set; }

    public override string ToString() => Name??"";

    public override bool Equals(object? obj)
    {
        if (obj is RequirementState state)
        {
            return (state.MappedState == MappedState && MappedState != MappedRequirementState.Other) ||
                (state.MappedState == MappedRequirementState.Other && state.Name == Name);
        }
        return false;
    }

    public override int GetHashCode() => MappedState.GetHashCode();

}
