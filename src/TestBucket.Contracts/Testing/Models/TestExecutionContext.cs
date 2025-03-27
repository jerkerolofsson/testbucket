using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Testing.Models;
public class TestExecutionContext
{
    /// <summary>
    /// Identifier for the  test run
    /// </summary>
    public required long TestRunId { get; set; }

    /// <summary>
    /// Identifier for a test case instance within a test run
    /// </summary>
    public required long TestCaseRunId { get; set; }

    /// <summary>
    /// Assigned variables, e.g. from the selected environment
    /// </summary>
    public Dictionary<string, string> Variables { get; set; } = [];
}
