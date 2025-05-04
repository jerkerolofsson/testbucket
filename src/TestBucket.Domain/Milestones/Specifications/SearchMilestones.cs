using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Milestones.Specifications;
internal class SearchMilestones : FilterSpecification<Milestone>
{
    private readonly string _text;

    public SearchMilestones(string text)
    {
        _text = text;
    }

    protected override Expression<Func<Milestone, bool>> GetExpression()
    {
        return x => x.Title != null && x.Title.Contains(_text);
    }
}
