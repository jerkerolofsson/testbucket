using System.Linq.Expressions;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Code.Specifications;
internal class FilterComponentByName : FilterSpecification<Component>
{
    private readonly string _name;

    public FilterComponentByName(string name)
    {
        _name = name;
    }

    protected override Expression<Func<Component, bool>> GetExpression()
    {
        return x => x.Name == _name;
    }
}
