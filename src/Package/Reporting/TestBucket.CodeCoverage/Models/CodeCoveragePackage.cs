namespace TestBucket.CodeCoverage.Models;
public record class CodeCoveragePackage
{
    private readonly List<CodeCoverageClass> _classes = [];

    /// <summary>
    /// Package/assembly name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Classes
    /// </summary>
    public IReadOnlyList<CodeCoverageClass> Classes => _classes;

    public CodeCoveragePackage()
    {

    }

    public void AddClass(CodeCoverageClass codeCoverageClass)
    {
        _classes.Add(codeCoverageClass);
    }

    public CodeCoverageClass GetOrCreateClass(string name, string filename)
    {
        var codeCoverageClass = FindClassByName(name);
        if (codeCoverageClass is null)
        {
            codeCoverageClass = new CodeCoverageClass { Name = name, FileName = filename };
            AddClass(codeCoverageClass);
        }
        return codeCoverageClass;
    }

    public CodeCoverageClass? FindClassByName(string name)
    {
        return FindClass(x => x.Name == name);
    }
    public CodeCoverageClass? FindClass(Predicate<CodeCoverageClass> predicate)
    {
        return Classes.FirstOrDefault(x => predicate(x));
    }
}
