using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;

using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.TestAccounts.Specifications;
internal class FindAccountBySearch : FilterSpecification<TestAccount>
{
    private readonly string _text;

    public FindAccountBySearch(string type)
    {
        _text = type.ToLower();
    }

    protected override Expression<Func<TestAccount, bool>> GetExpression()
    {
        return x => x.Name.ToLower().Contains(_text) || x.Type.ToLower().Contains(_text);
    }
}
