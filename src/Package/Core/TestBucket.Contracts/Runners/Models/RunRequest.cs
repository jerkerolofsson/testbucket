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
        /// Environment variables
        /// </summary>
        public Dictionary<string, string> EnvironmentVariables { get; set; } = [];
    }
}
