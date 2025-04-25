namespace TestBucket.Contracts.Issues.States;
public class IssueState
{
    /// <summary>
    /// Name of the state (this can be anything, defined by external systems)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Mapped internal state
    /// </summary>
    public MappedIssueState MappedState { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is IssueState state)
        {
            return state.MappedState == MappedState ||
                (state.MappedState == MappedIssueState.Other && state.Name == Name);
        }
        return false;
    }

    public override int GetHashCode() => MappedState.GetHashCode();

}
