using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Issues.Specifications;

/// <summary>
/// Finds issues created from a specific extension configuration
/// </summary>
internal class FindLocalIssueByType : FilterSpecification<LocalIssue>
{
    private readonly string _id;

    public FindLocalIssueByType(string id)
    {
        _id = id.ToLower();
    }

    protected override Expression<Func<LocalIssue, bool>> GetExpression()
    {
        return x => x.IssueType != null && x.IssueType.ToLower() == _id;
    }
}
