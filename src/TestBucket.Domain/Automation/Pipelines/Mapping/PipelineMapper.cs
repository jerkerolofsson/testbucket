using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Automation;
using TestBucket.Domain.Automation.Pipelines.Models;

namespace TestBucket.Domain.Automation.Mapping;
internal static class PipelineMapper
{
    public static bool CopyTo(this PipelineJobDto src, PipelineJob dest)
    {
        bool changed = false;

        if (dest.Status != src.Status)
        {
            dest.Status = src.Status;
            changed = true;
        }
        if (dest.HasArtifacts != src.HasArtifacts)
        {
            dest.HasArtifacts = src.HasArtifacts;
            changed = true;
        }
        if (dest.FinishedAt != src.FinishedAt)
        {
            dest.FinishedAt = src.FinishedAt;
            changed = true;
        }
        if (dest.StartedAt != src.StartedAt)
        {
            dest.StartedAt = src.StartedAt;
            changed = true;
        }
        if (dest.Stage != src.Stage)
        {
            dest.Stage = src.Stage;
            changed = true;
        }
        if (dest.Duration != src.Duration)
        {
            dest.Duration = src.Duration;
            changed = true;
        }
        if (dest.QueuedDuration != src.QueuedDuration)
        {
            dest.QueuedDuration = src.QueuedDuration;
            changed = true;
        }
        if (dest.AllowFailure != src.AllowFailure)
        {
            dest.AllowFailure = src.AllowFailure;
            changed = true;
        }
        if (dest.Coverage != src.Coverage)
        {
            dest.Coverage = src.Coverage;
            changed = true;
        }
        if (dest.FailureReason != src.FailureReason)
        {
            dest.FailureReason = src.FailureReason;
            changed = true;
        }
        if (dest.WebUrl != src.WebUrl)
        {
            dest.WebUrl = src.WebUrl;
            changed = true;
        }
        if (dest.Name != src.Name)
        {
            dest.Name = src.Name;
            changed = true;
        }
        if (dest.TagList is null && src.TagList is not null)
        {
            dest.TagList = src.TagList;
            changed = true;
        }


        return changed;
    }

    public static bool CopyTo(this PipelineDto src, Pipeline dest)
    {
        bool changed = false;
        if (dest.Duration != src.Duration)
        {
            dest.Duration = src.Duration;
            changed = true;
        }
        if (dest.WebUrl != src.WebUrl)
        {
            dest.WebUrl = src.WebUrl;
            changed = true;
        }
        if (dest.Status != src.Status)
        {
            dest.Status = src.Status;
            changed = true;
        }
        if (dest.HeadCommit != src.HeadCommit)
        {
            dest.HeadCommit = src.HeadCommit;
            changed = true;
        }
        if (dest.DisplayTitle != src.DisplayTitle)
        {
            dest.DisplayTitle = src.DisplayTitle;
            changed = true;
        }
        if (!string.IsNullOrEmpty(src.Error) && dest.StartError != src.Error)
        {
            changed = false;
            dest.StartError = src.Error;
        }

        if (dest.PipelineJobs is not null)
        {
            foreach (var srcJob in src.Jobs)
            {
                var destJob = dest.PipelineJobs.Where(x=>x.CiCdJobIdentifier == srcJob.CiCdJobIdentifier).FirstOrDefault();
                if(destJob is null)
                {
                    changed = true;
                    destJob = new PipelineJob 
                    { 
                        Id = 0, 
                        PipelineId = dest.Id, 
                        CiCdJobIdentifier = srcJob.CiCdJobIdentifier, 
                        Created =  DateTimeOffset.UtcNow,
                        Modified = DateTimeOffset.UtcNow,
                        HasArtifacts = srcJob.HasArtifacts,
                        Status = srcJob.Status,
                    };
                    srcJob.CopyTo(destJob);
                    dest.PipelineJobs.Add(destJob);
                }
                else
                {
                    var jobChanged = srcJob.CopyTo(destJob);
                    if(jobChanged)
                    {
                        destJob.Modified = DateTimeOffset.UtcNow;
                        changed = true;
                    }
                }
            }
        }

        return changed;
    }
}
