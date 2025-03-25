using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Testing.Aggregates
{
    /// <summary>
    /// Contains metrics about test execution
    /// </summary>
    public class TestExecutionResultSummary
    {
        /// <summary>
        /// Number of completed tests (Any other result than NoRun)
        /// </summary>
        public int Completed { get; set; }

        /// <summary>
        /// Total number of tests
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Number of passed
        /// </summary>
        public int Passed { get; set; }

        /// <summary>
        /// Number of failed tests
        /// </summary>
        public int Failed { get; set; }

        /// <summary>
        /// Number of blocked tests
        /// </summary>
        public int Blocked { get; set; }

        /// <summary>
        /// Number of skipped tests
        /// </summary>
        public int Skipped { get; set; }

        /// <summary>
        /// Number of no run tests
        /// </summary>
        public int NoRun => Total - Completed;

        /// <summary>
        /// Number of asserted tests
        /// </summary>
        public int Assert { get; set; }
        public int Hang { get; set; }
        public int Error { get; set; }
    }
}
