using System.Text.Json.Serialization;

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

        #region Count by test result
        [JsonIgnore]
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

        [JsonIgnore]
        public int Passed => Count(TestResult.Passed);

        [JsonIgnore]
        public int Failed => Count(TestResult.Failed);

        [JsonIgnore]
        public int Errors => Count(TestResult.Error);

        [JsonIgnore]
        public int Crashed => Count(TestResult.Crashed);

        [JsonIgnore]
        public int Skipped => Count(TestResult.Skipped);

        [JsonIgnore]
        public int NoRuns => Count(TestResult.NoRun);

        [JsonIgnore]
        public int Blocked => Count(TestResult.Blocked);

        [JsonIgnore]
        public int Asserted => Count(TestResult.Assert);

        /// <summary>
        /// Total number of executed tests
        /// </summary>
        [JsonIgnore]
        public int Executed
        {
            get
            {
                if (Suites is not null)
                {
                    return Suites.Sum(x => x.Executed);
                }
                return 0;
            }
        }

        /// <summary>
        /// Total number of tests, in all suites
        /// </summary>
        [JsonIgnore]
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
        #endregion Count by test result

        /// <summary>
        /// Test suites
        /// </summary>
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
