using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using TestBucket.Data.Identity.Models;
using TestBucket.Data.Projects.Models;

namespace TestBucket.Data.Testing.Models;

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
    /// Timestamp when the test case was created
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Name of the test case
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Test case description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Folder path for the test case, separated with /
    /// </summary>
    public string Path { get; set; } = "";

    /// <summary>
    /// ID of tenant
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// ID of project
    /// </summary>
    public long TestProjectId { get; set; }

    /// <summary>
    /// ID of test suite
    /// </summary>
    public long TestSuiteId { get; set; }

    /// <summary>
    /// ID of test suite folder
    /// </summary>
    public long TestSuiteFolderId { get; set; }

    // Navigation
    public Tenant? Tenant { get; set; }
    public TestProject? TestProject { get; set; }
    public TestSuite? TestSuite { get; set; }
    public TestSuiteFolder? TestSuiteFolder { get; set; }
}
