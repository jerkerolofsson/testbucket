using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Testing.Models
{
    public enum TestResult
    {
        NoRun = 0,
        Blocked = 1,
        Failed = 2,
        Passed = 3,

        /// <summary>
        /// There was a runner error. The test didn't fail, but couldnt be run
        /// </summary>
        Error = 4,

        /// <summary>
        /// Crash detected during testing
        /// </summary>
        Crashed = 5,

        /// <summary>
        /// Test was [intentionally] skipped
        /// </summary>
        Skipped = 6,

        /// <summary>
        /// Assert 
        /// </summary>
        Assert = 7,

        /// <summary>
        /// Hang detected
        /// </summary>
        Hang = 8,

        /// <summary>
        /// Assert.InConclusive
        /// 
        /// Manual review required
        /// </summary>
        Inconclusive = 9,


        Other = 100
    }
}
