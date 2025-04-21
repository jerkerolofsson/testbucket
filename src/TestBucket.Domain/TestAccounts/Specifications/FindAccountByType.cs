using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.TestAccounts.Specifications;
internal class FindAccountByType : FilterSpecification<TestAccount>
{
    private readonly string _type;

    public FindAccountByType(string type)
    {
        _type = type;
    }

    protected override Expression<Func<TestAccount, bool>> GetExpression()
    {
        return x => x.Type == _type;
    }
}
