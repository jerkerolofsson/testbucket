using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Testing.Models;

[Table("testcases")]
[Index(nameof(Created))]
[Index(nameof(Name), nameof(Created))]
[Index(nameof(TenantId), nameof(Slug))]
[Index(nameof(TenantId), nameof(TestProjectId), nameof(ExternalId), nameof(Created))]
public class TestCase : TestEntity
{
    /// <summary>
    /// Database ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 1 for first item. This is unique only per project
    /// </summary>
    public int? SequenceNumber { get; set; }

    /// <summary>
    /// External id
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// External display id
    /// </summary>
    public string? ExternalDisplayId { get; set; }

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
    /// Planned session duration, in minutes
    /// </summary>
    public int? SessionDuration { get; set; }

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
    /// Pre-conditions
    /// </summary>
    public string? Preconditions { get; set; }

    /// <summary>
    /// Post-conditions
    /// </summary>
    public string? Postconditions { get; set; }

    /// <summary>
    /// Defines how the script of the test is defined,
    /// - ScriptedDefualt: as a single field describing all steps
    /// - ScriptedSteps: individual step descriptions
    /// </summary>
    public ScriptType ScriptType { get; set; } = ScriptType.ScriptedDefault;

    /// <summary>
    /// Defines how the test should be executed
    /// </summary>
    public TestExecutionType ExecutionType { get; set; }

    /// <summary>
    /// Defines the runner language that is needed
    /// </summary>
    public string? RunnerLanguage { get; set; }

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
    /// ID of test suite
    /// </summary>
    public long TestSuiteId { get; set; }

    /// <summary>
    /// ID of test suite folder
    /// </summary>
    public long? TestSuiteFolderId { get; set; }

    /// <summary>
    /// Flag that indicates that the test case requires classification by a background service
    /// 
    /// </summary>
    public bool ClassificationRequired { get; set; }

    /// <summary>
    /// Template flag
    /// </summary>
    public bool IsTemplate { get; set; }

    /// <summary>
    /// Test state
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Mapped state
    /// </summary>
    public MappedTestState? MappedState { get; set; }

    /// <summary>
    /// Variables for the test case
    /// </summary>
    [Column(TypeName = "jsonb")]
    public Dictionary<string, string>? TestParameters { get; set; }

    /// <summary>
    /// List of dependencies required to run this test case
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<TestCaseDependency>? Dependencies { get; set; }

    /// <summary>
    /// Assigned to (responsible test designer)
    /// </summary>
    public string? AssignedTo { get; set; }

    /// <summary>
    /// List of reviewers that are assigned to review this test
    /// </summary>
    [Column(TypeName = "jsonb")] 
    public List<AssignedReviewer>? ReviewAssignedTo { get; set; }

    /// <summary>
    /// Text embedding for semantic search and classification.
    /// Consists of the Name and descriptions
    /// </summary>
    [Column(TypeName = "vector(384)")]
    public Pgvector.Vector? Embedding { get; set; }

    // Navigation
    public TestSuiteFolder? TestSuiteFolder { get; set; }
    public virtual List<TestCaseField>? TestCaseFields { get; set; }
    public virtual List<TestStep>? TestSteps { get; set; }
    public virtual List<Comment>? Comments { get; set; }
    public virtual List<RequirementTestLink>? RequirementLinks { get; set; }
}
