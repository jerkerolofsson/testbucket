using TestBucket.Domain.Automation.Pipelines.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Components.Shared;

/// <summary>
/// An item representing either a test suite or a folder
/// </summary>
public record class BrowserItem
{
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public string? Href { get; set; }

    public string? VirtualFolderName { get; set; }

    public RequirementSpecification? RequirementSpecification { get; set; }
    public RequirementSpecificationFolder? RequirementFolder { get; set; }
    public Requirement? Requirement { get; set; }

    public TestCase? TestCase { get; set; }
    public TestSuite? TestSuite { get; set; }
    public TestLabFolder? TestLabFolder { get; set; }
    public TestRepositoryFolder? TestRepositoryFolder { get; set; }

    /// <summary>
    /// Test suite folder
    /// </summary>
    public TestSuiteFolder? Folder { get; set; }
    public TestRun? TestRun { get; set; }
    public Pipeline? Pipeline { get; set; }
    public SearchFolder? SearchFolder { get; internal set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        if (TestSuite is not null)
        {
            sb.Append($"TestSuite: {TestSuite.Id} ({TestSuite.Name})");
        }
        else if (Folder is not null)
        {
            sb.Append($"TestSuiteFolder: {Folder.Id} ({Folder.Name})");
        }
        else if (TestCase is not null)
        {
            sb.Append($"TestCase: {TestCase.Id} ({TestCase.Name})");
        }

        else if (Requirement is not null)
        {
            sb.Append($"Requirement: {Requirement.Id} ({Requirement.Name})");
        }
        else if (RequirementFolder is not null)
        {
            sb.Append($"RequirementFolder: {RequirementFolder.Id} ({RequirementFolder.Name})");
        }
        else if (RequirementSpecification is not null)
        {
            sb.Append($"RequirementSpecification: {RequirementSpecification.Id} ({RequirementSpecification.Name})");
        }
        else if(VirtualFolderName is not null)
        {
            sb.Append(this.VirtualFolderName);
        }

        return sb.ToString();

    }
}
