using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Labels.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Labels.Specifications;
internal class FindByTitleContains : FilterSpecification<Label>
{
    private readonly string _text;

    public FindByTitleContains(string text)
    {
        _text = text;
    }

    protected override Expression<Func<Label, bool>> GetExpression()
    {
        return x => x.Title != null && x.Title.Contains(_text);
    }
}
