namespace TestBucket.Domain.Automation.Artifact;
public class DefaultGlobPatterns
{
    public const string DefaultTestResultsArtifactsPattern = "**/*xunit*.xml;**/*junit*.xml;**/*nunit*.xml;**/*test*.xml;**/*.trx;**/*.ctrf";

    public const string DefaultCoverageReportArtifactsPattern = "**/*coverage*.xml";
}
