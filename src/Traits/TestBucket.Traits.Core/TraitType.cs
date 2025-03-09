namespace TestBucket.Traits.Core;

/// <summary>
/// This is an exhaustive list of all known traits, including traits that may be managed by some formats in the results
/// XML file (for example Name, TestResult)
/// </summary>
public enum TraitType
{
    /// <summary>
    /// Special representation of a trait that does not have a conversion from a string to a custom type
    /// </summary>
    Custom,

    /// <summary>
    /// ID from original source
    /// </summary>
    TestId,

    /// <summary>
    /// Name
    /// </summary>
    Name,

    /// <summary>
    /// Test case result
    /// </summary>
    TestResult,

    /// <summary>
    /// Test case run state (Completed, Started, Assigned..)
    /// </summary>
    TestState,

    /// <summary>
    /// Test case priority
    /// </summary>
    TestPriority,

    /// <summary>
    /// Description of the test case
    /// </summary>
    TestDescription,

    /// <summary>
    /// Category of test (Integration, Unit, E2E)
    /// </summary>
    TestCategory,

    /// <summary>
    /// For browser tests, name of the browser used
    /// </summary>
    Browser,

    // Failures

    /// <summary>
    /// Type of failure
    /// </summary>
    FailureType,

    /// <summary>
    /// Failure message
    /// </summary>
    FailureMessage,

    /// <summary>
    /// Stacktrace / call stack
    /// </summary>
    CallStack,

    /// <summary>
    /// Assembly implementing the automated test
    /// </summary>
    Assembly,

    /// <summary>
    /// Module implementing the automated test (e.g. python file)
    /// </summary>
    Module,

    /// <summary>
    /// Class implementing the automated test
    /// </summary>
    ClassName,

    /// <summary>
    /// Method implementing the automated test
    /// </summary>
    Method,

    /// <summary>
    /// Line number of failure
    /// </summary>
    Line,

    /// <summary>
    /// Duration to run the test
    /// </summary>
    Duration,

    // Target

    /// <summary>
    /// Commit hash
    /// </summary>
    Commit,

    /// <summary>
    /// SW Version
    /// </summary>
    SoftwareVersion,

    /// <summary>
    /// HW Version
    /// </summary>
    HardwareVersion,

    CreatedTime,
    StartedTime,
    EndedTime,

    /// <summary>
    /// stdout
    /// </summary>
    SystemOut,

    /// <summary>
    /// stderr
    /// </summary>
    SystemErr,

    Project,

    Version,

    /// <summary>
    /// 
    /// </summary>
    Ci,

    /// <summary>
    /// Collection name
    /// </summary>
    CollectionName,

    Environment,
    Tag,
    QualityCharacteristic,
    Area,
    Component,
}
