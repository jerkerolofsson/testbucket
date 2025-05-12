namespace TestBucket.CodeCoverage.Models;
public class CodeCoverageCondition
{
    public int Number { get; set; }

    /// <summary>
    /// Type of condition
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Coverage for the condition between 0.0 and 1.0
    /// </summary>
    public double Coverage { get; set; }
}
