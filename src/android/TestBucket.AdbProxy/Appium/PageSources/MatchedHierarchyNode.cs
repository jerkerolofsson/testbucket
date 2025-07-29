using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Appium.PageSources;

public record class MatchedHierarchyNode(NodeMatchType MatchType, HierarchyNode Node)
{
    public int MatchScore => MatchType switch
    {
        NodeMatchType.ExactId => 100,
        NodeMatchType.ExactContentDescription => 100,
        NodeMatchType.ExactText => 100,

        NodeMatchType.PartialId => 50,
        NodeMatchType.PartialContentDescription => 50,
        NodeMatchType.PartialText => 50,
        _ => 0,
    } + GetIsClickableScore() + GetIsParentClickableScore() + GetAccessibilityScore();

    /// <summary>
    /// Rank items that are important for accessibility higher
    /// </summary>
    /// <returns></returns>
    private int GetAccessibilityScore()
    {
        return Node.A11yImportant == true ? 5 : 0;
    }

    /// <summary>
    /// Rank items that are clickable higher
    /// </summary>
    /// <returns></returns>
    private int GetIsClickableScore()
    {
        return Node.Clickable == true ? 20 : 0;
    }

    /// <summary>
    /// Rank items where the parent is clickable higher
    /// </summary>
    /// <returns></returns>
    private int GetIsParentClickableScore()
    {
        HierarchyNode? parent = Node.Parent;
        while (parent is not null)
        {
            if(parent.Clickable == true)
            {
                return 10;
            }
            parent = parent.Parent;
        }
        return 0;
    }
}
