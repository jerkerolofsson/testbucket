using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Formats.Dtos;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests
{
    [FunctionalTest]
    [Component("Test Result Formats")]
    [Feature("Import Test Results")]
    [UnitTest]
    [EnrichedTest]
    public class ImportDefaultsTests
    {
        [Fact]
        public void GetExternalId_WithOnlyRunName()
        {
            var run = new TestRunDto { Name = "run" };
            var suite = new TestSuiteRunDto { Name = null };
            var testCase = new TestCaseRunDto { Name = null };

            var name = ImportDefaults.GetExternalId(run, suite, testCase);
            Assert.NotEmpty(name);
        }

        [Fact]
        public void GetExternalId_WithOnlySuiteName()
        {
            var run = new TestRunDto { Name = null };
            var suite = new TestSuiteRunDto { Name = "suite" };
            var testCase = new TestCaseRunDto { Name = null };

            var name = ImportDefaults.GetExternalId(run, suite, testCase);
            Assert.NotEmpty(name);
        }


        [Fact]
        public void GetExternalId_WithOnlyCaseName()
        {
            var run = new TestRunDto { Name = null };
            var suite = new TestSuiteRunDto { Name = null };
            var testCase = new TestCaseRunDto { Name = "case" };

            var name = ImportDefaults.GetExternalId(run, suite, testCase);
            Assert.NotEmpty(name);
        }

        [Fact]
        public void GetExternalId_WithAllComponents()
        {
            var run = new TestRunDto { Name = "run" };
            var suite = new TestSuiteRunDto { Name = "suite" };
            var testCase = new TestCaseRunDto { Name = "test" };

            var name = ImportDefaults.GetExternalId(run, suite, testCase );
            Assert.NotEmpty(name);
        }
    }
}
