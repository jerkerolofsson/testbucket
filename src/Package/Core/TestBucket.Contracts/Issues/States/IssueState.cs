using TestBucket.Contracts.States;

namespace TestBucket.Contracts.Issues.States;
public class IssueState : BaseState
{
    /// <summary>
    /// Mapped internal state
    /// </summary>
    public MappedIssueState MappedState { get; set; }

    public override string ToString() => Name??"";

    public IssueState()
    {

    }

    public IssueState(string name, MappedIssueState mappedState)
    {
        Name = name;
        MappedState = mappedState;
    }

    public override bool Equals(object? obj)
    {
        if (obj is IssueState state)
        {
            return (state.MappedState == MappedState && MappedState != MappedIssueState.Other) ||
                (state.MappedState == MappedIssueState.Other && state.Name == Name);
        }
        return false;
    }

    public override int GetHashCode() => MappedState.GetHashCode();

}
