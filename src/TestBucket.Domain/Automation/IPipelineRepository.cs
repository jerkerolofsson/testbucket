using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Automation.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Automation;
public interface IPipelineRepository
{
    Task AddAsync(Pipeline pipeline);
    Task DeleteAsync(Pipeline pipeline);
    Task<Pipeline?> GetByIdAsync(long pipelineId);
    Task<IReadOnlyList<Pipeline>> GetPipelinesForTestRunAsync(FilterSpecification<Pipeline>[] filters, long testRunId);
    Task UpdateAsync(Pipeline pipeline);
}
