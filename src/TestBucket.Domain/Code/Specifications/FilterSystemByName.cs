using System.Linq.Expressions;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Code.Specifications;
internal class FilterSystemByName : FilterSpecification<ProductSystem>
{
    private readonly string _name;

    public FilterSystemByName(string name)
    {
        _name = name;
    }

    protected override Expression<Func<ProductSystem, bool>> GetExpression()
    {
        return x => x.Name == _name;
    }
}
