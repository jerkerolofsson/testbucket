namespace TestBucket.Contracts.Insights;
public enum InsightsSort
{
    Unsorted,

    /// <summary>
    /// Sort by the label, ascending
    /// </summary>
    LabelAscending,

    LabelDescending,

    ValueAscending,

    ValueDescending,    
}
