using TestBucket.Contracts.Integrations;

namespace TestBucket.Jira
{
    internal class JiraExtension : IExtension
    {
        public string SystemName => ExtensionConstants.SystemName;

        /// <summary>
        /// Defines the capabilities implemented by the extension.
        /// This will show the option to enable the capability in the UI settings for the extension
        /// </summary>
        public ExternalSystemCapability SupportedCapabilities => 
            ExternalSystemCapability.GetIssues |
            ExternalSystemCapability.GetRequirements | 
            ExternalSystemCapability.CreateIssues;

        public string FriendlyName => ExtensionConstants.FriendlyName;

        public string Description => ExtensionConstants.Description;

        public string Version => ExtensionConstants.Version;

        public string? Icon => ExtensionConstants.Icon;


        public string DefaultBaseUrl => "https://<project>.atlassian.net";
        public string ProjectIdHelperText => "Jira Project Name";
        public string AccessTokenHelperText => "Jira Access Token";

        public ExtensionFields RequiredFields => ExtensionFields.ClientId | ExtensionFields.BaseUrl | ExtensionFields.ProjectId;

        public void ConfigureDefaults(ExternalSystemDto system)
        {
            //string scope = "write:jira-work read:jira-work offline_access";

            system.Scope = "write:jira-work read:jira-work offline_access";
            system.AuthEndpoint ??= $"https://auth.atlassian.com/authorize?audience=api.atlassian.com&response_type=code&prompt=consent";
            system.TokenEndpoint ??= "https://auth.atlassian.com/oauth/token";
        }
    }
}
