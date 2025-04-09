namespace TestBucket.Formats.XUnit
{
    internal static class XUnitResultMapper
    {
        public static string Map(TestResult? result)
        {
            return result switch
            {
                TestResult.Passed => "Pass",
                TestResult.Failed => "Fail",
                TestResult.Skipped => "Skip",
                _ => "Skip"
            };
        }
        public static TestResult Map(string resultString)
        {
            return resultString switch
            {
                "Pass" => TestResult.Passed,
                "Fail" => TestResult.Failed,
                "Skip" => TestResult.Skipped,
                _ => TestResult.Skipped,
            };
        }
    }
}
