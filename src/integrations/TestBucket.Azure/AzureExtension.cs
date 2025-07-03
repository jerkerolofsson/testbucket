using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;

namespace TestBucket.Azure
{
    class AzureExtension : IExtension
    {
        public string SystemName => ExtensionConstants.SystemName;

        public ExternalSystemCapability SupportedCapabilities => ExternalSystemCapability.GetMilestones;

        public string FriendlyName => ExtensionConstants.FriendlyName;

        public string Description => ExtensionConstants.Description;

        public string Version => ExtensionConstants.Version;

        public string? Icon => ExtensionConstants.Icon;

        public string DefaultBaseUrl => "https://azure.microsoft.com";
        public string ProjectIdHelperText => "tbd";

        public string AccessTokenHelperText => "tbd";

        public ExtensionFields RequiredFields => ExtensionFields.BaseUrl | ExtensionFields.AccessToken;

        public void ConfigureDefaults(ExternalSystemDto system)
        {
        }
    }
}
