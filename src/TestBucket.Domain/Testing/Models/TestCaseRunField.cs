﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Fields.Models;

namespace TestBucket.Domain.Testing.Models;

[Table("test_case_run_fields")]
[Index(nameof(TenantId), nameof(TestCaseRunId))]
public class TestCaseRunField : FieldValue
{

    // Navigation

    public required long TestCaseRunId { get; set; }
    public TestCaseRun? TestCaseRun { get; set; }

    public required long TestRunId { get; set; }
    public TestRun? TestRun { get; set; }
}
