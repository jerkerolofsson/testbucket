using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;

namespace TestBucket.Domain.Projects.Mapping
{
    internal static class ExternalSystemMapper
    {
        internal static ExternalSystem ToDbo(this ExternalSystemDto x)
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
                BaseUrl = x.BaseUrl,
                ExternalProjectId = x.ExternalProjectId,
                ReadOnly = x.ReadOnly,
                TestResultsArtifactsPattern = x.TestResultsArtifactsPattern,
            };
        }
        internal static ExternalSystemDto ToDto(this ExternalSystem x)
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
                BaseUrl = x.BaseUrl,
                ExternalProjectId = x.ExternalProjectId,
                ReadOnly = x.ReadOnly,
                TestResultsArtifactsPattern = x.TestResultsArtifactsPattern,
            };
        }
    }
}
