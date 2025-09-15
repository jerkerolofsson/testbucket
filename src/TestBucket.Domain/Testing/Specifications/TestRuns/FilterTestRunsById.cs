using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestRuns;
internal class FilterTestRunsById : FilterSpecification<TestRun>
{
    private readonly long _id;

    public FilterTestRunsById(long id)
    {
        _id = id;
    }

    protected override Expression<Func<TestRun, bool>> GetExpression()
    {
        return x => x.Id == _id;
    }
}
