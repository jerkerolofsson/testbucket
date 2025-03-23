using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Testing.Models;
public class SearchTestCaseRunQuery : SearchQuery
{
    public long? TestRunId { get; set; }
    public long? TestSuiteId { get; set; }

    public TestResult? Result { get; set; }
    public string? State { get; set; }

}
