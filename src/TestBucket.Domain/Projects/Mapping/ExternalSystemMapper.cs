using TestBucket.Contracts.Integrations;

namespace TestBucket.Domain.Projects.Mapping
{
    public static class ExternalSystemMapper
    {
        public static ExternalSystem ToDbo(this ExternalSystemDto x)
        {
            return new ExternalSystem
            {
                Id = x.Id,
                Enabled = x.Enabled,
                EnabledCapabilities = x.EnabledCapabilities,
                SupportedCapabilities = x.SupportedCapabilities,
                Name = x.Name,
                Provider = x.Provider,
                AccessToken = x.AccessToken,
                ApiKey = x.ApiKey,
                ClientId = x.ClientId,
                ClientSecret = x.ClientSecret,
                BaseUrl = x.BaseUrl,
                ExternalProjectId = x.ExternalProjectId,
                ReadOnly = x.ReadOnly,
                TestResultsArtifactsPattern = x.TestResultsArtifactsPattern,
                CoverageReportArtifactsPattern = x.CoverageReportArtifactsPattern,
            };
        }
        public static ExternalSystemDto ToDto(this ExternalSystem x)
        {
            return new ExternalSystemDto
            {
                Id = x.Id,
                Enabled = x.Enabled,
                EnabledCapabilities = x.EnabledCapabilities,
                SupportedCapabilities = x.SupportedCapabilities,
                Name = x.Name,
                Provider = x.Provider,
                AccessToken = x.AccessToken,
                ApiKey = x.ApiKey,
                ClientId = x.ClientId,
                ClientSecret = x.ClientSecret,
                BaseUrl = x.BaseUrl,
                ExternalProjectId = x.ExternalProjectId,
                ReadOnly = x.ReadOnly,
                TestResultsArtifactsPattern = x.TestResultsArtifactsPattern,
                CoverageReportArtifactsPattern = x.CoverageReportArtifactsPattern,
            };
        }
    }
}
