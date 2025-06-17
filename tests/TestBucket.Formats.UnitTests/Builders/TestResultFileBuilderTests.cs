using System;
using System.Linq;
using System.Text;
using TestBucket.Formats.Builders;
using TestBucket.Formats.Dtos;
using TestBucket.Formats.XUnit;
using TestBucket.Traits.Core;
using TestBucket.Traits.Xunit;
using Xunit;
using static TestBucket.Formats.Builders.TestResultFileBuilder;

namespace TestBucket.Formats.UnitTests.Builders
{
    /// <summary>
    /// Contains unit tests for the <see cref="TestResultFileBuilder"/> class, verifying correct construction and serialization of test result files.
    /// </summary>
    [UnitTest]
    [FunctionalTest]
    [Component("Test Result Formats")]
    [Feature("Import Test Results")]
    [EnrichedTest]
    public class TestResultFileBuilderTests
    {
        private TestResultFileBuilder Create() => new TestResultFileBuilder();

        /// <summary>
        /// Builds and deserializes a test run using the specified builder and the xUnit XML format.
        /// </summary>
        /// <param name="builder">The <see cref="TestResultFileBuilder"/> to use.</param>
        /// <returns>The deserialized <see cref="TestRunDto"/>.</returns>
        private static TestRunDto BuildAndDeserialize(TestResultFileBuilder builder)
        {
            var xml = builder.Build(TestResultFormat.xUnitXml);
            var serializer = new XUnitSerializer();
            var run = serializer.Deserialize(xml);
            return run;
        }

        /// <summary>
        /// Verifies that a test run with a single passed test is correctly marked as passed.
        /// </summary>
        [Fact]
        public void TestResultFileBuilder_WithOnePassedTests_IsPassed()
        {
            var builder = Create();
            builder.AddTestSuite()
                .SetName("Suite1")
                .AddTestCase().SetName("Test1").SetResult(TestResult.Passed)
                .AddTrait("Category", "UnitTest");

            TestRunDto run = BuildAndDeserialize(builder);

            // Assert
            Assert.NotNull(run);
            Assert.NotNull(run.Suites);
            Assert.Single(run.Suites);
            Assert.Single(run.Suites[0].Tests);

            var test = run.Suites[0].Tests[0];
            Assert.Equal("Test1", test.Name);
            Assert.Equal(TestResult.Passed, test.Result);
        }

        /// <summary>
        /// Verifies that a test run with a failed test is correctly marked as failed and includes the failure message.
        /// </summary>
        [Fact]
        public void TestResultFileBuilder_WithFailedTest_IsFailed()
        {
            var builder = Create();
            builder.AddTestSuite()
                .SetName("Suite2")
                .AddTestCase().SetName("Test2").SetResult(TestResult.Failed, "Failure message");

            TestRunDto run = BuildAndDeserialize(builder);

            Assert.NotNull(run);
            Assert.Single(run.Suites);
            Assert.Single(run.Suites[0].Tests);

            var test = run.Suites[0].Tests[0];
            Assert.Equal("Test2", test.Name);
            Assert.Equal(TestResult.Failed, test.Result);
            Assert.Equal("Failure message", test.Message);
        }

        /// <summary>
        /// Verifies that multiple test results are counted correctly for passed, failed, and skipped tests.
        /// </summary>
        [Fact]
        public void TestResultFileBuilder_WithMultipleTestResults_CountsCorrectly()
        {
            var builder = Create();
            var suite = builder.AddTestSuite().SetName("Suite3");
            suite.AddTestCase().SetName("TestA").SetResult(TestResult.Passed);
            suite.AddTestCase().SetName("TestB").SetResult(TestResult.Failed);
            suite.AddTestCase().SetName("TestC").SetResult(TestResult.Skipped);

            TestRunDto run = BuildAndDeserialize(builder);

            Assert.NotNull(run);
            Assert.Single(run.Suites);
            Assert.Equal(3, run.Suites[0].Tests.Count);

            Assert.Equal(1, run.Count(TestResult.Passed));
            Assert.Equal(1, run.Count(TestResult.Failed));
            Assert.Equal(1, run.Count(TestResult.Skipped));
        }

        /// <summary>
        /// Verifies that attachments added to a test case are present in the serialized and deserialized result.
        /// </summary>
        [Fact]
        public void TestResultFileBuilder_WithAttachment_AttachmentIsPresent()
        {
            var builder = Create();
            var data = Encoding.UTF8.GetBytes("dummy content");
            builder.AddTestSuite()
                .SetName("Suite4")
                .AddTestCase().SetName("TestWithAttachment")
                .SetResult(TestResult.Passed)
                .AddAttachment("log.txt", "text/plain", data);

            TestRunDto run = BuildAndDeserialize(builder);

            var test = run.Suites[0].Tests[0];
            Assert.NotNull(test.Attachments);
            Assert.Single(test.Attachments);
            Assert.Equal("log.txt", test.Attachments[0].Name);
            Assert.Equal("text/plain", test.Attachments[0].ContentType);
        }

        /// <summary>
        /// Verifies that custom traits added to a test case are present in the serialized and deserialized result.
        /// </summary>
        [Fact]
        public void TestResultFileBuilder_WithCustomTrait_TraitIsPresent()
        {
            var builder = Create();
            builder.AddTestSuite()
                .SetName("Suite5")
                .AddTestCase().SetName("TestWithTrait")
                .SetResult(TestResult.Passed)
                .AddTrait("CustomTrait", "CustomValue");

            TestRunDto run = BuildAndDeserialize(builder);

            var test = run.Suites[0].Tests[0];
            Assert.Contains(test.Traits, t => t.Name == "CustomTrait" && t.Value == "CustomValue");
        }

        /// <summary>
        /// Verifies that multiple test suites are present in the serialized and deserialized result.
        /// </summary>
        [Fact]
        public void TestResultFileBuilder_WithMultipleSuites_SuitesArePresent()
        {
            var builder = Create();
            builder.AddTestSuite().SetName("SuiteA").AddTestCase().SetName("Test1").SetResult(TestResult.Passed);
            builder.AddTestSuite().SetName("SuiteB").AddTestCase().SetName("Test2").SetResult(TestResult.Failed);

            TestRunDto run = BuildAndDeserialize(builder);

            Assert.Equal(2, run.Suites.Count);
            Assert.Equal("SuiteA", run.Suites[0].Name);
            Assert.Equal("SuiteB", run.Suites[1].Name);
        }
    }
}