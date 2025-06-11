using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCaseRuns;

public class FilterTestCaseRunsByExternalDisplayId : FilterSpecification<TestCaseRun>
{
    private readonly string _id;

    public FilterTestCaseRunsByExternalDisplayId(string id)
    {
        _id = id;
    }

    protected override Expression<Func<TestCaseRun, bool>> GetExpression()
    {
        return x => x.TestCase != null && x.TestCase.ExternalDisplayId == _id;
    }
}
