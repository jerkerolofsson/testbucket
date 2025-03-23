using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCaseRuns;

public class FilterTestCaseRunsByTestSuiteFolder : FilterSpecification<TestCaseRun>
{
    private readonly long _id;

    public FilterTestCaseRunsByTestSuiteFolder(long id)
    {
        _id = id;
    }

    protected override Expression<Func<TestCaseRun, bool>> GetExpression()
    {
        return x => x.TestCase != null && x.TestCase.TestSuiteFolderId == _id;
    }
}
