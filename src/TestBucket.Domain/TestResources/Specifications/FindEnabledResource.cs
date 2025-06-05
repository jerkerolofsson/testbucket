using System.Linq.Expressions;

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
