namespace TestBucket.Components.Tests.Services;

public class TestBrowserRequest : SearchTestQuery
{
    public bool ShowTestSuiteFolders { get; set; } = true;
    public bool ShowTestCases { get; set; } = true;
    public bool ShowTestRuns { get; set; } = true;
    public bool ShowTestSuites { get; set; } = true;

    public BrowserItem? Parent { get; set; }
    public bool ShowTestRunPipelines { get; set; }
    public bool ShowTestRunTests { get; set; }
}
