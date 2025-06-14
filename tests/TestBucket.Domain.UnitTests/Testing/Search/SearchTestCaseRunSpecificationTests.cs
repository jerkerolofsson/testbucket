using TestBucket.Domain.Metrics.Models;
using TestBucket.Domain.Testing.Specifications.TestCaseRuns;
using TestBucket.Domain.Testing.Specifications.TestCases;

namespace TestBucket.Domain.UnitTests.Testing.Search
{
    /// <summary>
    /// Contains unit tests for TestCaseRuns filter specifications.
    /// Each test verifies that a specific filter correctly matches or excludes <see cref="TestCaseRun"/> instances
    /// based on the filter's intended logic.
    /// </summary>
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    [Component("Testing")]
    [Feature("Search")]
    public class SearchTestCaseRunSpecificationTests
    {
        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsByRun"/> matches TestCaseRun with the specified TestRunId.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsByRun_MatchesCorrectRun()
        {

            var spec = new FilterTestCaseRunsByRun(42);
            var match = new TestCaseRun { TestRunId = 42, Name = "a" };
            var noMatch = new TestCaseRun { TestRunId = 99, Name = "b" };
            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsCompleted"/> matches TestCaseRun with a result other than NoRun.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsCompleted_MatchesCompleted()
        {
            var spec = new FilterTestCaseRunsCompleted();
            var completed = new TestCaseRun { Result = Contracts.Testing.Models.TestResult.Passed, Name = "a" };
            var notCompleted = new TestCaseRun { Result = Contracts.Testing.Models.TestResult.NoRun, Name = "b" };
            Assert.True(spec.IsMatch(completed));
            Assert.False(spec.IsMatch(notCompleted));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsIncomplete"/> matches TestCaseRun with result NoRun.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsIncomplete_MatchesIncomplete()
        {
            var spec = new FilterTestCaseRunsIncomplete();
            var incomplete = new TestCaseRun { Result = Contracts.Testing.Models.TestResult.NoRun, Name = "a" };
            var notIncomplete = new TestCaseRun { Result = Contracts.Testing.Models.TestResult.Failed, Name = "b" };
            Assert.True(spec.IsMatch(incomplete));
            Assert.False(spec.IsMatch(notIncomplete));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsAssigned"/> matches TestCaseRun with a non-null AssignedToUserName.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsAssigned_MatchesAssigned()
        {
            var spec = new FilterTestCaseRunsAssigned();
            var assigned = new TestCaseRun { AssignedToUserName = "user", Name = "a" };
            var unassigned = new TestCaseRun { AssignedToUserName = null, Name = "b" };
            Assert.True(spec.IsMatch(assigned));
            Assert.False(spec.IsMatch(unassigned));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsUnassigned"/> matches TestCaseRun with a null AssignedToUserName.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsUnassigned_MatchesUnassigned()
        {
            var spec = new FilterTestCaseRunsUnassigned();
            var assigned = new TestCaseRun { AssignedToUserName = "user", Name = "a" };
            var unassigned = new TestCaseRun { AssignedToUserName = null, Name = "b" };
            Assert.False(spec.IsMatch(assigned));
            Assert.True(spec.IsMatch(unassigned));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsByAssignment"/> matches TestCaseRun assigned to the specified user.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsByAssignment_MatchesUser()
        {
            var spec = new FilterTestCaseRunsByAssignment("alice");
            var match = new TestCaseRun { AssignedToUserName = "alice", Name = "a" };
            var noMatch = new TestCaseRun { AssignedToUserName = "bob", Name = "b" };
            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsByExternalDisplayId"/> matches TestCaseRun with the specified external display ID.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsByExternalDisplayId_MatchesId()
        {
            var spec = new FilterTestCaseRunsByExternalDisplayId("ext-1");
            var match = new TestCaseRun { Name = "a", TestCase = new TestCase { ExternalDisplayId = "ext-1", Name = "a" } };
            var noMatch = new TestCaseRun { Name = "a", TestCase = new TestCase { ExternalDisplayId = "ext-2", Name = "b" } };
            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsByTestCase"/> matches TestCaseRun with the specified TestCaseId.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsByTestCase_MatchesTestCase()
        {
            var spec = new FilterTestCaseRunsByTestCase(5);
            var match = new TestCaseRun { TestCase = new TestCase() { Name = "a" }, TestCaseId = 5, Name = "a" };
            var noMatch = new TestCaseRun { TestCase = new TestCase() { Name = "b" }, TestCaseId = 6, Name = "b" };
            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsByTestSuite"/> matches TestCaseRun with the specified TestSuiteId.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsByTestSuite_MatchesTestSuite()
        {
            var spec = new FilterTestCaseRunsByTestSuite(7);
            var match = new TestCaseRun { TestCase = new TestCase { TestSuiteId = 7, Name = "a" }, Name = "a" };
            var noMatch = new TestCaseRun { TestCase = new TestCase { TestSuiteId = 8, Name = "b" }, Name = "b" };
            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsByText"/> matches TestCaseRun whose name contains the search phrase.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsByText_MatchesText()
        {
            var spec = new FilterTestCaseRunsByText("foo");
            var match = new TestCaseRun { Name = "FooBar" };
            var noMatch = new TestCaseRun { Name = "BarBaz" };
            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsByState"/> matches TestCaseRun with the specified state.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsByState_MatchesState()
        {
            var spec = new FilterTestCaseRunsByState("Ready");
            var match = new TestCaseRun { Name = "a", State = "Ready" };
            var noMatch = new TestCaseRun { Name = "a", State = "Blocked" };
            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsByResult"/> matches TestCaseRun with the specified result.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsByResult_MatchesResult()
        {
            var spec = new FilterTestCaseRunsByResult(Contracts.Testing.Models.TestResult.Failed);
            var match = new TestCaseRun { Name = "a", Result = Contracts.Testing.Models.TestResult.Failed };
            var noMatch = new TestCaseRun { Name = "a", Result = Contracts.Testing.Models.TestResult.Passed };
            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsByStringField"/> matches TestCaseRun with a string field of the specified value.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsByStringField_MatchesStringField()
        {
            var spec = new FilterTestCaseRunsByStringField(1, "cat");
            var match = new TestCaseRun { Name = "a", TestCaseRunFields = new List<TestCaseRunField> { new TestCaseRunField { FieldDefinitionId = 1, StringValue = "cat", TestCaseRunId = 0, TestRunId = 0 } } };
            var noMatch = new TestCaseRun { Name = "b", TestCaseRunFields = new List<TestCaseRunField> { new TestCaseRunField { FieldDefinitionId = 1, StringValue = "dog", TestCaseRunId = 0, TestRunId = 0 } } };
            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Verifies that <see cref="FilterTestCaseRunsByBooleanField"/> matches TestCaseRun with a boolean field of the specified value.
        /// </summary>
        [Fact]
        public void FilterTestCaseRunsByBooleanField_MatchesBooleanField()
        {
            var spec = new FilterTestCaseRunsByBooleanField(2, true);
            var match = new TestCaseRun { Name = "a", TestCaseRunFields = new List<TestCaseRunField> { new TestCaseRunField { FieldDefinitionId = 2, BooleanValue = true, TestCaseRunId = 0, TestRunId = 0 } } };
            var noMatch = new TestCaseRun { Name = "b", TestCaseRunFields = new List<TestCaseRunField> { new TestCaseRunField { FieldDefinitionId = 2, BooleanValue = false, TestCaseRunId = 0, TestRunId = 0 } } };
            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }
    }
}