namespace TestBucket.CodeCoverage.Models;
public record class CodeCoverageClass
{
    public required string Name { get; set; }
    public required string FileName { get; set; }

    private readonly List<CodeCoverageMethod> _methods = [];

    public IReadOnlyList<CodeCoverageMethod> Methods => _methods;

    public CodeCoverageClass()
    {

    }

    public void AddMethod(CodeCoverageMethod method)
    {
        _methods.Add(method);
    }

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

    public CodeCoverageMethod? FindMethodBySignature(string name, string signature)
    {
        return FindMethod(x => x.Signature == signature && x.Name == name);
    }
    public CodeCoverageMethod? FindMethod(Predicate<CodeCoverageMethod> predicate)
    {
        return Methods.FirstOrDefault(x => predicate(x));
    }
}
