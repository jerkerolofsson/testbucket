namespace TestBucket.Contracts.Testing.States;

/// <summary>
/// States for test cases and test case runs
/// </summary>
public enum MappedTestState
{
    /// <summary>
    /// Test not started
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// Assigned to a user for work
    /// </summary>
    Assigned = 1,

    /// <summary>
    /// Test is ongoing / test case in draft state
    /// </summary>
    Ongoing = 2,

    /// <summary>
    /// Test is completed
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Test case in review
    /// </summary>
    Review = 4,

    Draft = 5,

    /// <summary>
    /// Other, used defined with no mapping to internal state
    /// </summary>
    Other = 100,
}
