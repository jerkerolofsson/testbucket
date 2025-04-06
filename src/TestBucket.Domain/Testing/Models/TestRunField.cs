using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Fields.Models;

namespace TestBucket.Domain.Testing.Models;

[Table("test_run_fields")]
public class TestRunField : FieldValue
{

    // Navigation
    public required long TestRunId { get; set; }
    public TestRun? TestRun { get; set; }
}
