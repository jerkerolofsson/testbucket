using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Automation;
using TestBucket.Domain.Automation.Models;

namespace TestBucket.Domain.Automation.Mapping;
internal static class PipelineMapper
{
    public static void CopyTo(this PipelineDto src, Pipeline dest)
    {
        dest.Status = src.Status;
        if (!string.IsNullOrEmpty(src.Error))
        {
            dest.StartError = src.Error;
        }
    }
}
