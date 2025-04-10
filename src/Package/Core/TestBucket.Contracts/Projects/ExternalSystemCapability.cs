using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Projects
{
    [Flags]
    public enum ExternalSystemCapability
    {
        None = 0,

        CreatePipeline  = 0x01,
        GetPipelines    = 0x02,

        GetIssues       = 0x04,
        CreateIssues    = 0x08,

        GetReleases     = 0x20,
        GetMilestones   = 0x40,
    }
}
