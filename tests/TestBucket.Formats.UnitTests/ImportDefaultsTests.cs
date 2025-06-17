using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Formats.Dtos;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests
{
    /// <summary>
    /// Contains unit tests for the <see cref="ImportDefaults"/> class, specifically for the GetExternalId method.
    /// </summary>
    [FunctionalTest]
    [Component("Test Result Formats")]
    [Feature("Import Test Results")]
    [UnitTest]
    [EnrichedTest]
    public class ImportDefaultsTests
    {
        /// <summary>
        /// Tests that <see cref="ImportDefaults.GetExternalId"/> returns a non-empty value when only the run name is provided.
        /// </summary>
        [Fact]
        public void GetExternalId_WithOnlyRunName()
        {
            var run = new TestRunDto { Name = "run" };
            var suite = new TestSuiteRunDto { Name = null };
            var testCase = new TestCaseRunDto { Name = null };

            var name = ImportDefaults.GetExternalId(run, suite, testCase);
            Assert.NotEmpty(name);
        }

        /// <summary>
        /// Tests that <see cref="ImportDefaults.GetExternalId"/> returns a non-empty value when only the suite name is provided.
        /// </summary>
        [Fact]
        public void GetExternalId_WithOnlySuiteName()
        {
            var run = new TestRunDto { Name = null };
            var suite = new TestSuiteRunDto { Name = "suite" };
            var testCase = new TestCaseRunDto { Name = null };

            var name = ImportDefaults.GetExternalId(run, suite, testCase);
            Assert.NotEmpty(name);
        }

        /// <summary>
        /// Tests that <see cref="ImportDefaults.GetExternalId"/> returns a non-empty value when only the test case name is provided.
        /// </summary>
        [Fact]
        public void GetExternalId_WithOnlyCaseName()
        {
            var run = new TestRunDto { Name = null };
            var suite = new TestSuiteRunDto { Name = null };
            var testCase = new TestCaseRunDto { Name = "case" };

            var name = ImportDefaults.GetExternalId(run, suite, testCase);
            Assert.NotEmpty(name);
        }

        /// <summary>
        /// Tests that <see cref="ImportDefaults.GetExternalId"/> returns a non-empty value when all components (run, suite, and test case names) are provided.
        /// </summary>
        [Fact]
        public void GetExternalId_WithAllComponents()
        {
            var run = new TestRunDto { Name = "run" };
            var suite = new TestSuiteRunDto { Name = "suite" };
            var testCase = new TestCaseRunDto { Name = "test" };

            var name = ImportDefaults.GetExternalId(run, suite, testCase);
            Assert.NotEmpty(name);
        }
    }
}