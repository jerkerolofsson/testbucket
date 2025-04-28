using System.Linq.Expressions;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Code.Specifications;
internal class FilterLayerByName : FilterSpecification<Component>
{
    private readonly string _name;

    public FilterLayerByName(string name)
    {
        _name = name;
    }

    protected override Expression<Func<Component, bool>> GetExpression()
    {
        return x => x.Name == _name;
    }
}
