using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.TestResources.Specifications;
internal class FindLockedAccount : FilterSpecification<TestAccount>
{
    protected override Expression<Func<TestAccount, bool>> GetExpression()
    {
        return x => x.Locked == true;
    }
}
