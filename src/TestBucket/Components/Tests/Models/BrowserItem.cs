using TestBucket.Data.Testing.Models;

namespace TestBucket.Components.Tests.Models;

/// <summary>
/// An item browsed
/// </summary>
public record class BrowserItem
{
    public TestSuite? TestSuite { get; set; }
    public TestSuiteFolder? Folder { get; set; }
}
