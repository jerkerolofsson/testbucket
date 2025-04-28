using System.Linq.Expressions;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Code.Specifications;
internal class FilterFeatureByName : FilterSpecification<Feature>
{
    private readonly string _name;

    public FilterFeatureByName(string name)
    {
        _name = name;
    }

    protected override Expression<Func<Feature, bool>> GetExpression()
    {
        return x => x.Name == _name;
    }
}
