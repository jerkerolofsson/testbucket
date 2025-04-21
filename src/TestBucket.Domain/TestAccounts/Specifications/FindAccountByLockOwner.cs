using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestAccounts.Models;
using TestBucket.Domain.TestResources.Models;

namespace TestBucket.Domain.TestResources.Specifications;
internal class FindAccountByLockOwner : FilterSpecification<TestAccount>
{
    private readonly string _owner;

    public FindAccountByLockOwner(string owner)
    {
        _owner = owner;
    }

    protected override Expression<Func<TestAccount, bool>> GetExpression()
    {
        return x => x.LockOwner == _owner;
    }
}
