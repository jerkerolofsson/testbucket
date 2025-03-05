namespace TestBucket.Formats.Dtos
{
    /// <summary>
    /// An aggregated result
    /// </summary>
    public class TestResultSummary
    {
        public long Passed { get; set; }
        public long Failed { get; set; }
        public long Skipped { get; set; }
        public long Errors { get; set; }
        public long Crashed { get; set; }
        public long Asserts { get; set; }
    }
}
