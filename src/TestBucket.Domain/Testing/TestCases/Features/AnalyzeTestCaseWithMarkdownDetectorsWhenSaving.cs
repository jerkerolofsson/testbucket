using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Markdown;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestCases.Features;
internal class AnalyzeTestCaseWithMarkdownDetectorsWhenSaving
{
    internal static async Task DetectThingsWithMarkdownDetectorsAsync(ClaimsPrincipal principal, TestCase testCase, IEnumerable<IMarkdownDetector> detectors)
    {
        foreach (var detector in detectors)
        {
            await detector.ProcessAsync(principal, testCase);
        }
    }
}
