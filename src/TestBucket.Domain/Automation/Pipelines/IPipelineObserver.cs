using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Automation.Pipelines.Models;

namespace TestBucket.Domain.Automation.Pipelines;
public interface IPipelineObserver
{
    Task OnPipelineUpdatedAsync(Pipeline pipeline);
}
