using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Testing.Models
{
    public enum TestExecutionType
    {
        /// <summary>
        /// Manually executed test case
        /// </summary>
        Manual = 0,

        /// <summary>
        /// Automation script is contained within the body of the test case
        /// The user has to run the automated part manually.
        /// </summary>
        Hybrid = 1,

        /// <summary>
        /// Automated with an external script
        /// </summary>
        Automated = 2,

        /// <summary>
        /// Automation script is contained within the body of the test case
        /// When running the tests a job will be queued to be executed on a runner
        /// </summary>
        HybridAutomated = 3,
    }
}
