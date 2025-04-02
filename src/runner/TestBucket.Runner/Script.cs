using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Runner
{
    public class Script
    {
        /// <summary>
        /// The wrapping executing process
        /// </summary>
        public required string RunnerType { get; set; }

        /// <summary>
        /// Additional environment variables that will be used for the process
        /// </summary>
        public Dictionary<string, string?> EnvironmentVariables { get; set; } = [];

        /// <summary>
        /// Script to run
        /// </summary>
        public required string Text { get; set; }

        public required string WorkingDirectory { get; set; }
    }
}
