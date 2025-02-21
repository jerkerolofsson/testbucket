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

[Table("testsuite__folders")]
[Index(nameof(TenantId), nameof(Created))]
public class TestSuiteFolder
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
    /// ID of tenant
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// ID of project
    /// </summary>
    public long TestProjectId { get; set; }

    // Navigation

    public long? ParentId { get; set; }
    public TestSuiteFolder? Parent { get; set; }
    public IEnumerable<TestCase>? TestCases { get; set; }
    public Tenant? Tenant { get; set; }
    public TestProject? TestProject { get; set; }
}
