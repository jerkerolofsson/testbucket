
namespace TestBucket.Domain.Testing.Models;
/// <summary>
/// This is a root folder in the test lab.
/// It can contain child folders or TestRun's
/// </summary>
[Table("testlab__folders")]
public class TestLabFolder : TestEntity
{
    /// <summary>
    /// Database ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Name of the test case
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Path of the folder, separated by /
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// IDs for the path, for navigation
    /// </summary>
    public long[]? PathIds { get; set; }

    /// <summary>
    /// SVG icon
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// HTML color
    /// </summary>
    public string? Color { get; set; }

    // Navigation

    /// <summary>
    /// Foreign key to parent folder. 
    /// If null it is a root folder
    /// </summary>
    public long? ParentId { get; set; }


    // Navigation

    public TestLabFolder? Parent { get; set; }
    public virtual List<TestLabFolder>? ChildFolders { get; set; }
    public virtual List<TestRun>? TestRuns { get; set; }

}
