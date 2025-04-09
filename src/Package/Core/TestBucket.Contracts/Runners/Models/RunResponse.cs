using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Runners.Models
{
    public class RunResponse
    {
        /// <summary>
        /// True if the request was accepted and started
        /// </summary>
        public bool Accepted { get; set; }
    }
}
