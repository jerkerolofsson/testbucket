using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestResources.Models;

namespace TestBucket.Domain.TestResources.Specifications;
internal class FindResourceByType : FilterSpecification<TestResource>
{
    private readonly string _type;

    public FindResourceByType(string type)
    {
        _type = type;
    }

    protected override Expression<Func<TestResource, bool>> GetExpression()
    {
        return x => x.Types != null && x.Types.Contains(_type);
    }
}
