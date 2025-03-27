using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Testing.Models;

/// <summary>
/// Output result for automated test execution
/// </summary>
public class TestRunnerResult
{
    /// <summary>
    /// The test result
    /// </summary>
    public required TestResult Result { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    public string? Message { get; set; }
}
