namespace TestBucket.Contracts.Issues.Types;
public class IssueType
{
    /// <summary>
    /// Name of the state (this can be anything, defined by external systems)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Mapped internal type
    /// </summary>
    public MappedIssueType MappedType { get; set; }

    /// <summary>
    /// Accent color for the type
    /// </summary>
    public string? Color { get; set; }

    public override string ToString() => Name??"";

    public override bool Equals(object? obj)
    {
        if (obj is IssueType state)
        {
            return state.MappedType == MappedType ||
                (state.MappedType == MappedIssueType.Other && state.Name == Name);
        }
        return false;
    }

    public override int GetHashCode() => MappedType.GetHashCode();

}
