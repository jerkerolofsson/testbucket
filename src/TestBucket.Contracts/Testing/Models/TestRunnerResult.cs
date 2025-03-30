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
    /// Serialized test result (e.g. a JUnitXml)
    /// </summary>
    public required string Result { get; set; }

    /// <summary>
    /// Test result format (of Result)
    /// </summary>
    public required TestResultFormat Format { get; set; }
}
