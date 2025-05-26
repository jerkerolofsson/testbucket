using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Testing.Models;
public class TestExecutionContext
{
    /// <summary>
    /// A unique instance identifier.
    /// Resources are allocated to this guid, and are unlocked with this guid. 
    /// It is used to track jobs and pipelines
    /// </summary>
    public required string Guid { get; set; }

    /// <summary>
    /// Identifier for the test run
    /// </summary>
    public required long? TestRunId { get; set; }

    /// <summary>
    /// Identifier for the project
    /// </summary>
    public required long ProjectId { get; set; }

    /// <summary>
    /// Identifier for the team
    /// </summary>
    public required long TeamId { get; set; }

    /// <summary>
    /// Identifier for the team
    /// </summary>
    public long? TestSuiteId { get; set; }

    /// <summary>
    /// Assigned variables, e.g. from the selected environment
    /// </summary>
    public Dictionary<string, string> Variables { get; set; } = [];

    /// <summary>
    /// The test case ID (if a single test case )
    /// </summary>
    public long? TestCaseId { get; set; }

    /// <summary>
    /// ID if test environment to use
    /// </summary>
    public long? TestEnvironmentId { get; set; }

    /// <summary>
    /// Compilation errors
    /// </summary>
    public List<CompilerError> CompilerErrors { get; } = [];

    /// <summary>
    /// Branch/Tag
    /// </summary>
    public string? CiCdRef { get; set; }

    /// <summary>
    /// Workflow (Github actions)
    /// </summary>
    public string? CiCdWorkflow { get; set; } = "test.yml";

    /// <summary>
    /// This is the ID returned from the external CI/CD system when running
    /// 
    /// Note: For github the ID is not returned so when starting a new run we are polling for the latest new one
    /// </summary>
    public string? CiCdPipelineIdentifier { get; set; }

    /// <summary>
    /// The CI/CD system name (e.g. gitlab)
    /// </summary>
    public string? CiCdSystem { get; set; }

    /// <summary>
    /// Tenant ID
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// ID for configuration of external system
    /// </summary>
    public long? CiCdExternalSystemId { get; set; }

    /// <summary>
    /// Dependencies that are required
    /// </summary>
    public List<TestCaseDependency>? Dependencies { get; set; }

    /// <summary>
    /// Time when resources will be unlocked if not manually unlocked
    /// </summary>
    public DateTimeOffset ResourceExpiry { get; set; } = DateTimeOffset.MaxValue;
}
