using TestBucket.Domain.Automation.Pipelines.Models;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Components.Shared;

/// <summary>
/// An item representing either a test suite or a folder
/// </summary>
public record class BrowserItem
{
    public string? Color { get; set; }
    public string? Icon { get; set; }

    public string? VirtualFolderName { get; set; }

    public RequirementSpecification? RequirementSpecification { get; set; }
    public RequirementSpecificationFolder? RequirementFolder { get; set; }
    public Requirement? Requirement { get; set; }

    public TestCase? TestCase { get; set; }
    public TestSuite? TestSuite { get; set; }
    public TestSuiteFolder? Folder { get; set; }
    public TestRun? TestRun { get; set; }
    public Pipeline? Pipeline { get; set; }
    public SearchTestCaseRunQuery? TestCaseRunQuery { get; internal set; }
}
