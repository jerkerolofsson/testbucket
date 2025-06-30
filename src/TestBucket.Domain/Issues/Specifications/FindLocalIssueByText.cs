using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Issues.Specifications;
internal class FindLocalIssueByText : FilterSpecification<LocalIssue>
{
    private readonly string _text;

    public FindLocalIssueByText(string text)
    {
        _text = text.ToLower();
    }

    protected override Expression<Func<LocalIssue, bool>> GetExpression()
    {
        return x => (x.Title != null && x.Title.ToLower().Contains(_text)) ||
        (x.ExternalDisplayId != null && x.ExternalDisplayId.ToLower().Contains(_text)) || 
        (x.Description != null && x.Description.ToLower().Contains(_text));
    }
}
