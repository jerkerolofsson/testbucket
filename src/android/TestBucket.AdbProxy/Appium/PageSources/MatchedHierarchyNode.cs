
namespace TestBucket.AdbProxy.Appium.PageSources;

public record class MatchedHierarchyNode(NodeMatchType MatchType, HierarchyNode Node)
{
    public double? EmbeddingDistance { get; set; }

    public int MatchScore => MatchType switch
    {
        NodeMatchType.ExactId => 1000,
        NodeMatchType.ExactContentDescription => 1000,
        NodeMatchType.ExactText => 1000,

        NodeMatchType.PartialId => 500,
        NodeMatchType.PartialContentDescription => 500,
        NodeMatchType.PartialText => 500,

        NodeMatchType.EmbeddingText => 400,
        NodeMatchType.EmbeddingContentDescription => 300,
        NodeMatchType.EmbeddingId => 200,
        _ => 0,
    } + GetIsClickableScore() + GetAccessibilityScore() + GetEmbeddingDistanceScore();

    private int GetEmbeddingDistanceScore()
    {
        if(EmbeddingDistance != null)
        {
            return (int)(EmbeddingDistance.Value*100.0);
        }
        return 0;
    }

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
        if (Node.Clickable == true)
        {
            return 30;
        }

        // Check if parent is clickable
        if (Node.MatchThisOrAncestor(x => x.Clickable == true))
        {
            return 10;
        }
        return 0;
    }


    /// <summary>
    /// Returns true if this element or a parent element is clickable
    /// </summary>
    /// <returns></returns>
    public bool IsThisOrParentClickable()
    {
        return Node.MatchThisOrAncestor(x => x.Clickable == true);
    }
}
