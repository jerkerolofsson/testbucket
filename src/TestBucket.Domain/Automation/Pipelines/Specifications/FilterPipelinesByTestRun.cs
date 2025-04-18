using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Automation.Pipelines.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Automation.Pipelines.Specifications;
internal class FilterPipelinesByTestRun : FilterSpecification<Pipeline>
{
    private readonly long _testRunId;

    public FilterPipelinesByTestRun(long testRunId)
    {
        _testRunId = testRunId;
    }

    protected override Expression<Func<Pipeline, bool>> GetExpression()
    {
        return x => x.TestRunId == _testRunId;
    }
}
