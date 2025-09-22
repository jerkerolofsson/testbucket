using System.Linq.Expressions;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Milestones.Specifications;
internal class FilterMilestonesByCurrent : FilterSpecification<Milestone>
{
    private readonly DateTimeOffset _now;

    public FilterMilestonesByCurrent(DateTimeOffset now)
    {
        _now = now;
    }

    protected override Expression<Func<Milestone, bool>> GetExpression()
    {
        return x => x.State == Contracts.Issues.Models.MilestoneState.Open &&
            (x.EndDate != null && x.EndDate >= _now) &&
        (x.StartDate != null || x.StartDate <= _now);
        }
}
