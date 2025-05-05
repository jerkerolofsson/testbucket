using System.Linq.Expressions;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications.Requirements;
 public class FilterRequirementByExternalId : FilterSpecification<Requirement>
{
    private readonly string _externalProvider;
    private readonly string _externalId;

    public FilterRequirementByExternalId(string externalProvider, string externalId)
    {
        _externalProvider = externalProvider;
        _externalId = externalId;
    }

    protected override Expression<Func<Requirement, bool>> GetExpression()
    {
        return x => x.ExternalProvider == _externalProvider && x.ExternalId == _externalId;
    }
}
