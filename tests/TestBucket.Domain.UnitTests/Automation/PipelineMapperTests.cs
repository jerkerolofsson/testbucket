using System;
using System.Collections.Generic;
using TestBucket.Contracts.Automation;
using TestBucket.Domain.Automation.Pipelines.Models;
using TestBucket.Domain.Automation.Mapping;
using Xunit;

namespace TestBucket.Domain.UnitTests.Automation;

/// <summary>
/// Unit tests for the <see cref="PipelineMapper"/> class, verifying mapping logic for Pipeline and PipelineJob objects.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Component("Automation")]
public class PipelineMapperTests
{
    /// <summary>
    /// Verifies that PipelineJobDto.CopyTo updates all relevant fields in the destination
    /// and returns <c>true</c> when changes are made.
    /// </summary>
    [Fact]
    public void CopyTo_PipelineJobDto_UpdatesDestinationAndReturnsChanged()
    {
        var src = new PipelineJobDto
        {
            Status = PipelineJobStatus.Success,
            HasArtifacts = true,
            FinishedAt = DateTimeOffset.UtcNow,
            StartedAt = DateTimeOffset.UtcNow.AddMinutes(-10),
            Stage = "build",
            Duration = TimeSpan.FromMinutes(10),
            QueuedDuration = TimeSpan.FromMinutes(1),
            AllowFailure = false,
            Coverage = 95.5,
            FailureReason = "None",
            WebUrl = "http://job",
            Name = "Job1",
            TagList = new[] { "tag1", "tag2" },
            CiCdJobIdentifier = "job-123"
        };
        var dest = new PipelineJob
        {
            Id = 1,
            PipelineId = 1,
            Status = PipelineJobStatus.Failed,
            HasArtifacts = false,
            FinishedAt = null,
            StartedAt = null,
            Stage = "test",
            Duration = TimeSpan.FromMinutes(5),
            QueuedDuration = TimeSpan.FromMinutes(2),
            AllowFailure = true,
            Coverage = 80.0,
            FailureReason = "Error",
            WebUrl = "http://oldjob",
            Name = "OldJob",
            TagList = null,
            CiCdJobIdentifier = "job-123"
        };

        var changed = src.CopyTo(dest);

        Assert.True(changed);
        Assert.Equal(src.Status, dest.Status);
        Assert.Equal(src.HasArtifacts, dest.HasArtifacts);
        Assert.Equal(src.FinishedAt, dest.FinishedAt);
        Assert.Equal(src.StartedAt, dest.StartedAt);
        Assert.Equal(src.Stage, dest.Stage);
        Assert.Equal(src.Duration, dest.Duration);
        Assert.Equal(src.QueuedDuration, dest.QueuedDuration);
        Assert.Equal(src.AllowFailure, dest.AllowFailure);
        Assert.Equal(src.Coverage, dest.Coverage);
        Assert.Equal(src.FailureReason, dest.FailureReason);
        Assert.Equal(src.WebUrl, dest.WebUrl);
        Assert.Equal(src.Name, dest.Name);
        Assert.Equal(src.TagList, dest.TagList);
    }

    /// <summary>
    /// Verifies that PipelineDto.CopyTo updates all relevant fields in the destination,
    /// adds new jobs, and returns <c>true</c> when changes are made.
    /// </summary>
    [Fact]
    public void CopyTo_PipelineDto_UpdatesDestinationAndReturnsChanged()
    {
        var srcJob = new PipelineJobDto
        {
            CiCdJobIdentifier = "job-1",
            HasArtifacts = true,
            Status = PipelineJobStatus.Success
        };
        var src = new PipelineDto
        {
            Duration = TimeSpan.FromMinutes(15),
            WebUrl = "http://pipeline",
            Status = PipelineStatus.Success,
            HeadCommit = "abc123",
            DisplayTitle = "Pipeline 1",
            Error = "Some error",
            Jobs = new List<PipelineJobDto> { srcJob }
        };
        var dest = new Pipeline
        {
            Id = 1,
            Duration = TimeSpan.FromMinutes(10),
            WebUrl = "http://oldpipeline",
            Status = PipelineStatus.Failed,
            HeadCommit = "oldcommit",
            DisplayTitle = "Old Pipeline",
            StartError = null,
            PipelineJobs = new List<PipelineJob>()
        };

        var changed = src.CopyTo(dest);

        Assert.True(changed);
        Assert.Equal(src.Duration, dest.Duration);
        Assert.Equal(src.WebUrl, dest.WebUrl);
        Assert.Equal(src.Status, dest.Status);
        Assert.Equal(src.HeadCommit, dest.HeadCommit);
        Assert.Equal(src.DisplayTitle, dest.DisplayTitle);
        Assert.Equal(src.Error, dest.StartError);
        Assert.Single(dest.PipelineJobs);
        Assert.Equal(srcJob.CiCdJobIdentifier, dest.PipelineJobs[0].CiCdJobIdentifier);
        Assert.Equal(srcJob.HasArtifacts, dest.PipelineJobs[0].HasArtifacts);
        Assert.Equal(srcJob.Status, dest.PipelineJobs[0].Status);
    }
}