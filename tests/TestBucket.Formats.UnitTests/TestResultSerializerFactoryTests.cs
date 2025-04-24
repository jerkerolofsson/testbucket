using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Traits.Core;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests
{
    [Feature("Import Test Results")]
    [Component("TestResultSerializerFactory")]
    [UnitTest]
    [EnrichedTest]
    public class TestResultSerializerFactoryTests
    {
        [TestDescription("Verifies that the correct test result format is resolved with various media-types")]
        [Theory]
        [InlineData(TestResultFormat.MicrosoftTrx, "application/x-trx")]
        [InlineData(TestResultFormat.MicrosoftTrx, "application/trx")]
        [InlineData(TestResultFormat.MicrosoftTrx, "text/xml+trx")]
        [InlineData(TestResultFormat.MicrosoftTrx, "application/xml+trx")]
        [InlineData(TestResultFormat.xUnitXml, "application/x-xunit")]
        [InlineData(TestResultFormat.xUnitXml, "application/xunit")]
        [InlineData(TestResultFormat.xUnitXml, "text/xml+xunit")]
        [InlineData(TestResultFormat.xUnitXml, "application/xml+xunit")]
        [InlineData(TestResultFormat.NUnitXml, "application/x-nunit")]
        [InlineData(TestResultFormat.NUnitXml, "application/nunit")]
        [InlineData(TestResultFormat.NUnitXml, "text/xml+nunit")]
        [InlineData(TestResultFormat.NUnitXml, "application/xml+nunit")]
        [InlineData(TestResultFormat.JUnitXml, "application/x-junit")]
        [InlineData(TestResultFormat.JUnitXml, "application/junit")]
        [InlineData(TestResultFormat.JUnitXml, "text/xml+junit")]
        [InlineData(TestResultFormat.JUnitXml, "application/xml+junit")]
        [InlineData(TestResultFormat.CommonTestReportFormat, "application/x-ctrf")]
        [InlineData(TestResultFormat.CommonTestReportFormat, "application/json+ctrf")]
        [InlineData(TestResultFormat.CommonTestReportFormat, "application/json")]
        public void GetFormatFromContentType_With(TestResultFormat expectedFormat, string contentType)
        {
            var format = TestResultSerializerFactory.GetFormatFromContentType(contentType);
            Assert.Equal(expectedFormat, format);
        }

        [Fact]
        public void GetFormatFromContentType_WithUnknownFormat_ReturnsUnknownFormat()
        {
            var format = TestResultSerializerFactory.GetFormatFromContentType("application/vnd.this-is-not-a-known-media-type");
            Assert.Equal(TestResultFormat.UnknownFormat, format);
        }
        [Fact]
        public void GetFormatFromContentType_WithNull_ReturnsUnknownFormat()
        {
            var format = TestResultSerializerFactory.GetFormatFromContentType(null);
            Assert.Equal(TestResultFormat.UnknownFormat, format);
        }

        [Theory]
        [InlineData(TestResultFormat.xUnitXml)]
        [InlineData(TestResultFormat.JUnitXml)]
        [InlineData(TestResultFormat.MicrosoftTrx)]
        [InlineData(TestResultFormat.CommonTestReportFormat)]
        public void Create_NoExceptionThrow(TestResultFormat format)
        {
            var _ = TestResultSerializerFactory.Create(format);
        }
    }
}
