using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Appium.PageSources;
/// <summary>
/// Represents a node in a UI hierarchy, typically used for Appium page sources.
/// </summary>
public record class HierarchyNode
{
    /// <summary>
    /// Gets or sets the tag name of the node.
    /// </summary>
    public required string TagName { get; set; }

    /// <summary>
    /// Parent node
    /// </summary>
    public HierarchyNode? Parent { get; set; }

    /// <summary>
    /// Gets or sets the index of the node.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the text for the node.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the class name of the node.
    /// </summary>
    public string? ClassName { get; set; }

    /// <summary>
    /// Gets or sets the package name of the node.
    /// </summary>
    public string? PackageName { get; set; }

    /// <summary>
    /// Gets or sets the resource ID of the node.
    /// </summary>
    public string? ResourceId { get; set; }

    /// <summary>
    /// Gets or sets the content description of the node.
    /// </summary>
    public string? ContentDescription { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node is checked.
    /// </summary>
    public bool? Checked { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node is checkable.
    /// </summary>
    public bool? Checkable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node is scrollable.
    /// </summary>
    public bool? Scrollable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node is displayed.
    /// </summary>
    public bool? Displayed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node is long-clickable.
    /// </summary>
    public bool? LongClickable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node is clickable.
    /// </summary>
    public bool? Clickable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node is enabled.
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node is focusable.
    /// </summary>
    public bool? Focusable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node is a password field.
    /// </summary>
    public bool? Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node is important for accessibility.
    /// </summary>
    public bool? A11yImportant { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node is focusable by a screen reader.
    /// </summary>
    public bool? ScreenReaderFocusable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node is showing a hint.
    /// </summary>
    public bool? ShowingHint { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node is context-clickable.
    /// </summary>
    public bool? ContextClickable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node has an invalid context.
    /// </summary>
    public bool? ContextInvalid { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node is a heading.
    /// </summary>
    public bool? Heading { get; set; }

    /// <summary>
    /// Gets or sets the drawing order of the node.
    /// </summary>
    public int? DrawingOrder { get; set; }

    /// <summary>
    /// Gets or sets the live region mode of the node.
    /// </summary>
    public int? LiveRegion { get; set; }

    /// <summary>
    /// Gets or sets the X coordinate boundary of the node.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate boundary of the node.
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Gets the left boundary of the node.
    /// </summary>
    public int Left => X;

    /// <summary>
    /// Gets the top boundary of the node.
    /// </summary>
    public int Top => Y;

    /// <summary>
    /// Gets the right boundary of the node.
    /// </summary>
    public int Right => X + Width;

    /// <summary>
    /// Gets the bottom boundary of the node.
    /// </summary>
    public int Bottom => Y + Height;

    /// <summary>
    /// Gets or sets the width of the node.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the node.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the child nodes of the current node.
    /// </summary>
    public List<HierarchyNode> Nodes { get; set; } = new List<HierarchyNode>();

    /// <summary>
    /// Matches this node, or a descendant. If it doesn't match it moves to an ancestor and repeats.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="maxAncestors"></param>
    /// <returns></returns>
    public bool MatchThisOrSibling(Predicate<HierarchyNode> predicate, int maxAncestors)
    {
        if (predicate(this))
        {
            return true;
        }

        var node = this;

        for (int i = 0; i <= maxAncestors && node is not null; i++)
        {
            if (node.MatchThisOrDescendant(predicate))
            {
                return true;
            }
            node = node.Parent;
        }

        return false;
    }

    public IEnumerable<HierarchyNode> Descendants()
    {
        foreach (var node in Nodes)
        {
            yield return node;

            foreach (var childNode in node.Descendants())
            {
                yield return childNode;
            }
        }
    }

    /// <summary>
    /// Moves up numAncestors ancestors then scans all descendants
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="numAncestors"></param>
    /// <returns></returns>
    public IEnumerable<HierarchyNode> FindAncestorDescendants(Predicate<HierarchyNode> predicate, int numAncestors)
    {
        var node = this.Parent;
        for (int i = 0; i <= numAncestors && node is not null; i++)
        {
            // Exit early
            if (node.Parent is null)
            {
                break;
            }
            node = node.Parent;
        }

        if (node is not null)
        {
            foreach (var descenant in node.Descendants())
                if (predicate(descenant))
                {
                    yield return descenant;
                }

        }
    }
    public bool MatchThisOrDescendant(Predicate<HierarchyNode> predicate)
    {
        if (predicate(this))
        {
            return true;
        }

        // Check if parent is clickable
        foreach (var node in this.Nodes)
        {
            if (node.MatchThisOrDescendant(predicate))
            {
                return true;
            }
        }
        return false;
    }

    public bool MatchThisOrAncestor(Predicate<HierarchyNode> predicate)
    {
        if (predicate(this))
        {
            return true;
        }

        // Check if parent is clickable
        var node = this.Parent;
        while (node is not null)
        {
            if (predicate(node))
            {
                return true;
            }
            node = node.Parent;
        }

        return false;
    }
}