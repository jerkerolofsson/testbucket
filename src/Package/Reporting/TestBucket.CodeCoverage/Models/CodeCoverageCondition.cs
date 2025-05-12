using System.Xml.Linq;

namespace TestBucket.CodeCoverage.Models;
public record class CodeCoverageCondition : CodeEntity
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

    public override Lazy<int> LineCount => new Lazy<int>(() => 1);

    public override Lazy<int> CoveredLineCount => new Lazy<int>(() => Coverage > 0 ? 1 : 0);

    public override string GetName() => $"Condition {Number}";

    public override IReadOnlyList<CodeEntity> GetChildren() => [];

    public override Lazy<double> CoveragePercent 
    {
        get
        {
            return new Lazy<double>(() => Math.Round(Coverage * 100.0, 2));
        }
    }
}
