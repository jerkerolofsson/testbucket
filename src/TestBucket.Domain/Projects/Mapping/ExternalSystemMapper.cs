using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Projects;

namespace TestBucket.Domain.Projects.Mapping
{
    internal static class ExternalSystemMapper
    {
        internal static ExternalSystemDto ToDto(this ExternalSystem x)
        {
            return new ExternalSystemDto
            {
                Enabled = x.Enabled,
                EnabledCapabilities = x.EnabledCapabilities,
                SupportedCapabilities = x.SupportedCapabilities,
                Name = x.Name,
                AccessToken = x.AccessToken,
                BaseUrl = x.BaseUrl,
                ExternalProjectId = x.ExternalProjectId,
                ReadOnly = x.ReadOnly,
                TestResultsArtifactsPattern = x.TestResultsArtifactsPattern,
            };
        }
    }
}
