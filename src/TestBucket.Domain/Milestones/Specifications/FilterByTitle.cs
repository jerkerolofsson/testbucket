using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Milestones.Specifications;
internal class FilterByTitle : FilterSpecification<Milestone>
{
    private readonly string _name;

    public FilterByTitle(string name)
    {
        _name = name;
    }

    protected override Expression<Func<Milestone, bool>> GetExpression()
    {
        return x => x.Title == _name;
    }
}
