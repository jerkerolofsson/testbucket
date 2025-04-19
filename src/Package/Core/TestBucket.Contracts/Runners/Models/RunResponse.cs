using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Automation;
using TestBucket.Formats;

namespace TestBucket.Contracts.Runners.Models
{
    public class RunResponse
    {
        public PipelineJobStatus Status { get; set; } = PipelineJobStatus.Unknown;

        /// <summary>
        /// ID for the job
        /// </summary>
        public required string Guid { get; set; }

        /// <summary>
        /// True if the request was accepted and started
        /// </summary>
        public bool Accepted { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Standard out, from runner
        /// </summary>
        public string? StdOut { get; set; }

        /// <summary>
        /// Standard error, from runner
        /// </summary>
        public string? StdErr { get; set; }

        /// <summary>
        /// Serialized test result (e.g. a JUnitXml)
        /// </summary>
        public string? Result { get; set; }

        /// <summary>
        /// Test result format (of Result)
        /// </summary>
        public TestResultFormat? Format { get; set; }
    }
}
