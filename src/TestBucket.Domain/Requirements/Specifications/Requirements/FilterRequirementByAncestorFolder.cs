using System.Linq.Expressions;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications.Requirements;

/// <summary>
/// Filters requirements that are descendants of the specified folder
/// </summary>
public class FilterRequirementByAncestorFolder : FilterSpecification<Requirement>
{
    private readonly long _id;

    public FilterRequirementByAncestorFolder(long parentId)
    {
        _id = parentId;
    }

    protected override Expression<Func<Requirement, bool>> GetExpression()
    {
        return x => x.PathIds != null && x.PathIds.Contains(_id);
    }
}
