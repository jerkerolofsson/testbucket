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

/// <summary>
/// Result of one executed test case
/// </summary>
[Table("testcaseruns")]
[Index(nameof(Created))]
[Index(nameof(Name))]
public class TestCaseRun
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
    /// Test result
    /// </summary>
    public string Result { get; set; } = "";

    /// <summary>
    /// ID of tenant
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// ID of project
    /// </summary>
    public long TestProjectId { get; set; }

    /// <summary>
    /// ID of test case
    /// </summary>
    public long TestCaseId { get; set; }

    /// <summary>
    /// ID of test run
    /// </summary>
    public long TestRunId { get; set; }

    // Navigation
    public TestRun? TestRun { get; set; }
    public TestCase? TestCase { get; set; }
    public Tenant? Tenant { get; set; }
    public TestProject? TestProject { get; set; }
}
