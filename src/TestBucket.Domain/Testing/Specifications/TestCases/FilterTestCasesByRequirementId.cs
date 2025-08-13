using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

public class FilterTestCasesByRequirementId : FilterSpecification<TestCase>
{
    private readonly long _requirementId;

    public FilterTestCasesByRequirementId(long requirementId)
    {
        _requirementId = requirementId;
    }

    protected override Expression<Func<TestCase, bool>> GetExpression()
    {
        return x => x.RequirementLinks != null && x.RequirementLinks.Any(link => link.RequirementId == _requirementId);
    }
}
