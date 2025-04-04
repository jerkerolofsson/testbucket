﻿namespace TestBucket.Formats.Dtos
{
    public class TestSuiteRunDto : TestTraitCollection
    {
        public int Count(TestResult result)
        {
            if (Tests is not null)
            {
                return Tests.Where(x => x.Result == result).Count();
            }
            return 0;
        }

        #region Count by test result

        /// <summary>
        /// Returns the sum of the duration of all suites
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                if (Tests is not null)
                {
                    var seconds = Tests.Where(x => x.Duration is not null).Select(x => x.Duration!.Value.TotalSeconds).Sum();
                    return TimeSpan.FromSeconds(seconds);
                }
                return TimeSpan.Zero;
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
                if (Tests is not null)
                {
                    return Tests.Count;
                }
                return 0;
            }
        }
        #endregion

        /// <summary>
        /// One item per executed test
        /// </summary>
        public List<TestCaseRunDto> Tests { get; set; } = [];
    }
}
