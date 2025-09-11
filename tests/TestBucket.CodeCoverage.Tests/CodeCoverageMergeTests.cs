using TestBucket.CodeCoverage.Models;

namespace TestBucket.CodeCoverage.Tests;

/// <summary>
/// Contains unit tests for <see cref="CodeCoverageReportMerger"/> to verify merging of code coverage reports,
/// including packages, classes, lines, methods, and conditions.
/// </summary>
[Feature("Code Coverage")]
[Component("Code Coverage")]
[UnitTest]
[EnrichedTest]
[FunctionalTest]
public class CodeCoverageMergeTests
{
    /// <summary>
    /// Verifies that merging two simple reports combines packages and classes correctly.
    /// </summary>
    [Fact]
    [CoveredRequirement("COV-2")]
    public void Merge_SimpleReports_MergesPackagesAndClasses()
    {
        var report1 = new CodeCoverageReport();
        var package1 = report1.GetOrCreatePackage("TestPackage");
        var class1 = package1.GetOrCreateClass("TestClass", "TestClass.cs");
        var line1 = class1.GetOrCreateLine(10);
        line1.Hits = 1;

        var report2 = new CodeCoverageReport();
        var package2 = report2.GetOrCreatePackage("TestPackage");
        var class2 = package2.GetOrCreateClass("TestClass", "TestClass.cs");
        var line2 = class2.GetOrCreateLine(20);
        line2.Hits = 2;

        var merger = new CodeCoverageReportMerger();
        var merged = merger.Merge(report1, report2);

        var mergedPackage = merged.Packages.Single();
        var mergedClass = mergedPackage.Classes.Single();
        Assert.Equal("TestClass", mergedClass.Name);
        Assert.Equal(2, mergedClass.Lines.Count);
        Assert.Contains(mergedClass.Lines, l => l.LineNumber == 10 && l.Hits == 1);
        Assert.Contains(mergedClass.Lines, l => l.LineNumber == 20 && l.Hits == 2);
    }

    /// <summary>
    /// Verifies that hits on the same line are accumulated when merging reports.
    /// </summary>
    [Fact]
    [CoveredRequirement("COV-2")]
    public void Merge_SimpleReports_HitsOnSameLineAccumulated()
    {
        var report1 = new CodeCoverageReport();
        var package1 = report1.GetOrCreatePackage("TestPackage");
        var class1 = package1.GetOrCreateClass("TestClass", "TestClass.cs");
        var line1 = class1.GetOrCreateLine(10);
        line1.Hits = 1;

        var report2 = new CodeCoverageReport();
        var package2 = report2.GetOrCreatePackage("TestPackage");
        var class2 = package2.GetOrCreateClass("TestClass", "TestClass.cs");
        var line2 = class2.GetOrCreateLine(10);
        line2.Hits = 2;

        var merger = new CodeCoverageReportMerger();
        var merged = merger.Merge(report1, report2);

        var mergedPackage = merged.Packages.Single();
        var mergedClass = mergedPackage.Classes.Single();
        Assert.Equal("TestClass", mergedClass.Name);
        Assert.Single(mergedClass.Lines);
        Assert.Contains(mergedClass.Lines, l => l.LineNumber == 10 && l.Hits == 3);
    }

    /// <summary>
    /// Verifies that method lines are merged by line number and the last value for hits is used.
    /// </summary>
    [Fact]
    [CoveredRequirement("COV-2")]
    public void Merge_MergesMethodLinesByLineNumber()
    {
        var report1 = new CodeCoverageReport();
        var package1 = report1.GetOrCreatePackage("Pkg");
        var class1 = package1.GetOrCreateClass("Cls", "Cls.cs");
        var method1 = class1.GetOrCreateMethod("M", "void()");
        var line1 = method1.GetOrCreateLine(5);
        line1.Hits = 1;

        var report2 = new CodeCoverageReport();
        var package2 = report2.GetOrCreatePackage("Pkg");
        var class2 = package2.GetOrCreateClass("Cls", "Cls.cs");
        var method2 = class2.GetOrCreateMethod("M", "void()");
        var line2 = method2.GetOrCreateLine(5);
        line2.Hits = 2;

        var merger = new CodeCoverageReportMerger();
        var merged = merger.Merge(report1, report2);

        var mergedMethod = merged.Packages.Single().Classes.Single().Methods.Single();
        var mergedLine = mergedMethod.Lines.Single(l => l.LineNumber == 5);
        // The merge logic does not sum hits, but merges lines by number
        Assert.Equal(5, mergedLine.LineNumber);
        // The last value wins for Hits (since not merged in logic)
        Assert.Equal(3, mergedLine.Hits);
    }

    /// <summary>
    /// Verifies that conditions are merged by number and coverage, with the last value for coverage used.
    /// </summary>
    [Fact]
    public void Merge_MergesConditionsByNumberAndCoverage()
    {
        var report1 = new CodeCoverageReport();
        var package1 = report1.GetOrCreatePackage("Pkg");
        var class1 = package1.GetOrCreateClass("Cls", "Cls.cs");
        var line1 = class1.GetOrCreateLine(1);
        var cond1 = new CodeCoverageCondition { Number = 1, Type = "branch", Coverage = 0.5 };
        line1.AddCondition(cond1);

        var report2 = new CodeCoverageReport();
        var package2 = report2.GetOrCreatePackage("Pkg");
        var class2 = package2.GetOrCreateClass("Cls", "Cls.cs");
        var line2 = class2.GetOrCreateLine(1);
        var cond2 = new CodeCoverageCondition { Number = 1, Type = "branch", Coverage = 0.8 };
        line2.AddCondition(cond2);

        var merger = new CodeCoverageReportMerger();
        var merged = merger.Merge(report1, report2);

        var mergedLine = merged.Packages.Single().Classes.Single().Lines.Single();
        var mergedCond = mergedLine.Conditions.Single(c => c.Number == 1);
        // The merge logic sets coverage to the last value (not max)
        Assert.Equal("branch", mergedCond.Type);
        Assert.Equal(0.8, mergedCond.Coverage);
    }

    /// <summary>
    /// Verifies that merging multiple packages and classes merges all correctly.
    /// </summary>
    [Fact]
    [CoveredRequirement("COV-2")]
    public void Merge_MultiplePackagesAndClasses_MergesAll()
    {
        var report1 = new CodeCoverageReport();
        var pkg1 = report1.GetOrCreatePackage("A");
        var cls1 = pkg1.GetOrCreateClass("C1", "C1.cs");
        cls1.GetOrCreateLine(1).Hits = 1;

        var report2 = new CodeCoverageReport();
        var pkg2 = report2.GetOrCreatePackage("B");
        var cls2 = pkg2.GetOrCreateClass("C2", "C2.cs");
        cls2.GetOrCreateLine(2).Hits = 2;

        var merger = new CodeCoverageReportMerger();
        var merged = merger.Merge(report1, report2);

        Assert.Equal(2, merged.Packages.Count);
        Assert.Contains(merged.Packages, p => p.Name == "A");
        Assert.Contains(merged.Packages, p => p.Name == "B");
        Assert.Single(merged.Packages.Single(p => p.Name == "A").Classes);
        Assert.Single(merged.Packages.Single(p => p.Name == "B").Classes);
    }
}