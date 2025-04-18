using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Runners.Models
{
    /// <summary>
    /// Sent from the runner to the server
    /// </summary>
    public class ConnectRequest
    {
        /// <summary>
        /// Id of the runner
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Runner tags
        /// </summary>
        public string[] Tags { get; set; } = [];

        /// <summary>
        /// Runner supported languages/shells
        /// </summary>
        public string[] Languages { get; set; } = [];

        /// <summary>
        /// Name of the runner
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Public base url for this service if it is behind a reverse proxy or similar
        /// </summary>
        public string? PublicBaseUrl { get; set; }
    }
}
