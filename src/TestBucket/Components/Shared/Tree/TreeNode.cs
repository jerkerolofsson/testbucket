using System.Diagnostics.CodeAnalysis;

namespace TestBucket.Components.Shared.Tree;

/// <summary>
/// Represents a node in a tree structure, supporting generic value types and child nodes.
/// </summary>
/// <typeparam name="T">The type of value stored in the node.</typeparam>
public class TreeNode<T> : IEquatable<TreeNode<T>> where T : class
{
    /// <summary>
    /// Gets or sets the display text for the node.
    /// </summary>
    public virtual string? Text { get; set; }

    /// <summary>
    /// Gets or sets the icon identifier for the node.
    /// </summary>
    public virtual string? Icon { get; set; }

    /// <summary>
    /// Gets the value associated with the node.
    /// </summary>
    public T? Value { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the node is expanded in the UI.
    /// </summary>
    public virtual bool Expanded { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node can be expanded.
    /// </summary>
    public virtual bool Expandable { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the node is selected.
    /// </summary>
    public virtual bool Selected { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node is visible.
    /// </summary>
    public virtual bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets the child nodes of this node.
    /// </summary>
    public virtual IReadOnlyCollection<TreeNode<T>>? Children { get; set; }

    /// <summary>
    /// Gets a value indicating whether this node has any child nodes.
    /// </summary>
    [MemberNotNullWhen(true, "Children")]
    public virtual bool HasChildren
    {
        [MemberNotNullWhen(true, "Children")]
        get
        {
            if (Children is not null)
            {
                return Children.Count > 0;
            }

            return false;
        }
    }

    /// <summary>
    /// Initializes a new instance of the TreeNode class.
    /// </summary>
    public TreeNode()
        : this(default(T))
    {
    }

    /// <summary>
    /// Initializes a new instance of the TreeNode class with the specified value.
    /// </summary>
    /// <param name="value">The value to associate with the node.</param>
    protected TreeNode(T? value)
    {
        Value = value;
    }

    /// <summary>
    /// Determines whether the specified TreeNode is equal to the current node.
    /// </summary>
    /// <param name="other">The node to compare with the current node.</param>
    /// <returns><c>true</c> if the nodes are equal; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Determines whether the specified object is equal to the current node.
    /// </summary>
    /// <param name="obj">The object to compare with the current node.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is TreeNode<T> other)
        {
            return Equals(other);
        }

        return false;
    }

    /// <summary>
    /// Returns a string that represents the current node.
    /// </summary>
    /// <returns>A string representation of the node.</returns>
    public override string ToString()
    {
        if (Value is not null)
        {
            return Value.ToString() ?? "Value: (null)";
        }
        return "TreeNode";
    }

    /// <summary>
    /// Returns a hash code for the current node.
    /// </summary>
    /// <returns>A hash code for the node.</returns>
    public override int GetHashCode()
    {
        if (Value == null)
        {
            return 0;
        }

        return EqualityComparer<T>.Default.GetHashCode(Value);
    }
}