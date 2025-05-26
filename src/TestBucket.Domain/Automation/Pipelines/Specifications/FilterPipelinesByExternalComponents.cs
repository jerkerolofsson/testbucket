using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Automation.Pipelines.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Automation.Pipelines.Specifications;
internal class FilterPipelinesByExternalComponents : FilterSpecification<Pipeline>
{
    private readonly string _ciCdSystemName;
    private readonly string _ciCdProjectId;
    private readonly string _ciCdPipelineId;

    public FilterPipelinesByExternalComponents(string ciCdSystemName, string ciCdProjectId, string ciCdPipelineId)
    {
        _ciCdSystemName = ciCdSystemName;
        _ciCdProjectId = ciCdProjectId;
        _ciCdPipelineId = ciCdPipelineId;
    }

    protected override Expression<Func<Pipeline, bool>> GetExpression()
    {
        return x => x.CiCdSystem == _ciCdSystemName && x.CiCdProjectId == _ciCdProjectId && x.CiCdPipelineIdentifier == _ciCdPipelineId;
    }
}
