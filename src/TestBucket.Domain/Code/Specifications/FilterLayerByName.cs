using System.Linq.Expressions;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Code.Specifications;
internal class FilterLayerByName : FilterSpecification<ArchitecturalLayer>
{
    private readonly string _name;

    public FilterLayerByName(string name)
    {
        _name = name;
    }

    protected override Expression<Func<ArchitecturalLayer, bool>> GetExpression()
    {
        return x => x.Name == _name;
    }
}
