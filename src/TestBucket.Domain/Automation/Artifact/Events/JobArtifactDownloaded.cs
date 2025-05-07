using Mediator;

namespace TestBucket.Domain.Automation.Artifact.Events
{
    /// <summary>
    /// Notification which is published once a pipeline has completed
    /// </summary>
    /// <param name="Principal"></param>
    /// <param name="TestResultsArtifactsPattern">Glob pattern for test result files within the zip</param>
    /// <param name="ZipBytes"></param>
    public sealed record JobArtifactDownloaded(
        ClaimsPrincipal Principal, 
        long TestRunId,
        string TestResultsArtifactsPattern,
        byte[] ZipBytes) : INotification;
}
