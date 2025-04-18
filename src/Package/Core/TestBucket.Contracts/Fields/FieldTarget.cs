using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Fields;

/// <summary>
/// Defines what the field is applicable for
/// </summary>
[Flags]
public enum FieldTarget
{
    /// <summary>
    /// Test cases
    /// </summary>
    TestCase = 1,

    /// <summary>
    /// Test suites
    /// </summary>
    TestSuite = 2,

    /// <summary>
    /// Test run
    /// </summary>
    TestRun = 4,

    /// <summary>
    /// Project fields
    /// </summary>
    Project = 8,

    /// <summary>
    /// Test Case run
    /// </summary>
    TestCaseRun = 16,
}
