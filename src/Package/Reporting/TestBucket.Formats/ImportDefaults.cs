namespace TestBucket.Formats;
internal static class ImportDefaults
{
    public static string GetExternalId(TestRunDto testRun, TestSuiteRunDto testSuite, TestCaseRunDto testCase)
    {
        string[] components = [testRun.Name ?? "", testSuite.Name ?? "", testCase.ClassName ?? "", testCase.Name ?? "", testCase.Module ?? ""];
        return string.Join('-', components);
    }
}
