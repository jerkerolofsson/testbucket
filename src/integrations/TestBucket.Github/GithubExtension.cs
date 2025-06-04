using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;

namespace TestBucket.Github
{
    class GithubExtension : IExtension
    {
        public string SystemName => ExtensionConstants.SystemName;

        public ExternalSystemCapability SupportedCapabilities => 
            ExternalSystemCapability.GetMilestones | 
            ExternalSystemCapability.CreatePipeline | 
            ExternalSystemCapability.ReadCodeRepository |
            ExternalSystemCapability.ReadPipelineArtifacts | 
            ExternalSystemCapability.GetIssues | 
            ExternalSystemCapability.GetLabels |
            ExternalSystemCapability.CreateIssues;

        public string FriendlyName => ExtensionConstants.FriendlyName;

        public string Description => ExtensionConstants.Description;

        public string Version => ExtensionConstants.Version;

        public string? Icon => ExtensionConstants.Icon;

        public string DefaultBaseUrl => "https://www.github.com";
        public string ProjectIdHelperText => "Format: organization/project";
        public string AccessTokenHelperText => "Github Personal Access Token with valid scopes for the enabled capabilities";

        public ExtensionFields RequiredFields => ExtensionFields.AccessToken | ExtensionFields.BaseUrl | ExtensionFields.ProjectId;
    }
}
