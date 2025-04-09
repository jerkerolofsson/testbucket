using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Automation;
public class PipelineDto
{
    public bool IsCompleted => Status is PipelineStatus.Failed or PipelineStatus.Completed or PipelineStatus.Success or PipelineStatus.Error;

    public PipelineStatus Status { get; set; }
    public TimeSpan? Duration { get; set; }
    public string? Error { get; set; }
    public string? WebUrl { get; set; }
}
