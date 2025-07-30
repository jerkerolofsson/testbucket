using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Metrics.Models;
using TestBucket.Domain.Testing.Specifications.TestCaseRuns;
using TestBucket.Domain.Testing.Specifications.TestCases;
using Xunit;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.UnitTests.Testing.Search
{
    /// <summary>
    /// Contains unit tests for TestCase filter specifications.
    /// Each test verifies that a specific filter correctly matches or excludes TestCase instances
    /// based on the filter's intended logic.
    /// </summary>
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    [Component("Testing")]
    [Feature("Search")]
    public class SearchTestCaseSpecificationTests
    {
        /// <summary>
        /// Tests for filtering test cases by automation implementation.
        /// </summary>
        /// <remarks>
        /// Verifies that the filter matches test cases with the specified class name, method, module, and assembly.
        /// </remarks>
        [Fact]
        public void FilterTestCasesByAutomationImplementation_MatchesCorrectly()
        {
            var spec = new FilterTestCasesByAutomationImplementation("ClassName", "Method", "Module", "Assembly");
            var match = new TestCase { Name = "Test1", ClassName = "ClassName", Method = "Method", Module = "Module", AutomationAssembly = "Assembly" };
            var noMatch = new TestCase { Name = "Test2", ClassName = "OtherClass", Method = "OtherMethod", Module = "OtherModule", AutomationAssembly = "OtherAssembly" };

            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Tests for filtering test cases by execution type.
        /// </summary>
        /// <remarks>
        /// Verifies that the filter matches test cases with the specified execution type.
        /// </remarks>
        [Fact]
        public void FilterTestCasesByExecutionType_MatchesCorrectly()
        {
            var spec = new FilterTestCasesByExecutionType(TestExecutionType.Automated);
            var match = new TestCase { Name = "Test1", ExecutionType = TestExecutionType.Automated };
            var noMatch = new TestCase { Name = "Test2", ExecutionType = TestExecutionType.Manual };

            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Tests for filtering test cases by external display ID.
        /// </summary>
        /// <remarks>
        /// Verifies that the filter matches test cases with the specified external display ID.
        /// </remarks>
        [Fact]
        public void FilterTestCasesByExternalDisplayId_MatchesCorrectly()
        {
            var spec = new FilterTestCasesByExternalDisplayId("DisplayId123");
            var match = new TestCase { Name = "Test1", ExternalDisplayId = "DisplayId123" };
            var noMatch = new TestCase { Name = "Test2", ExternalDisplayId = "OtherDisplayId" };

            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Tests for filtering test cases by external ID.
        /// </summary>
        /// <remarks>
        /// Verifies that the filter matches test cases with the specified external ID.
        /// </remarks>
        [Fact]
        public void FilterTestCasesByExternalId_MatchesCorrectly()
        {
            var spec = new FilterTestCasesByExternalId("ExternalId123");
            var match = new TestCase { Name = "Test1", ExternalId = "ExternalId123" };
            var noMatch = new TestCase { Name = "Test2", ExternalId = "OtherExternalId" };

            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Tests for filtering test cases by ID.
        /// </summary>
        /// <remarks>
        /// Verifies that the filter matches test cases with the specified ID.
        /// </remarks>
        [Fact]
        public void FilterTestCasesById_MatchesCorrectly()
        {
            var spec = new FilterTestCasesById(42);
            var match = new TestCase { Name = "Test1", Id = 42 };
            var noMatch = new TestCase { Name = "Test2", Id = 99 };

            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Tests for filtering test cases by name.
        /// </summary>
        /// <remarks>
        /// Verifies that the filter matches test cases with the specified name.
        /// </remarks>
        [Fact]
        public void FilterTestCasesByName_MatchesCorrectly()
        {
            var spec = new FilterTestCasesByName("TestName");
            var match = new TestCase { Name = "TestName" };
            var noMatch = new TestCase { Name = "OtherName" };

            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Tests for filtering test cases by test suite ID.
        /// </summary>
        /// <remarks>
        /// Verifies that the filter matches test cases with the specified test suite ID.
        /// </remarks>
        [Fact]
        public void FilterTestCasesByTestSuite_MatchesCorrectly()
        {
            var spec = new FilterTestCasesByTestSuite(7);
            var match = new TestCase { Name = "Test1", TestSuiteId = 7 };
            var noMatch = new TestCase { Name = "Test2", TestSuiteId = 8 };

            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Tests for filtering test cases by test suite folder.
        /// </summary>
        /// <remarks>
        /// Verifies that the filter matches test cases with the specified test suite folder ID.
        /// </remarks>
        [Fact]
        public void FilterTestCasesByTestSuiteFolder_MatchesCorrectly()
        {
            var spec = new FilterTestCasesByTestSuiteFolder(5, true);
            var match = new TestCase { Name = "Test1", PathIds = new long[] { 5, 6, 7 } };
            var noMatch = new TestCase { Name = "Test2", PathIds = new long[] { 8, 9, 10 } };

            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Tests for filtering test cases by text.
        /// </summary>
        /// <remarks>
        /// Verifies that the filter matches test cases with the specified text.
        /// </remarks>
        [Fact]
        public void FilterTestCasesByText_MatchesCorrectly()
        {
            var spec = new FilterTestCasesByText("SearchText");
            var match = new TestCase { Name = "SearchText" };
            var noMatch = new TestCase { Name = "OtherText" };

            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Tests for excluding automated test cases.
        /// </summary>
        /// <remarks>
        /// Verifies that the filter excludes test cases with automated execution type.
        /// </remarks>
        [Fact]
        public void FilterTestCasesExcludeAutomated_MatchesCorrectly()
        {
            var spec = new FilterTestCasesExcludeAutomated();
            var match = new TestCase { Name = "Test1", ExecutionType = TestExecutionType.Manual };
            var noMatch = new TestCase { Name = "Test2", ExecutionType = TestExecutionType.Automated };

            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }

        /// <summary>
        /// Tests for filtering test cases that require classification.
        /// </summary>
        /// <remarks>
        /// Verifies that the filter matches test cases that require classification.
        /// </remarks>
        [Fact]
        public void FilterTestCasesThatRequireClassification_MatchesCorrectly()
        {
            var spec = new FilterTestCasesThatRequireClassification();
            var match = new TestCase { Name = "Test1", ClassificationRequired = true };
            var noMatch = new TestCase { Name = "Test2", ClassificationRequired = false };

            Assert.True(spec.IsMatch(match));
            Assert.False(spec.IsMatch(noMatch));
        }
    }
}