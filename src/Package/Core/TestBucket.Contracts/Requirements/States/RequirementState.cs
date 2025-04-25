namespace TestBucket.Contracts.Requirements.States;

/// <summary>
/// User defined state of a requirement
/// </summary>
public class RequirementState
{
    /// <summary>
    /// Name of the state (this can be anything, defined by external systems)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Mapped internal state
    /// </summary>
    public MappedRequirementState MappedState { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is RequirementState state)
        {
            return state.MappedState == MappedState ||
                (state.MappedState == MappedRequirementState.Other && state.Name == Name);
        }
        return false;
    }

    public override int GetHashCode() => MappedState.GetHashCode();

}
