namespace TestBucket.CodeCoverage.Models;

/// <summary>
/// Represents a code coverage class, containing coverage data for lines and methods within a class.
/// </summary>
public record class CodeCoverageClass : CodeEntity
{
    /// <summary>
    /// Lines for the class, excluding methods.
    /// </summary>
    private readonly List<CodeCoverageLine> _lines = [];

    /// <summary>
    /// Methods for the class.
    /// </summary>
    private readonly List<CodeCoverageMethod> _methods = [];

    /// <summary>
    /// Name of the class.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Filename of the class (may be relative).
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// Gets the methods for the class.
    /// </summary>
    public IReadOnlyList<CodeCoverageMethod> Methods => _methods;

    /// <summary>
    /// Gets the lines for this class, excluding methods.
    /// </summary>
    public IReadOnlyList<CodeCoverageLine> Lines => _lines;

    /// <summary>
    /// Gets the total number of covered lines in the class, including methods.
    /// </summary>
    public override Lazy<int> CoveredLineCount => new Lazy<int>(() => ClassCoveredLineCount.Value + MethodCoveredLineCount.Value);

    /// <summary>
    /// Gets the total number of lines in the class, including methods.
    /// </summary>
    public override Lazy<int> LineCount => new Lazy<int>(() => ClassLineCount.Value + MethodLineCount.Value);

    /// <summary>
    /// Gets the number of covered lines in the class, excluding methods.
    /// </summary>
    public Lazy<int> ClassCoveredLineCount => new Lazy<int>(() => _lines.Select(x => x.CoveredLineCount.Value).Sum());

    /// <summary>
    /// Gets the number of lines in the class, excluding methods.
    /// </summary>
    public Lazy<int> ClassLineCount => new Lazy<int>(() => _lines.Select(x => x.LineCount.Value).Sum());

    /// <summary>
    /// Gets the number of covered lines in all methods.
    /// </summary>
    public Lazy<int> MethodCoveredLineCount => new Lazy<int>(() => _methods.Select(x => x.CoveredLineCount.Value).Sum());

    /// <summary>
    /// Gets the number of lines in all methods.
    /// </summary>
    public Lazy<int> MethodLineCount => new Lazy<int>(() => _methods.Select(x => x.LineCount.Value).Sum());

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeCoverageClass"/> class.
    /// </summary>
    public CodeCoverageClass()
    {
    }

    /// <summary>
    /// Adds a line to the class.
    /// </summary>
    /// <param name="line">The line to add.</param>
    public void AddLine(CodeCoverageLine line)
    {
        _lines.Add(line);
    }

    /// <summary>
    /// Gets an existing line by line number, or creates and adds a new one if not found.
    /// </summary>
    /// <param name="lineNumber">The line number to find or create.</param>
    /// <returns>The found or newly created <see cref="CodeCoverageLine"/>.</returns>
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

    /// <summary>
    /// Finds a line by its line number.
    /// </summary>
    /// <param name="lineNumber">The line number to search for.</param>
    /// <returns>The matching <see cref="CodeCoverageLine"/>, or <c>null</c> if not found.</returns>
    public CodeCoverageLine? FindLineByNumber(int lineNumber)
    {
        return FindLine(x => x.LineNumber == lineNumber);
    }

    /// <summary>
    /// Finds a line matching the given predicate.
    /// </summary>
    /// <param name="predicate">The predicate to match.</param>
    /// <returns>The matching <see cref="CodeCoverageLine"/>, or <c>null</c> if not found.</returns>
    public CodeCoverageLine? FindLine(Predicate<CodeCoverageLine> predicate)
    {
        return Lines.FirstOrDefault(x => predicate(x));
    }

    /// <summary>
    /// Adds a method to the class.
    /// </summary>
    /// <param name="method">The method to add.</param>
    public void AddMethod(CodeCoverageMethod method)
    {
        _methods.Add(method);
    }

    /// <summary>
    /// Gets an existing method by name and signature, or creates and adds a new one if not found.
    /// </summary>
    /// <param name="name">The method name.</param>
    /// <param name="signature">The method signature.</param>
    /// <returns>The found or newly created <see cref="CodeCoverageMethod"/>.</returns>
    public CodeCoverageMethod GetOrCreateMethod(string name, string signature)
    {
        var method = FindMethodBySignature(name, signature);
        if (method is null)
        {
            method = new CodeCoverageMethod { Name = name, Signature = signature };
            AddMethod(method);
        }
        return method;
    }

    /// <summary>
    /// Finds a method by its name and signature.
    /// </summary>
    /// <param name="name">The method name.</param>
    /// <param name="signature">The method signature.</param>
    /// <returns>The matching <see cref="CodeCoverageMethod"/>, or <c>null</c> if not found.</returns>
    public CodeCoverageMethod? FindMethodBySignature(string name, string signature)
    {
        return FindMethod(x => x.Signature == signature && x.Name == name);
    }

    /// <summary>
    /// Finds a method matching the given predicate.
    /// </summary>
    /// <param name="predicate">The predicate to match.</param>
    /// <returns>The matching <see cref="CodeCoverageMethod"/>, or <c>null</c> if not found.</returns>
    public CodeCoverageMethod? FindMethod(Predicate<CodeCoverageMethod> predicate)
    {
        return Methods.FirstOrDefault(x => predicate(x));
    }

    /// <summary>
    /// Gets the name of the class.
    /// </summary>
    /// <returns>The class name.</returns>
    public override string GetName() => Name;

    /// <summary>
    /// Gets the child entities (methods) of the class.
    /// </summary>
    /// <returns>A read-only list of <see cref="CodeEntity"/> representing the methods.</returns>
    public override IReadOnlyList<CodeEntity> GetChildren() => _methods;
}