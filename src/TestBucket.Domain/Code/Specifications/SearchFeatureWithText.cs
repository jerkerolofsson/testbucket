using System.Linq.Expressions;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Code.Specifications;
internal class SearchFeatureWithText : FilterSpecification<Feature>
{
    private readonly string _name;

    public SearchFeatureWithText(string name)
    {
        _name = name.ToLower();
    }

    protected override Expression<Func<Feature, bool>> GetExpression()
    {
        return x => x.Name.ToLower().Contains(_name);
    }
}
