using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Projects;

namespace TestBucket.Gitlab
{
    class GitlabExtension : IExtension
    {
        public string SystemName => ExtensionConstants.SystemName;

        public ExternalSystemCapability SupportedCapabilities => 
            ExternalSystemCapability.CreatePipeline | 
            ExternalSystemCapability.GetPipelines | 
            ExternalSystemCapability.GetReleases |
            ExternalSystemCapability.GetIssues |
            ExternalSystemCapability.GetMilestones;

        public string FriendlyName => ExtensionConstants.FriendlyName;

        public string Description => ExtensionConstants.Description;

        public string Version => ExtensionConstants.Version;

        public string? Icon => ExtensionConstants.Icon;


        public string DefaultBaseUrl => "https://gitlab.com/";
        public string ProjectIdHelperText => "Gitlab project ID (integer value)";
        public string AccessTokenHelperText => "Gitlab Access Token";
    }
}
