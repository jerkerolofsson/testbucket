using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Environments.Models;
using TestBucket.Domain.Fields.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Environments.Specifications
{
    public class FilterEnvironmentByDefault : FilterSpecification<TestEnvironment>
    {
        protected override Expression<Func<TestEnvironment, bool>> GetExpression()
        {
            return x => x.Default;
        }
    }
}
