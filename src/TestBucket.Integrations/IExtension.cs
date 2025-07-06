using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Integrations
{
    public interface IExtension
    {
        /// <summary>
        /// Name/Id of integrated system
        /// </summary>
        public string SystemName { get; }

        /// <summary>
        /// User friendly name
        /// </summary>
        public string FriendlyName { get; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Version
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Icon (SVG)
        /// </summary>
        public string? Icon { get; }

        /// <summary>
        /// Defines the capabilities implemented by the extension.
        /// This will show the option to enable the capability in the UI settings for the extension
        /// </summary>
        public ExternalSystemCapability SupportedCapabilities { get; }

        /// <summary>
        /// Required fields from extension
        /// </summary>
        public ExtensionFields RequiredFields { get; }

        /// <summary>
        /// The default URL to integrate with
        /// </summary>
        string DefaultBaseUrl { get; }

        /// <summary>
        /// Helper text for the user regarding how to enter the project id
        /// </summary>
        public string ProjectIdHelperText { get; }

        /// <summary>
        /// Description of access token specific to system we are integrating wtih
        /// </summary>
        string AccessTokenHelperText { get; }

        /// <summary>
        /// Configures default settings
        /// </summary>
        /// <param name="system"></param>
        void ConfigureDefaults(ExternalSystemDto system);
    }
}
