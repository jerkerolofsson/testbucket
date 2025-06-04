using TestBucket.Contracts.Integrations;

namespace TestBucket.Gitlab
{
    class GitlabExtension : IExtension
    {
        public string SystemName => ExtensionConstants.SystemName;

        /// <summary>
        /// Defines the capabilities implemented by the extension.
        /// This will show the option to enable the capability in the UI settings for the extension
        /// </summary>
        public ExternalSystemCapability SupportedCapabilities => 
            ExternalSystemCapability.CreatePipeline | 
            ExternalSystemCapability.GetPipelines | 
            ExternalSystemCapability.GetReleases |
            ExternalSystemCapability.GetIssues |
            ExternalSystemCapability.GetLabels |
            ExternalSystemCapability.GetMilestones | 
            ExternalSystemCapability.ReadPipelineArtifacts |
            ExternalSystemCapability.ReadCodeRepository;

        public string FriendlyName => ExtensionConstants.FriendlyName;

        public string Description => ExtensionConstants.Description;

        public string Version => ExtensionConstants.Version;

        public string? Icon => ExtensionConstants.Icon;


        public string DefaultBaseUrl => "https://gitlab.com/";
        public string ProjectIdHelperText => "Gitlab project ID (integer value)";
        public string AccessTokenHelperText => "Gitlab Access Token";

        public ExtensionFields RequiredFields => ExtensionFields.AccessToken | ExtensionFields.BaseUrl | ExtensionFields.ProjectId;
    }
}
