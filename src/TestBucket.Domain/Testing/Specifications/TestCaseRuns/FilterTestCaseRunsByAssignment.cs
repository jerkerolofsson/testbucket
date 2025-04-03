using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCaseRuns;

public class FilterTestCaseRunsByAssignment : FilterSpecification<TestCaseRun>
{
    private readonly string _userName;

    public FilterTestCaseRunsByAssignment(string userName)
    {
        _userName = userName;
    }

    protected override Expression<Func<TestCaseRun, bool>> GetExpression()
    {
        return x => x.AssignedToUserName == _userName;
    }
}
