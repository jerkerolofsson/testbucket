using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Formats.Ctrf
{
    internal static class CtrfResultMapping
    {
        internal static string GetCtrfStatusFromTestResult(TestResult result)
        {
            return result switch
            {
                TestResult.Passed => "passed",
                TestResult.Failed => "failed",
                TestResult.NoRun => "pending",
                TestResult.Skipped => "skipped",
                TestResult.Other => "other",

                // Not in spec
                TestResult.Error => "error",
                TestResult.Blocked => "blocked",
                TestResult.Hang => "hang",
                TestResult.Crashed => "crashed",
                
                _ => "other"
            };
        }
        internal static  TestResult GetTestResultFromCtrfStatus(string status)
        {
            return status switch
            {
                "passed" => TestResult.Passed,
                "failed" => TestResult.Failed,
                "pending" => TestResult.NoRun,
                "skipped" => TestResult.Skipped,
                "other" => TestResult.Other,

                // Not in spec
                "error" => TestResult.Error,
                "blocked" => TestResult.Blocked,
                "hang" => TestResult.Hang,
                "crashed" => TestResult.Crashed,

                _ => TestResult.Other
            };
        }
    }
}
