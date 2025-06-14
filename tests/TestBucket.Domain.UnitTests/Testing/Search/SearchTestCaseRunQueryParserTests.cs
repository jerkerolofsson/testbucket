using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.UnitTests.Testing.Search
{
    /// <summary>
    /// Unit tests for <see cref="SearchTestCaseRunQueryParser"/>.
    /// </summary>
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    [Component("Testing")]
    public class SearchTestCaseRunQueryParserTests
    {
        private static readonly IReadOnlyList<FieldDefinition> EmptyFields = new List<FieldDefinition>();

        /// <summary>
        /// Verifies that the parser correctly parses the 'id' keyword.
        /// </summary>
        [Fact]
        [Feature("Search")]
        public void Parse_IdKeyword_SetsExternalDisplayId()
        {
            var query = SearchTestCaseRunQueryParser.Parse("id:ABC-123", EmptyFields);
            Assert.Equal("ABC-123", query.ExternalDisplayId);
        }

        /// <summary>
        /// Verifies that the parser correctly parses the 'assigned-to' keyword.
        /// </summary>
        [Fact]
        [Feature("Search")]
        public void Parse_AssignedToKeyword_SetsAssignedToUser()
        {
            var query = SearchTestCaseRunQueryParser.Parse("assigned-to:alice", EmptyFields);
            Assert.Equal("alice", query.AssignedToUser);
        }

        /// <summary>
        /// Verifies that the parser correctly parses the 'testsuite-id' keyword.
        /// </summary>
        [Fact]
        [Feature("Search")]
        public void Parse_TestSuiteIdKeyword_SetsTestSuiteId()
        {
            var query = SearchTestCaseRunQueryParser.Parse("testsuite-id:42", EmptyFields);
            Assert.Equal(42, query.TestSuiteId);
        }

        /// <summary>
        /// Verifies that the parser correctly parses the 'testrun-id' keyword.
        /// </summary>
        [Fact]
        [Feature("Search")]
        public void Parse_TestRunIdKeyword_SetsTestRunId()
        {
            var query = SearchTestCaseRunQueryParser.Parse("testrun-id:99", EmptyFields);
            Assert.Equal(99, query.TestRunId);
        }

        /// <summary>
        /// Verifies that the parser correctly parses the 'completed' keyword.
        /// </summary>
        [Fact]
        [Feature("Search")]
        public void Parse_CompletedKeyword_SetsCompleted()
        {
            var query = SearchTestCaseRunQueryParser.Parse("completed:yes", EmptyFields);
            Assert.True(query.Completed);

            query = SearchTestCaseRunQueryParser.Parse("completed:no", EmptyFields);
            Assert.False(query.Completed);
        }

        /// <summary>
        /// Verifies that the parser correctly parses the 'unassigned' keyword.
        /// </summary>
        [Fact]
        [Feature("Search")]
        public void Parse_UnassignedKeyword_SetsUnassigned()
        {
            var query = SearchTestCaseRunQueryParser.Parse("unassigned:yes", EmptyFields);
            Assert.True(query.Unassigned);

            query = SearchTestCaseRunQueryParser.Parse("unassigned:no", EmptyFields);
            Assert.False(query.Unassigned);
        }

        /// <summary>
        /// Verifies that the parser correctly parses the 'result' keyword.
        /// </summary>
        [Fact]
        [Feature("Search")]
        public void Parse_ResultKeyword_SetsResult()
        {
            var query = SearchTestCaseRunQueryParser.Parse("result:passed", EmptyFields);
            Assert.Equal(Contracts.Testing.Models.TestResult.Passed, query.Result);

            query = SearchTestCaseRunQueryParser.Parse("result:failed", EmptyFields);
            Assert.Equal(Contracts.Testing.Models.TestResult.Failed, query.Result);
        }

        /// <summary>
        /// Verifies that the parser correctly parses the 'state' keyword.
        /// </summary>
        [Fact]
        [Feature("Search")]
        public void Parse_StateKeyword_SetsState()
        {
            var query = SearchTestCaseRunQueryParser.Parse("state:active", EmptyFields);
            Assert.Equal("active", query.State);
        }

        /// <summary>
        /// Verifies that the parser correctly parses the 'metric' keyword.
        /// </summary>
        [Fact]
        [Feature("Metrics")]
        public void Parse_MetricKeyword_SetsHasMetricName()
        {
            var query = SearchTestCaseRunQueryParser.Parse("metric:duration", EmptyFields);
            Assert.Equal("duration", query.HasMetricName);
        }
    }
}