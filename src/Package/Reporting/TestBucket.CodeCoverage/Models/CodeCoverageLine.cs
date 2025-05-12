using System.Xml.Linq;

namespace TestBucket.CodeCoverage.Models;
public record class CodeCoverageLine : CodeEntity
{
    private readonly List<CodeCoverageCondition> _conditions = [];

    /// <summary>
    /// The line number
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// Branch or not
    /// </summary>
    public bool IsBranch { get; set; }

    /// <summary>
    /// Number of hits
    /// </summary>
    public int Hits { get; set; }

    /// <summary>
    /// Gets all conditions
    /// </summary>
    public IReadOnlyList<CodeCoverageCondition> Conditions => _conditions;

    public override Lazy<int> CoveredLineCount => new Lazy<int>(() => Hits > 0 ? 1 : 0);
    public override Lazy<int> LineCount => new Lazy<int>(() =>  1);

    public CodeCoverageLine()
    {

    }

    /// <summary>
    /// Adds a condition
    /// </summary>
    /// <param name="condition"></param>
    public void AddCondition(CodeCoverageCondition condition)
    {
        _conditions.Add(condition);
    }

    public CodeCoverageCondition GetOrCreateCondition(int number)
    {
        var condition = FindConditionByNumber(number);
        if (condition is null)
        {
            condition = new CodeCoverageCondition { Number = number };
            AddCondition(condition);
        }
        return condition;
    }

    public CodeCoverageCondition? FindConditionByNumber(int number)
    {
        return FindLine(x => x.Number == number);
    }
    public CodeCoverageCondition? FindLine(Predicate<CodeCoverageCondition> predicate)
    {
        return Conditions.FirstOrDefault(x => predicate(x));
    }


    public override string GetName() => $"Line {LineNumber}";

    public override IReadOnlyList<CodeEntity> GetChildren() => _conditions;

}
