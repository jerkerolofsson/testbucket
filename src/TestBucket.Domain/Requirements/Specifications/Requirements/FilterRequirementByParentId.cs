using System.Linq.Expressions;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications.Requirements;

public class FilterRequirementByParentId : FilterSpecification<Requirement>
{
    private readonly long _id;

    public FilterRequirementByParentId(long parentId)
    {
        _id = parentId;
    }

    protected override Expression<Func<Requirement, bool>> GetExpression()
    {
        return x => x.ParentRequirementId == _id;
    }
}
