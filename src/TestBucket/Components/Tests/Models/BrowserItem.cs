namespace TestBucket.Components.Tests.Models;

/// <summary>
/// An item representing either a test suite or a folder
/// </summary>
public record class BrowserItem
{
    public string? Color { get; set; }
    public string? Icon { get; set; }

    public TestCase? TestCase { get; set; }
    public TestSuite? TestSuite { get; set; }
    public TestSuiteFolder? Folder { get; set; }
    public TestRun? TestRun { get; set; }
}
