using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Issues.Specifications;
internal class FindLocalIssueById : FilterSpecification<LocalIssue>
{
    private readonly long _id;

    public FindLocalIssueById(long issueId)
    {
        _id = issueId;
    }

    protected override Expression<Func<LocalIssue, bool>> GetExpression()
    {
        return x => (x.Id == _id);
    }
}
