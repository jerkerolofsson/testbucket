using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Fields.Models;

public enum FieldTarget
{
    TestCase = 1,
    TestSuite = 2,
    TestRun = 4,

    Project = 8,
}
