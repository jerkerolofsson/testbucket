using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.Models;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Milestones.Specifications;
internal class FilterByState : FilterSpecification<Milestone>
{
    private readonly MilestoneState _state;

    public FilterByState(MilestoneState state)
    {
        _state = state;
    }

    protected override Expression<Func<Milestone, bool>> GetExpression()
    {
        return x => x.State == _state;
    }
}
