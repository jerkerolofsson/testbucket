using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Automation.Pipelines.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Automation.Pipelines;
public interface IPipelineRepository
{
    Task AddAsync(Pipeline pipeline);
    Task DeleteAsync(Pipeline pipeline);
    Task<Pipeline?> GetByIdAsync(long pipelineId);
    Task<IReadOnlyList<Pipeline>> GetPipelinesForTestRunAsync(FilterSpecification<Pipeline>[] filters, long testRunId);
    Task UpdateAsync(Pipeline pipeline);
}
