using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Formats.Dtos;

namespace TestBucket.Domain.Testing.Formats;
internal static class ImportDefaults
{
    public static string GetExternalId(TestRunDto testRun, TestSuiteRunDto testSuite, TestCaseRunDto testCase)
    {
        string[] components = [testRun.Name ?? "", testSuite.Name ?? "", testCase.ClassName ?? "", testCase.Name ?? "", testCase.Module ?? ""];
        return string.Join('-', components);
    }
}
