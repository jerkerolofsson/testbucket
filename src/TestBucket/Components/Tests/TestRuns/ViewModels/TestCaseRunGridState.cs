using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Components.Tests.TestRuns.ViewModels;

public record class TestCaseRunGridState(SearchTestCaseRunQuery Query, PagedResult<TestCaseRun> Data)
{
    public TestCaseRun? SelectedTestCaseRun { get; set; }
}
