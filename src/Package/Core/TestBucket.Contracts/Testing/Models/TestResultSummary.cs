namespace TestBucket.Formats.Dtos
{
    /// <summary>
    /// An aggregated result
    /// </summary>
    public class TestResultSummary
    {
        /// <summary>
        /// Number of passed tests
        /// </summary>
        public long Passed { get; set; }

        /// <summary>
        /// Number of failed tests
        /// </summary>
        public long Failed { get; set; }

        /// <summary>
        /// Number of skipped tests
        /// </summary>
        public long Skipped { get; set; }

        /// <summary>
        /// Number of errors
        /// </summary>
        public long Errors { get; set; }

        /// <summary>
        /// Number of crashed tests
        /// </summary>
        public long Crashed { get; set; }

        /// <summary>
        /// Number of asserted tests
        /// </summary>
        public long Asserts { get; set; }
    }
}
