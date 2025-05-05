using System.Linq.Expressions;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications.Requirements;
 public class FilterRequirementByRootId : FilterSpecification<Requirement>
{
    private readonly long _id;

    public FilterRequirementByRootId(long rootId)
    {
        _id = rootId;
    }

    protected override Expression<Func<Requirement, bool>> GetExpression()
    {
        return x => x.RootRequirementId == _id;
    }
}
