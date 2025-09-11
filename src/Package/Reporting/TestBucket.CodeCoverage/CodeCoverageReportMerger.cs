using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.CodeCoverage;

internal class CodeCoverageReportMerger
{
    public CodeCoverageReport Merge(CodeCoverageReport report1, CodeCoverageReport report2)
    {
        var merged = new CodeCoverageReport();

        // Merge all packages by name
        var allPackages = report1.Packages.Concat(report2.Packages)
            .GroupBy(p => p.Name);

        foreach (var packageGroup in allPackages)
        {
            var mergedPackage = merged.GetOrCreatePackage(packageGroup.Key);

            // Merge all classes by name and filename
            var allClasses = packageGroup.SelectMany(p => p.Classes)
                .GroupBy(c => (c.Name, c.FileName));

            foreach (var classGroup in allClasses)
            {
                var mergedClass = mergedPackage.GetOrCreateClass(classGroup.Key.Name, classGroup.Key.FileName);

                // Merge class lines by line number
                var allClassLines = classGroup.SelectMany(c => c.Lines)
                    .GroupBy(l => l.LineNumber);

                foreach (var lineGroup in allClassLines)
                {
                    var mergedLine = mergedClass.GetOrCreateLine(lineGroup.Key);
                    MergeLines(lineGroup, mergedLine);
                }

                // Merge methods by name and signature
                var allMethods = classGroup.SelectMany(c => c.Methods)
                    .GroupBy(m => (m.Name, m.Signature));

                foreach (var methodGroup in allMethods)
                {
                    var mergedMethod = mergedClass.GetOrCreateMethod(methodGroup.Key.Name, methodGroup.Key.Signature);

                    // Merge method lines by line number
                    var allMethodLines = methodGroup.SelectMany(m => m.Lines)
                        .GroupBy(l => l.LineNumber);

                    foreach (var methodLineGroup in allMethodLines)
                    {
                        var mergedMethodLine = mergedMethod.GetOrCreateLine(methodLineGroup.Key);
                        MergeLines(methodLineGroup, mergedMethodLine);
                    }
                }
            }
        }

        return merged;
    }

    private static void MergeLines(IGrouping<int, Models.CodeCoverageLine> lineGroup, Models.CodeCoverageLine mergedLine)
    {
        foreach (var line in lineGroup)
        {
            mergedLine.IsBranch = line.IsBranch;
            mergedLine.Hits += line.Hits;

            foreach (var condition in line.Conditions)
            {
                var mergedCondition = mergedLine.GetOrCreateCondition(condition.Number);
                mergedCondition.Type = condition.Type;
                mergedCondition.Coverage = Math.Max(mergedCondition.Coverage, condition.Coverage);
            }
        }
    }
}
