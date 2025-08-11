namespace TestBucket.Components.Tests.Models;

/// <summary>
/// A folder or a test case
/// </summary>
public class TestSuiteItem
{
    public TestSuiteFolder? Folder { get; set; }
    public TestCase? TestCase { get; set; }

    public string? State => TestCase?.State;
    public string? Name => TestCase?.Name ?? Folder?.Name;
    public DateTimeOffset? Modified => TestCase?.Modified ?? Folder?.Modified;
}
