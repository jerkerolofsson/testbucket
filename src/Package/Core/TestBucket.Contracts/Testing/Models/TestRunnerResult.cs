using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Formats;

namespace TestBucket.Contracts.Testing.Models;

/// <summary>
/// Output result for automated test execution
/// </summary>
public class TestRunnerResult
{
    /// <summary>
    /// Was the execution completed
    /// </summary>
    public bool Completed { get; set; } = true;

    /// <summary>
    /// Error message should be set if Completed is false
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Serialized test result (e.g. a JUnitXml)
    /// </summary>
    public required string Result { get; set; }

    /// <summary>
    /// Test result format (of Result)
    /// </summary>
    public required TestResultFormat Format { get; set; }

    /// <summary>
    /// Standard out
    /// </summary>
    public string? StdOut { get; set; }

    /// <summary>
    /// Standard err
    /// </summary>
    public string? StdErr { get; set; }
}
