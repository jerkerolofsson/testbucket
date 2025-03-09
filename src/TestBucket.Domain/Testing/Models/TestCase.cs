using TestBucket.Contracts.Testing.Models;

namespace TestBucket.Domain.Testing.Models;

[Table("testcases")]
[Index(nameof(Created))]
[Index(nameof(Name))]
public class TestCase
{
    /// <summary>
    /// Database ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// External id
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// Timestamp when the test case was created
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Name of the test case
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Class name
    /// </summary>
    public string? ClassName { get; set; }

    /// <summary>
    /// Module name
    /// </summary>
    public string? Module { get; set; }

    /// <summary>
    /// Method
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// User friendly ID of the test case
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Test case description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Defines how the script of the test is defined,
    /// - ScriptedDefualt: as a single field describing all steps
    /// - ScriptedSteps: individual step descriptions
    /// </summary>
    public ScriptType ScriptType { get; set; } = ScriptType.ScriptedDefault;

    /// <summary>
    /// For automation, the assembly implementing the test
    /// </summary>
    public string? AutomationAssembly { get; set; }

    /// <summary>
    /// Folder path for the test case, separated with /
    /// </summary>
    public string Path { get; set; } = "";

    /// <summary>
    /// IDs for the path, for navigation
    /// </summary>
    public long[]? PathIds { get; set; }

    /// <summary>
    /// ID of tenant
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// ID of project
    /// </summary>
    public long? TestProjectId { get; set; }

    /// <summary>
    /// ID of test suite
    /// </summary>
    public long TestSuiteId { get; set; }

    /// <summary>
    /// ID of test suite folder
    /// </summary>
    public long? TestSuiteFolderId { get; set; }

    // Navigation
    public Tenant? Tenant { get; set; }
    public TestProject? TestProject { get; set; }
    public TestSuite? TestSuite { get; set; }
    public TestSuiteFolder? TestSuiteFolder { get; set; }
    public virtual IEnumerable<TestCaseField>? TestCaseFields { get; set; }
    public virtual IEnumerable<TestStep>? TestSteps { get; set; }
}
