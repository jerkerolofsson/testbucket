using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Formats.Dtos;

namespace TestBucket.Contracts.Runners.Models
{
    /// <summary>
    /// Request sent from server to a runner
    /// </summary>
    public class RunRequest
    {
        /// <summary>
        /// ID for the job
        /// </summary>
        public required string Guid { get; set; }

        /// <summary>
        /// Run
        /// </summary>
        public long? TestRunId { get; set; }

        /// <summary>
        /// Langauge/shell (hybrid tests)
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// Script to run (hybrid tests)
        /// </summary>
        public string? Script { get; set; }

        /// <summary>
        /// Environment variables
        /// </summary>
        public Dictionary<string, string> EnvironmentVariables { get; set; } = [];
    }
}
