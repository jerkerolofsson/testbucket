using TestBucket.Domain.Metrics.Models;
using TestBucket.Domain.Testing.Specifications.TestCaseRuns;
using TestBucket.Domain.Testing.Specifications.TestCases;

namespace TestBucket.Domain.UnitTests.Testing.Search
{
    /// <summary>
    /// Contains unit tests for TestCaseRuns filter specifications related to metrics.
    /// Each test verifies that a specific filter correctly matches or excludes <see cref="TestCaseRun"/> instances
    /// based on the filter's intended logic.
    /// </summary>
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    [Component("Testing")]
    [Feature("Metrics")]
    public class SearchTestCaseRunMetricSpecificationTests
    {
        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsMetricFilter"/> matches <see cref="TestCaseRun"/> with a metric of the specified name.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsThatHasMetric_MatchesMetric()
        {
            var spec = new FilterTestCaseRunsMetricFilter("duration");
            var match = new TestCaseRun { Name = "a", Metrics = new List<Metric> { new Metric { Name = "duration", MeterName = "a", Value = 1 } } };
            var noMatch = new TestCaseRun { Name = "b", Metrics = new List<Metric> { new Metric { Name = "other", MeterName = "a", Value = 0 } } };
            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsMetricFilter"/> with an equals operator ("=") matches only <see cref="TestCaseRun"/> instances
        /// where the metric value is exactly equal to the specified value.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsMetric_EqualsOperator()
        {
            var spec = new FilterTestCaseRunsMetricFilter("duration==5");
            var match = new TestCaseRun { Name = "a", Metrics = new List<Metric> { new Metric { Name = "duration", MeterName = "a", Value = 5 } } };
            var noMatch = new TestCaseRun { Name = "a", Metrics = new List<Metric> { new Metric { Name = "duration", MeterName = "a", Value = 4 } } };
            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsMetricFilter"/> with a not-equals operator ("!=") matches only <see cref="TestCaseRun"/> instances
        /// where the metric value is not equal to the specified value.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsMetric_NotEqualsOperator()
        {
            var spec = new FilterTestCaseRunsMetricFilter("duration!=5");
            var match = new TestCaseRun { Name = "a", Metrics = new List<Metric> { new Metric { Name = "duration", MeterName = "a", Value = 4 } } };
            var noMatch = new TestCaseRun { Name = "a", Metrics = new List<Metric> { new Metric { Name = "duration", MeterName = "a", Value = 5 } } };
            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsMetricFilter"/> with a greater-than operator (">") matches only <see cref="TestCaseRun"/> instances
        /// where the metric value is greater than the specified value.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsMetric_GreaterThanOperator()
        {
            var spec = new FilterTestCaseRunsMetricFilter("duration>5");
            var match = new TestCaseRun { Name = "a", Metrics = new List<Metric> { new Metric { Name = "duration", MeterName = "a", Value = 6 } } };
            var noMatch = new TestCaseRun { Name = "a", Metrics = new List<Metric> { new Metric { Name = "duration", MeterName = "a", Value = 5 } } };
            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsMetricFilter"/> with a greater-than-or-equal operator (">=") matches only <see cref="TestCaseRun"/> instances
        /// where the metric value is greater than or equal to the specified value.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsMetric_GreaterThanOrEqualOperator()
        {
            var spec = new FilterTestCaseRunsMetricFilter("duration>=5");
            var match = new TestCaseRun { Name = "a", Metrics = new List<Metric> { new Metric { Name = "duration", MeterName = "a", Value = 5 } } };
            var match2 = new TestCaseRun { Name = "a", Metrics = new List<Metric> { new Metric { Name = "duration", MeterName = "a", Value = 6 } } };
            var noMatch = new TestCaseRun { Name = "a", Metrics = new List<Metric> { new Metric { Name = "duration", MeterName = "a", Value = 4 } } };
            Assert.True(spec.IsMatch(match));
            Assert.True(spec.IsMatch(match2));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsMetricFilter"/> with a less-than operator ("&lt;") matches only <see cref="TestCaseRun"/> instances
        /// where the metric value is less than the specified value.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsMetric_LessThanOperator()
        {
            var spec = new FilterTestCaseRunsMetricFilter("duration<5");
            var match = new TestCaseRun { Name = "a", Metrics = new List<Metric> { new Metric { Name = "duration", MeterName = "a", Value = 4 } } };
            var noMatch = new TestCaseRun { Name = "a", Metrics = new List<Metric> { new Metric { Name = "duration", MeterName = "a", Value = 5 } } };
            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsMetricFilter"/> with a less-than-or-equal operator ("&lt;=") matches only <see cref="TestCaseRun"/> instances
        /// where the metric value is less than or equal to the specified value.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsMetric_LessThanOrEqualOperator()
        {
            var spec = new FilterTestCaseRunsMetricFilter("duration<=5");
            var match = new TestCaseRun { Name = "a", Metrics = new List<Metric> { new Metric { Name = "duration", MeterName = "a", Value = 5 } } };
            var match2 = new TestCaseRun { Name = "a", Metrics = new List<Metric> { new Metric { Name = "duration", MeterName = "a", Value = 4 } } };
            var noMatch = new TestCaseRun { Name = "a", Metrics = new List<Metric> { new Metric { Name = "duration", MeterName = "a", Value = 6 } } };
            Assert.True(spec.IsMatch(match));
            Assert.True(spec.IsMatch(match2));
            Assert.False(spec.IsMatch(noMatch));
        }
    }
}