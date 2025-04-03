using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Runners.Models
{
    public class ConnectRequest
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
        /// Runner tags
        /// </summary>
        public string[] Tags { get; set; } = [];
    }
}
