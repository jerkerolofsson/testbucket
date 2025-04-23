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
    /// Test cases (test case runs inherit these)
    /// </summary>
    TestCase = 1,

    /// <summary>
    /// Test suites (test suite folders and test cases inherit these)
    /// </summary>
    TestSuite = 2,

    /// <summary>
    /// Test run (test case runs inherit these)
    /// </summary>
    TestRun = 4,

    /// <summary>
    /// Project fields (not used)
    /// </summary>
    Project = 8,

    /// <summary>
    /// Test Case run
    /// </summary>
    TestCaseRun = 16,

    /// <summary>
    /// Test suite folder (test case inherits these)
    /// </summary>
    TestSuiteFolder = 32,

    /// <summary>
    /// A requirement
    /// </summary>
    Requirement = 128,

    /// <summary>
    /// A requirement specification (requirements inherit these)
    /// </summary>
    RequirementSpecificationFolder = 256,

    /// <summary>
    /// A requirement specification (requirements and folders inherit these)
    /// </summary>
    RequirementSpecification = 512,

    /// <summary>
    /// Ticket / issue
    /// </summary>
    Issue = 1024,
}
