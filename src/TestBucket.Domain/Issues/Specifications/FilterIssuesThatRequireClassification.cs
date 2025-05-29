using System.Linq.Expressions;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

public class FilterIssuesThatRequireClassification : FilterSpecification<LocalIssue>
{
    public FilterIssuesThatRequireClassification()
    {
    }

    protected override Expression<Func<LocalIssue, bool>> GetExpression()
    {
        return x => x.ClassificationRequired;
    }
}
