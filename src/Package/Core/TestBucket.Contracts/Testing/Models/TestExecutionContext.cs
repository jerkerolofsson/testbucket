﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Testing.Models;
public class TestExecutionContext
{
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
    /// </summary>
    public string? CiCdPipelineIdentifier { get; set; }
    public string? CiCdSystem { get; set; }
    public string? TenantId { get; set; }
    public long? CiCdExternalSystemId { get; set; }
}
