using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Issues.Specifications;
internal class FindLocalIssueByExternalSystemName : FilterSpecification<LocalIssue>
{
    private readonly string _text;

    public FindLocalIssueByExternalSystemName(string text)
    {
        _text = text;
    }

    protected override Expression<Func<LocalIssue, bool>> GetExpression()
    {
        return x => (x.ExternalSystemName == _text);
    }
}
