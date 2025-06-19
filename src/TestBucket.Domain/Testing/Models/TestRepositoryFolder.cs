using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Testing.Models;
/// <summary>
/// This is a root folder in the test repository.
/// It can contain child folders or TestSuite's
/// </summary>
[Table("testrepository__folders")]
public class TestRepositoryFolder : TestEntity
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

    public TestRepositoryFolder? Parent { get; set; }
    public virtual List<TestRepositoryFolder>? ChildFolders { get; set; }
    public virtual List<TestSuite>? TestSuites { get; set; }

}
