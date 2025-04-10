using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Projects;

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
        /// Helper text for the user regarding how to enter the project id
        /// </summary>
        public string ProjectIdHelperText { get; }

        /// <summary>
        /// Icon (SVG)
        /// </summary>
        public string? Icon { get; }

        /// <summary>
        /// Supported capabilities of the system
        /// </summary>
        public ExternalSystemCapability SupportedCapabilities { get; }

        /// <summary>
        /// The default URL to integrate with
        /// </summary>
        string DefaultBaseUrl { get; }

        /// <summary>
        /// Description of access token specific to system we are integrating wtih
        /// </summary>
        string AccessTokenHelperText { get; }
    }
}
