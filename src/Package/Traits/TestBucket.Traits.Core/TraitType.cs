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
    Custom = 0,

    /// <summary>
    /// ID from original source
    /// </summary>
    TestId = 1,

    /// <summary>
    /// Name
    /// </summary>
    Name = 2,

    /// <summary>
    /// Test case result
    /// </summary>
    TestResult = 3,

    /// <summary>
    /// Test case run state (Completed, Started, Assigned..)
    /// </summary>
    TestState = 4,

    /// <summary>
    /// Test activity or applicable for test activity
    /// </summary>
    TestActivity = 5,

    /// <summary>
    /// Test case priority
    /// </summary>
    TestPriority = 6,

    /// <summary>
    /// Description of the test case
    /// </summary>
    TestDescription = 7,

    /// <summary>
    /// Category of test (Integration, Unit, E2E)
    /// </summary>
    TestCategory = 8,

    /// <summary>
    /// Filename / path of test
    /// </summary>
    TestFilePath = 9,

    /// <summary>
    /// For browser tests, name of the browser used
    /// </summary>
    Browser = 10,

    // Failures

    /// <summary>
    /// Type of failure
    /// </summary>
    FailureType = 11,

    /// <summary>
    /// Failure message
    /// </summary>
    FailureMessage = 12,

    /// <summary>
    /// Stacktrace / call stack
    /// </summary>
    CallStack = 13,

    /// <summary>
    /// Assembly implementing the automated test
    /// </summary>
    Assembly = 14,

    /// <summary>
    /// Module implementing the automated test (e.g. python file)
    /// </summary>
    Module = 15,

    /// <summary>
    /// Class implementing the automated test
    /// </summary>
    ClassName = 16,

    /// <summary>
    /// Method implementing the automated test
    /// </summary>
    Method = 17,

    /// <summary>
    /// Line number of failure
    /// </summary>
    Line = 18,

    /// <summary>
    /// Duration to run the test
    /// </summary>
    Duration = 19,

    // Target

    /// <summary>
    /// Commit hash
    /// </summary>
    Commit = 20,

    /// <summary>
    /// SW Version for SUT
    /// </summary>
    SoftwareVersion = 21,

    /// <summary>
    /// Project milestone
    /// </summary>
    Milestone =22,

    /// <summary>
    /// Project release
    /// </summary>
    Release = 23,

    /// <summary>
    /// HW Version for SUT
    /// </summary>
    HardwareVersion = 24,

    /// <summary>
    /// Test created time
    /// </summary>
    CreatedTime = 25,

    /// <summary>
    /// Test started time
    /// </summary>
    StartedTime = 26,

    /// <summary>
    /// Test ended time
    /// </summary>
    EndedTime = 27,

    /// <summary>
    /// stdout
    /// </summary>
    SystemOut = 28,

    /// <summary>
    /// stderr
    /// </summary>
    SystemErr = 29,

    /// <summary>
    /// Generic project name
    /// </summary>
    Project = 30,

    /// <summary>
    /// Generic version
    /// </summary>
    Version = 31,

    /// <summary>
    /// 
    /// </summary>
    Ci = 32,

    /// <summary>
    /// Collection name
    /// </summary>
    CollectionName = 33,

    /// <summary>
    /// Test environment ID or name
    /// </summary>
    Environment = 34,

    /// <summary>
    /// Custom tag
    /// </summary>
    Tag = 35,

    /// <summary>
    /// Quality characteristic
    /// </summary>
    QualityCharacteristic = 36,

    /// <summary>
    /// Test area
    /// </summary>
    Area = 37,

    /// <summary>
    /// Component
    /// </summary>
    Component = 38,

    /// <summary>
    /// User running the tests
    /// </summary>
    InstanceUserName = 39,

    /// <summary>
    /// Specific name only for a run. 
    /// 
    /// This may be used in some reporting formats where the test name is the same, but data driven arguments are added to the run name
    /// </summary>
    InstanceName = 40,

    /// <summary>
    /// ID for a specific run / instance
    /// </summary>
    InstanceId = 41,

    /// <summary>
    /// Computer name
    /// </summary>
    Computer = 42,

    /// <summary>
    /// Product feature
    /// </summary>
    Feature = 43,

    /// <summary>
    /// Team
    /// </summary>
    Team = 44,

    /// <summary>
    /// A covered requirement
    /// </summary>
    CoveredRequirement = 45,

    /// <summary>
    /// A covered issue/bug/ticket
    /// </summary>
    CoveredIssue = 46,

    /// <summary>
    /// The test/requirement is approved
    /// </summary>
    Approved = 48,

    /// <summary>
    /// A label
    /// </summary>
    Label = 49,

    /// <summary>
    /// Branch / head ref
    /// </summary>
    Branch = 50,

    /// <summary>
    /// Version of browser
    /// </summary>
    BrowserVersion = 51,

    /// <summary>
    /// SW variant
    /// </summary>
    SoftwareVariant = 52,

    /// <summary>
    /// OS for the system under test, could be windows, android etc
    /// </summary>
    SutOperatingSystemName = 53,
    SutOperatingSystemVersion = 54,

    /// <summary>
    /// Platform for the system under test, could be qualcom snapdragon or similar
    /// </summary>
    SutPlatformName = 55,
    SutPlatformVersion = 56,

    /// <summary>
    /// Device manufacturer
    /// </summary>
    SutManufacturer = 57,

    /// <summary>
    /// Device model
    /// </summary>
    SutModel = 58,

    /// <summary>
    /// Geographical longitude of SUT
    /// </summary>
    SutLocationLongitude = 59,

    /// <summary>
    /// Geographical latitude of SUT
    /// </summary>
    SutLocationLatitude = 60,

    // Test runner / test execution environment

    RunnerOperatingSystemName = 200,
    RunnerOperatingSystemVersion = 201,

    // Work items etc
    CoveredWorkItem = 300,
    CoveredUserStory = 301
}
