﻿using TestBucket.Formats.Builders;
using TestBucket.Traits.Core;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests
{
    /// <summary>
    /// Contains unit tests for building and deserializing test result files in various formats.
    /// </summary>
    [FunctionalTest]
    [Component("Test Result Formats")]
    [Feature("Import Test Results")]
    [UnitTest]
    [EnrichedTest]
    public class TestResultBuilderTests
    {
        /// <summary>
        /// Verifies that building a test result file with two test cases and deserializing it produces the expected structure.
        /// </summary>
        /// <param name="format">The test result file format to use.</param>
        [Theory]
        [InlineData(TestResultFormat.MicrosoftTrx)]
        [InlineData(TestResultFormat.JUnitXml)]
        [InlineData(TestResultFormat.xUnitXml)]
        [InlineData(TestResultFormat.CommonTestReportFormat)]
        public void BuildtextWithTwoTests_ThenDeserializeIt_StructureMatches(TestResultFormat format)
        {
            var text = new TestResultFileBuilder()
                .SetName("run1")
                .AddTestSuite()
                .SetName("suite1")
                .AddTestCase().SetName("test1")
                .AddTestCase().SetName("test2")
                .Build(format);

            var serializer = TestResultSerializerFactory.Create(format);
            var run = serializer.Deserialize(text);

            Assert.NotNull(run.Suites);
            Assert.NotEmpty(run.Suites);
            Assert.Equal("run1", run.Name);
            Assert.Single(run.Suites);
            Assert.Equal(2, run.Suites[0].Tests.Count);
            Assert.Equal("test1", run.Suites[0].Tests[0].Name);
            Assert.Equal("test2", run.Suites[0].Tests[1].Name);
        }

        /// <summary>
        /// Verifies that a custom trait added to a test case is correctly deserialized.
        /// </summary>
        /// <param name="format">The test result file format to use.</param>
        [Theory]
        [InlineData(TestResultFormat.MicrosoftTrx)]
        [InlineData(TestResultFormat.JUnitXml)]
        [InlineData(TestResultFormat.CommonTestReportFormat)]
        public void BuildtextWithCustomTrait_ThenDeserializeIt_TraitMatches(TestResultFormat format)
        {
            var text = new TestResultFileBuilder()
                .SetName("run1")
                .AddTestSuite()
                .SetName("suite1")
                .AddTestCase().SetName("test1")
                .AddTrait("CustomTestCategory", "UnitTest")
                .Build(format);

            var serializer = TestResultSerializerFactory.Create(format);
            var run = serializer.Deserialize(text);

            Assert.NotNull(run.Suites);
            Assert.NotEmpty(run.Suites);
            Assert.Equal("run1", run.Name);
            Assert.Single(run.Suites);
            Assert.Single(run.Suites[0].Tests);
            var trait = run.Suites[0].Tests[0].Traits.Where(x => x.Name == "CustomTestCategory").FirstOrDefault();

            Assert.NotNull(trait);
            Assert.Equal("UnitTest", trait.Value);
        }

        /// <summary>
        /// Verifies that a known trait type added to a test case is correctly deserialized.
        /// </summary>
        /// <param name="format">The test result file format to use.</param>
        /// <param name="traitType">The trait type to add and verify.</param>
        [Theory]
        [InlineData(TestResultFormat.MicrosoftTrx, TraitType.TestCategory)]
        [InlineData(TestResultFormat.MicrosoftTrx, TraitType.TestPriority)]
        [InlineData(TestResultFormat.JUnitXml, TraitType.TestCategory)]
        [InlineData(TestResultFormat.JUnitXml, TraitType.TestPriority)]
        [InlineData(TestResultFormat.CommonTestReportFormat, TraitType.TestCategory)]
        [InlineData(TestResultFormat.CommonTestReportFormat, TraitType.TestPriority)]
        public void BuildtextWithKnownTrait_ThenDeserializeIt_TraitMatches(TestResultFormat format, TraitType traitType)
        {
            var text = new TestResultFileBuilder()
                .SetName("run1")
                .AddTestSuite()
                .SetName("suite1")
                .AddTestCase().SetName("test1")
                .AddTrait(traitType, "UnitTest")
                .Build(format);

            var serializer = TestResultSerializerFactory.Create(format);
            var run = serializer.Deserialize(text);

            Assert.NotNull(run.Suites);
            Assert.NotEmpty(run.Suites);
            Assert.Equal("run1", run.Name);
            Assert.Single(run.Suites);
            Assert.Single(run.Suites[0].Tests);
            var trait = run.Suites[0].Tests[0].Traits.Where(x => x.Type == traitType).FirstOrDefault();

            Assert.NotNull(trait);
            Assert.Equal("UnitTest", trait.Value);
        }

        /// <summary>
        /// Verifies that an attachment added to a test case is correctly deserialized and matches the original data.
        /// </summary>
        /// <param name="format">The test result file format to use.</param>
        [Theory]
        [InlineData(TestResultFormat.MicrosoftTrx)]
        [InlineData(TestResultFormat.xUnitXml)]
        [InlineData(TestResultFormat.JUnitXml)]
        [InlineData(TestResultFormat.CommonTestReportFormat)]
        public void BuildtextWithAttachment_ThenDeserializeIt_AttachmentIdentical(TestResultFormat format)
        {
            byte[] attachmentBytes = [1, 2, 3];

            var text = new TestResultFileBuilder()
                .SetName("run1")
                .AddTestSuite()
                .SetName("suite1")
                .AddTestCase().SetName("test1").AddAttachment("screenshot.png", "image/png", attachmentBytes)
                .Build(format);

            var serializer = TestResultSerializerFactory.Create(format);
            var run = serializer.Deserialize(text);

            Assert.NotNull(run.Suites);
            Assert.NotEmpty(run.Suites);
            Assert.Equal("run1", run.Name);
            Assert.Single(run.Suites);
            Assert.Single(run.Suites[0].Tests);
            Assert.Single(run.Suites[0].Tests[0].Attachments);
            var attachment = run.Suites[0].Tests[0].Attachments[0];
            Assert.NotNull(attachment);
            Assert.Equal("image/png", attachment.ContentType);
            Assert.Equal("screenshot.png", attachment.Name);
            Assert.Equal(attachmentBytes, attachment.Data);
        }

        /// <summary>
        /// Verifies that setting start and end times for a test run are correctly serialized and deserialized.
        /// </summary>
        /// <param name="format">The test result file format to use.</param>
        [Theory]
        [InlineData(TestResultFormat.MicrosoftTrx)]
        [InlineData(TestResultFormat.xUnitXml)]
        [InlineData(TestResultFormat.JUnitXml)]
        [InlineData(TestResultFormat.CommonTestReportFormat)]
        public void BuildtextWithStartAndEndTimes_ThenDeserializeIt_DatesMatch(TestResultFormat format)
        {
            var startDate = new DateTimeOffset(2025, 4, 1, 1, 2, 3, TimeSpan.Zero);
            var endDate = new DateTimeOffset(2025, 4, 2, 3, 4, 5, TimeSpan.Zero);
            var text = new TestResultFileBuilder()
                .SetName("run1")
                .SetTestExecutionDateRange(startDate, endDate)
                .Build(format);

            var serializer = TestResultSerializerFactory.Create(format);
            var run = serializer.Deserialize(text);

            Assert.NotNull(run.StartedTime);
            Assert.NotNull(run.EndedTime);
            Assert.Equal(startDate, run.StartedTime);
            Assert.Equal(endDate, run.EndedTime);
        }
    }
}