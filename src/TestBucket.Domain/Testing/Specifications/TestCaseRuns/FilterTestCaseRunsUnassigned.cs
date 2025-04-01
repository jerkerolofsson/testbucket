using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCaseRuns;

public class FilterTestCaseRunsUnassigned : FilterSpecification<TestCaseRun>
{
    public FilterTestCaseRunsUnassigned()
    {
    }

    protected override Expression<Func<TestCaseRun, bool>> GetExpression()
    {
        return x => x.AssignedToUserName == null;
    }
}
