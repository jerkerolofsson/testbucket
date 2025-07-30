using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestRuns;
internal class ExcludeArchivedTestRuns : FilterSpecification<TestRun>
{
    protected override Expression<Func<TestRun, bool>> GetExpression()
    {
        return x => x.Archived == false;
    }
}
