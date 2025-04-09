using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Automation;
public enum PipelineStatus
{
    Unknown = 0,
    Error = 1,
    Created = 2,
    Running = 3,
    Canceled = 4,
    Canceling,
    Pending,
    Completed,
    Preparing,
    Failed,
    NoBuild,
    Success,
    Skipped,
    WaitingForResource,
    Scheduled,
    Manual
}
