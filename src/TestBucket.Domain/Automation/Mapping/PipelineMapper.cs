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
    public static bool CopyTo(this PipelineDto src, Pipeline dest)
    {
        bool changed = false;

        if (dest.Status != src.Status)
        {
            dest.Status = src.Status;
            changed = true;
        }
        if (!string.IsNullOrEmpty(src.Error) && dest.StartError != src.Error)
        {
            changed = false;
            dest.StartError = src.Error;
        }

        return changed;
    }
}
