using System.Linq.Expressions;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Code.Specifications;
internal class SearchComponentWithText : FilterSpecification<Component>
{
    private readonly string _name;

    public SearchComponentWithText(string name)
    {
        _name = name.ToLower();
    }

    protected override Expression<Func<Component, bool>> GetExpression()
    {
        return x => x.Name.ToLower().Contains(_name);
    }
}
