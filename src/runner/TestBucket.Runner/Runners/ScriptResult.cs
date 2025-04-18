using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Runner.Runners
{
    public class ScriptResult
    {
        /// <summary>
        /// Process exit code
        /// </summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// True if success
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Exception caught during exeecution
        /// </summary>
        public Exception? Exception { get; set; }
    }
}
