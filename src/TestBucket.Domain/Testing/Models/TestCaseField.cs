using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Fields.Models;

namespace TestBucket.Domain.Testing.Models;

[Table("test_case_fields")]
public class TestCaseField : FieldValue
{

    // Navigation

    public required long TestCaseId { get; set; }
    public TestCase? TestCase { get; set; }
}
