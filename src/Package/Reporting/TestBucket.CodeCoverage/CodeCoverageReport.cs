using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.CodeCoverage.Models;

namespace TestBucket.CodeCoverage;
public class CodeCoverageReport
{
    private readonly List<CodeCoveragePackage> _packages = [];
    public IReadOnlyList<CodeCoveragePackage> Packages => _packages;

    public void AddPackage(CodeCoveragePackage package)
    {
        _packages.Add(package);
    }

    public CodeCoveragePackage GetOrCreatePackage(string name)
    {
        var package = FindPackageByName(name);
        if (package is null)
        {
            package = new CodeCoveragePackage { Name = name };
            AddPackage(package);
            return package;
        }
        return package;
    }

    public CodeCoveragePackage? FindPackageByName(string name)
    {
        return FindPackage(x => x.Name == name);
    }
    public CodeCoveragePackage? FindPackage(Predicate<CodeCoveragePackage> predicate)
    {
        return _packages.FirstOrDefault(x => predicate(x));
    }
}
