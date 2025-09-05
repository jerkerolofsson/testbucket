using System.Linq.Expressions;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Milestones.Specifications;
internal class FilterMilestoneByExternalId : FilterSpecification<Milestone>
{
    private readonly string _systemName;
    private readonly string _externalId;

    public FilterMilestoneByExternalId(string systemName, string externalId)
    {
        _systemName = systemName;
        _externalId = externalId;
    }

    protected override Expression<Func<Milestone, bool>> GetExpression()
    {
        return x => x.ExternalSystemName == _systemName && x.ExternalId == _externalId;
    }
}
