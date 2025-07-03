using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;

namespace TestBucket.Trello
{
    class TrelloExtension : IExtension
    {
        public string SystemName => ExtensionConstants.SystemName;

        public ExternalSystemCapability SupportedCapabilities => ExternalSystemCapability.GetRequirements;

        public string FriendlyName => ExtensionConstants.FriendlyName;

        public string Description => ExtensionConstants.Description;

        public string Version => ExtensionConstants.Version;

        public string? Icon => ExtensionConstants.Icon;

        public string DefaultBaseUrl => "https://trello.com/";

        public string ProjectIdHelperText => "to be defined";
        public string AccessTokenHelperText => "to be defined";

        public ExtensionFields RequiredFields => ExtensionFields.AccessToken | ExtensionFields.BaseUrl | ExtensionFields.ApiKey;

        public void ConfigureDefaults(ExternalSystemDto system)
        {
        }
    }
}
