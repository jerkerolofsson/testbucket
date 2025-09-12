namespace TestBucket.Domain.Code.CodeCoverage.Models;

public enum CodeCoverageGroupType
{
    /// <summary>
    /// Accumulated coverage for one run
    /// </summary>
    TestRun,

    /// <summary>
    /// The summary contains accumulated code coverage for a commit
    /// </summary>
    Commit,
}
