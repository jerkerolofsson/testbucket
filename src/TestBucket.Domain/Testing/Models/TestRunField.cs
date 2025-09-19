namespace TestBucket.Domain.Testing.Models;

[Table("test_run_fields")]
public class TestRunField : FieldValue
{

    // Navigation
    public required long TestRunId { get; set; }
    public TestRun? TestRun { get; set; }
}
