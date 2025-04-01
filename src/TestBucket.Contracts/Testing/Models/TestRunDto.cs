namespace TestBucket.Formats.Dtos
{
    public class TestRunDto : TestTraitCollection
    {
        public int Count(TestResult result)
        {
            if (Suites is not null)
            {
                return Suites.Sum(x => x.Count(result));
            }
            return 0;
        }

        /// <summary>
        /// Attachments for the test run
        /// </summary>
        public AttachmentCollectionDto Attachments { get; set; } = new();

        /// <summary>
        /// Returns the sum of the duration of all suites
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return TimeSpan.FromSeconds(Suites.Sum(x => x.Duration.TotalSeconds));
            }
        }

        public int Passed => Count(TestResult.Passed);
        public int Failed => Count(TestResult.Failed);
        public int Errors => Count(TestResult.Error);
        public int Crashed => Count(TestResult.Crashed);
        public int Skipped => Count(TestResult.Skipped);
        public int NoRuns => Count(TestResult.NoRun);
        public int Blocked => Count(TestResult.Blocked);
        public int Asserted => Count(TestResult.Assert);

        public int Total
        {
            get
            {
                if (Suites is not null)
                {
                    return Suites.Sum(x => x.Total);
                }
                return 0;
            }
        }

        public List<TestSuiteRunDto> Suites { get; set; } = [];

        public IEnumerable<TestCaseRunDto> SelectTests(Predicate<TestCaseRunDto> predicate)
        {
            foreach(var suite in Suites)
            {
                foreach(var test in suite.Tests.Where(x => predicate(x)))
                {
                    yield return test;
                }
            }
        }
    }
}
