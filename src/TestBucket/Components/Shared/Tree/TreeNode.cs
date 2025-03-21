using System.Diagnostics.CodeAnalysis;

namespace TestBucket.Components.Shared.Tree;

public class TreeNode<T> : IEquatable<TreeNode<T>>
{
    public virtual string? Text { get; set; }

    public virtual string? Icon { get; set; }

    public T? Value { get; init; }

    public virtual bool Expanded { get; set; }

    public virtual bool Expandable { get; set; } = true;

    public virtual bool Selected { get; set; }

    public virtual bool Visible { get; set; } = true;

    public virtual IReadOnlyCollection<TreeNode<T>>? Children { get; set; }

    [MemberNotNullWhen(true, "Children")]
    public virtual bool HasChildren
    {
        [MemberNotNullWhen(true, "Children")]
        get
        {
            if (Children != null)
            {
                return Children.Count > 0;
            }

            return false;
        }
    }

    public TreeNode()
        : this(default(T))
    {
    }

    protected TreeNode(T? value)
    {
        Value = value;
    }

    public virtual bool Equals(TreeNode<T>? other)
    {
        if (other == null)
        {
            return false;
        }

        if (this == other)
        {
            return true;
        }

        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is TreeNode<T> other)
        {
            return Equals(other);
        }

        return false;
    }

    public override int GetHashCode()
    {
        if (Value == null)
        {
            return 0;
        }

        return EqualityComparer<T>.Default.GetHashCode(Value);
    }
}