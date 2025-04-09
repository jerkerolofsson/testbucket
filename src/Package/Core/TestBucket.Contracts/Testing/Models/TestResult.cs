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
        Error = 4,
        Crashed = 5,
        Skipped = 6,
        Assert = 7,
        Hang = 8,
        Other = 100
    }
}
