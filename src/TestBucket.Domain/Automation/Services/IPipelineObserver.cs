using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Automation.Models;

namespace TestBucket.Domain.Automation.Services;
public interface IPipelineObserver
{
    Task OnPipelineUpdatedAsync(Pipeline pipeline);
}
