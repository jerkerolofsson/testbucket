using System.Security.Claims;

using Mediator;

using TestBucket.Domain.Automation.Models;

namespace TestBucket.Domain.Automation.IntegrationEvents
{
    /// <summary>
    /// Notification which is published once a pipeline has completed
    /// </summary>
    /// <param name="Principal"></param>
    /// <param name="Pipeline"></param>
    /// <param name="Job"></param>
    /// <param name="TestResultsArtifactsPattern">Glob pattern for test result files within the zip</param>
    /// <param name="ZipBytes"></param>
    public sealed record JobArtifactDownloaded(
        ClaimsPrincipal Principal, 
        Pipeline Pipeline, 
        PipelineJob Job, 
        string TestResultsArtifactsPattern,
        byte[] ZipBytes) : INotification;
}
