using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.CodeCoverage.Models;
public record class CodeCoverageMethod : CodeEntity
{
    private readonly List<CodeCoverageLine> _lines = [];

    /// <summary>
    /// Method name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Method signature
    /// </summary>
    public required string Signature { get; set; }

    /// <summary>
    /// Lines for this method
    /// </summary>
    public IReadOnlyList<CodeCoverageLine> Lines => _lines;

    public override Lazy<int> CoveredLineCount => new Lazy<int>(() => _lines.Select(x => x.CoveredLineCount.Value).Sum());
    public override Lazy<int> LineCount => new Lazy<int>(() => _lines.Select(x => x.LineCount.Value).Sum());

    public CodeCoverageMethod()
    {

    }

    /// <summary>
    /// Adds a line
    /// </summary>
    /// <param name="line"></param>
    public void AddLine(CodeCoverageLine line)
    {
        _lines.Add(line);
    }

    public CodeCoverageLine GetOrCreateLine(int lineNumber)
    {
        var line = FindLineByNumber(lineNumber);
        if (line is null)
        {
            line = new CodeCoverageLine { LineNumber = lineNumber };
            AddLine(line);
        }
        return line;
    }

    public CodeCoverageLine? FindLineByNumber(int lineNumber)
    {
        return FindLine(x => x.LineNumber == lineNumber);
    }
    public CodeCoverageLine? FindLine(Predicate<CodeCoverageLine> predicate)
    {
        return Lines.FirstOrDefault(x => predicate(x));
    }

    public override string GetName() => Name;

    public override IReadOnlyList<CodeEntity> GetChildren() => _lines;

}
