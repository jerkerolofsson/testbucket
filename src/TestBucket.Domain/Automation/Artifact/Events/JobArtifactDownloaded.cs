using Mediator;

namespace TestBucket.Domain.Automation.Artifact.Events
{
    /// <summary>
    /// Notification which is published once a pipeline has completed containing the artifact-zip
    /// downloaded from the CI/CD system
    /// </summary>
    /// <param name="Principal"></param>
    /// <param name="TestResultsArtifactsPattern">Glob pattern for test result files within the zip</param>
    /// <param name="ZipBytes"></param>
    public sealed record JobArtifactDownloaded(
        ClaimsPrincipal Principal, 
        long TestProjectId,
        long TestRunId,
        string? TestResultsArtifactsPattern,
        string? CoverageReportArtifactsPattern,
        byte[] ZipBytes) : INotification;
}
