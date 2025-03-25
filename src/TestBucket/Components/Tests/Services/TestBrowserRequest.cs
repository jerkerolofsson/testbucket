namespace TestBucket.Components.Tests.Services;

public class TestBrowserRequest : SearchTestQuery
{
    public bool ShowTestCases { get; set; } = true;
    public bool ShowTestRuns { get; set; } = true;
    public bool ShowTestSuites { get; set; } = true;

    public BrowserItem? Parent { get; set; }
}
