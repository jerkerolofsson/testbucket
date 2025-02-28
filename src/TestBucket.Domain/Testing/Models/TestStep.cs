using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Testing.Models;

[Table("steps")]
public class TestStep
{
    public long Id { get; set; }
    public string? Description { get; set; }
    public string? ExpectedResult { get; set; }

    /// <summary>
    /// Navigation
    /// </summary>

    public long TestCaseId { get; set; }
    public TestCase? TestCase { get; set; }
}
