using System.Security.Claims;

using Mediator;

using TestBucket.Domain.Automation.Pipelines.Models;
using TestBucket.Domain.Tenants.Models;

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
        string TenantId,
        long TestRunId,
        string TestResultsArtifactsPattern,
        byte[] ZipBytes) : INotification;
}
