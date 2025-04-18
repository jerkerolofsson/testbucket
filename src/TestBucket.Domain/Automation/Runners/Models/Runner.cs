using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Automation.Runners.Models
{
    public class Runner : ProjectEntity
    {
        /// <summary>
        /// Id of the runner
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Name of the runner
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Supported languages (this is matched when running hybrid tests)
        /// </summary>
        [Column(TypeName = "jsonb")]
        public string[]? Languages { get; set; }

        /// <summary>
        /// Runner tags
        /// </summary>
        [Column(TypeName = "jsonb")]
        public required string[] Tags { get; set; }

        /// <summary>
        /// Public base url for this service if it is behind a reverse proxy or similar
        /// </summary>
        public string? PublicBaseUrl { get; set; }
    }
}
