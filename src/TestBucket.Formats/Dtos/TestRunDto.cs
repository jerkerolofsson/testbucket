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

        public List<TestSuiteRunDto>? Suites { get; set; }
    }
}
