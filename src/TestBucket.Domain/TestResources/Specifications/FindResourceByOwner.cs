using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestResources.Models;

namespace TestBucket.Domain.TestResources.Specifications;
internal class FindResourceByOwner : FilterSpecification<TestResource>
{
    private readonly string _owner;

    public FindResourceByOwner(string owner)
    {
        _owner = owner;
    }

    protected override Expression<Func<TestResource, bool>> GetExpression()
    {
        return x => x.Owner == _owner;
    }
}
