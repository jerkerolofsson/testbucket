using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

public class FilterTestCasesByReviewAssignedTo : FilterSpecification<TestCase>
{
    private readonly string _query;

    internal string User => _query;

    public FilterTestCasesByReviewAssignedTo(string user)
    {
        _query = user;
    }

    protected override Expression<Func<TestCase, bool>> GetExpression()
    {
        return x => x.ReviewAssignedTo != null && x.ReviewAssignedTo.Any(x=>x.UserName == _query);
    }
}
