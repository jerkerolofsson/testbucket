using TestBucket.CodeCoverage.Models;

namespace TestBucket.CodeCoverage;

/// <summary>
/// Represents a code coverage report containing a collection of code coverage packages.
/// </summary>
public class CodeCoverageReport
{
    private readonly List<CodeCoveragePackage> _packages = [];

    /// <summary>
    /// Gets the list of code coverage packages included in the report.
    /// </summary>
    public IReadOnlyList<CodeCoveragePackage> Packages => _packages;

    public Lazy<int> CoveredLineCount => new Lazy<int>(() => _packages.Select(x => x.CoveredLineCount.Value).Sum());
    public Lazy<int> LineCount => new Lazy<int>(() => _packages.Select(x => x.LineCount.Value).Sum());

    /// <summary>
    /// Line Coverage in % between 0.0 and 100.0
    /// </summary>
    public virtual Lazy<double> CoveragePercent
    {
        get
        {
            return new Lazy<double>(() =>
            {
                if (LineCount.Value == 0)
                {
                    return 0.0;
                }
                return Math.Round((double)CoveredLineCount.Value / LineCount.Value * 100, 2);
            });
        }
    }

    /// <summary>
    /// Adds a code coverage package to the report.
    /// </summary>
    /// <param name="package">The <see cref="CodeCoveragePackage"/> to add.</param>
    public void AddPackage(CodeCoveragePackage package)
    {
        _packages.Add(package);
    }

    /// <summary>
    /// Gets an existing package by name, or creates and adds a new one if it does not exist.
    /// </summary>
    /// <param name="name">The name of the package.</param>
    /// <returns>The existing or newly created <see cref="CodeCoveragePackage"/>.</returns>
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

    /// <summary>
    /// Finds a package by its name.
    /// </summary>
    /// <param name="name">The name of the package to find.</param>
    /// <returns>The <see cref="CodeCoveragePackage"/> if found; otherwise, <c>null</c>.</returns>
    public CodeCoveragePackage? FindPackageByName(string name)
    {
        return FindPackage(x => x.Name == name);
    }

    /// <summary>
    /// Finds a package that matches the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to match packages.</param>
    /// <returns>The <see cref="CodeCoveragePackage"/> if found; otherwise, <c>null</c>.</returns>
    public CodeCoveragePackage? FindPackage(Predicate<CodeCoveragePackage> predicate)
    {
        return _packages.FirstOrDefault(x => predicate(x));
    }


    /// <summary>
    /// Gets all classes
    /// </summary>
    /// <returns></returns>
    public IEnumerable<CodeCoverageClass> GetClasses(Predicate<CodeCoverageClass> predicate)
    {
        foreach (var package in _packages)
        {
            foreach (var @class in package.Classes)
            {
                if (predicate(@class))
                {
                    yield return @class;
                }
            }
        }
    }

    /// <summary>
    /// Gets all methods
    /// </summary>
    /// <returns></returns>
    public IEnumerable<CodeCoverageMethod> GetMethods(Predicate<CodeCoverageMethod> predicate)
    {
        foreach(var package in _packages)
        {
            foreach(var @class in package.Classes)
            {
                foreach(var method in @class.Methods)
                {
                    if(predicate(method))
                    {
                        yield return method;
                    }
                }
            }
        }
    }
}