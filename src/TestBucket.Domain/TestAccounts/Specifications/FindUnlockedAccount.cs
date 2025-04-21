using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.TestAccounts.Specifications;
internal class FindUnlockedAccount : FilterSpecification<TestAccount>
{
    protected override Expression<Func<TestAccount, bool>> GetExpression()
    {
        return x => x.Locked == false;
    }
}
