using TestBucket.Contracts.Integrations;

namespace TestBucket.Jira
{
    class JiraExtension : IExtension
    {
        public string SystemName => ExtensionConstants.SystemName;

        /// <summary>
        /// Defines the capabilities implemented by the extension.
        /// This will show the option to enable the capability in the UI settings for the extension
        /// </summary>
        public ExternalSystemCapability SupportedCapabilities => 
            ExternalSystemCapability.GetIssues;

        public string FriendlyName => ExtensionConstants.FriendlyName;

        public string Description => ExtensionConstants.Description;

        public string Version => ExtensionConstants.Version;

        public string? Icon => ExtensionConstants.Icon;


        public string DefaultBaseUrl => "https://<project>.atlassian.net";
        public string ProjectIdHelperText => "User name";
        public string AccessTokenHelperText => "Jira Access Token";

        public ExtensionFields RequiredFields => ExtensionFields.AccessToken | ExtensionFields.BaseUrl | ExtensionFields.ProjectId;

        public void ConfigureDefaults(ExternalSystemDto system)
        {
        }
    }

}
