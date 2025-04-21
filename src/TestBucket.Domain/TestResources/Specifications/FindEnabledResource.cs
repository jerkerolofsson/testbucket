using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestResources.Models;

namespace TestBucket.Domain.TestResources.Specifications;
internal class FindEnabledResource : FilterSpecification<TestResource>
{
    protected override Expression<Func<TestResource, bool>> GetExpression()
    {
        return x => x.Enabled == true;
    }
}
