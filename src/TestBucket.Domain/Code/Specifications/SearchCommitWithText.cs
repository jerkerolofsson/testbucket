using System.Linq.Expressions;

using TestBucket.Domain.Code.Models;

using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Code.Specifications;
internal class SearchCommitWithText : FilterSpecification<Commit>
{
    private readonly string _name;

    public SearchCommitWithText(string name)
    {
        _name = name.ToLower();
    }

    protected override Expression<Func<Commit, bool>> GetExpression()
    {
        return x => (x.Message != null && x.Message.ToLower().Contains(_name)) || x.Sha.ToLower() == _name;
    }
}
