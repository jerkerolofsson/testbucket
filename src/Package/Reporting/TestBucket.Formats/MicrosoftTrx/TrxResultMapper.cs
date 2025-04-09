namespace TestBucket.Formats.MicrosoftTrx
{
    internal static class TrxResultMapper
    {
        public static string Map(TestResult? result)
        {
            return result switch
            {
                TestResult.Passed => "Passed",
                TestResult.Failed => "Failed",
                TestResult.Skipped => "Ignored",
                TestResult.Hang => "Timeout",
                TestResult.Inconclusive => "Inconclusive",
                _ => "NotFound"
            };
        }
        public static TestResult Map(string? resultString)
        {
            return resultString switch
            {
                "Error" => TestResult.Error,
                "Failed" => TestResult.Failed,
                "Timeout" => TestResult.Hang,
                "Inconclusive" => TestResult.Inconclusive,
                "Ignored" => TestResult.Skipped,
                "NotRunnable" => TestResult.Skipped,
                "Passed" => TestResult.Passed,
                "NotFound" => TestResult.Error,
                "InProgress" => TestResult.Error,

                _ => TestResult.Skipped,
            };
        }
    }
}
