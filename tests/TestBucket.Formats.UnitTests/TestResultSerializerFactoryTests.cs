using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Traits.Core;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests
{
    [FunctionalTest]
    [Component("Test Result Formats")]
    [Feature("Import Test Results")]
    [UnitTest]
    [EnrichedTest]
    public class TestResultSerializerFactoryTests
    {
        /// <summary>
        /// Verifies that the correct test result format is resolved with various media-types.
        /// </summary>
        /// <param name="expectedFormat">The expected <see cref="TestResultFormat"/> to be resolved.</param>
        /// <param name="contentType">The content type string to test.</param>
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

        /// <summary>
        /// Verifies that an unknown content type returns <see cref="TestResultFormat.UnknownFormat"/>.
        /// </summary>
        [Fact]
        public void GetFormatFromContentType_WithUnknownFormat_ReturnsUnknownFormat()
        {
            var format = TestResultSerializerFactory.GetFormatFromContentType("application/vnd.this-is-not-a-known-media-type");
            Assert.Equal(TestResultFormat.UnknownFormat, format);
        }

        /// <summary>
        /// Verifies that passing null as content type returns <see cref="TestResultFormat.UnknownFormat"/>.
        /// </summary>
        [Fact]
        public void GetFormatFromContentType_WithNull_ReturnsUnknownFormat()
        {
            var format = TestResultSerializerFactory.GetFormatFromContentType(null);
            Assert.Equal(TestResultFormat.UnknownFormat, format);
        }

        /// <summary>
        /// Verifies that the <see cref="TestResultSerializerFactory.Create"/> method does not throw for supported formats.
        /// </summary>
        /// <param name="format">The <see cref="TestResultFormat"/> to test.</param>
        [Theory]
        [InlineData(TestResultFormat.xUnitXml)]
        [InlineData(TestResultFormat.JUnitXml)]
        [InlineData(TestResultFormat.MicrosoftTrx)]
        [InlineData(TestResultFormat.CommonTestReportFormat)]
        public void Create_NoExceptionThrow(TestResultFormat format)
        {
            var _ = TestResultSerializerFactory.Create(format);
        }

        /// <summary>
        /// Verifies that <see cref="TestResultSerializerFactory.GetContentTypeFromFormat"/> returns the expected content type for each format.
        /// </summary>
        /// <param name="format">The <see cref="TestResultFormat"/> to test.</param>
        /// <param name="expectedContentType">The expected content type string.</param>
        [Theory]
        [InlineData(TestResultFormat.MicrosoftTrx, "application/x-trx")]
        [InlineData(TestResultFormat.JUnitXml, "application/x-junit")]
        [InlineData(TestResultFormat.xUnitXml, "application/x-xunit")]
        [InlineData(TestResultFormat.NUnitXml, "application/x-nunit")]
        [InlineData(TestResultFormat.CommonTestReportFormat, "application/x-ctrf")]
        public void GetContentTypeFromFormat_ReturnsExpectedContentType(TestResultFormat format, string expectedContentType)
        {
            var contentType = TestResultSerializerFactory.GetContentTypeFromFormat(format);
            Assert.Equal(expectedContentType, contentType);
        }

        /// <summary>
        /// Verifies that <see cref="TestResultSerializerFactory.GetContentTypeFromFormat"/> returns null for unknown formats.
        /// </summary>
        [Fact]
        public void GetContentTypeFromFormat_UnknownFormat_ReturnsNull()
        {
            var contentType = TestResultSerializerFactory.GetContentTypeFromFormat(TestResultFormat.UnknownFormat);
            Assert.Null(contentType);
        }

        /// <summary>
        /// Verifies that <see cref="TestResultSerializerFactory.CreateFromContentType"/> returns the correct serializer type for each supported content type.
        /// </summary>
        /// <param name="contentType">The content type string to test.</param>
        /// <param name="expectedType">The expected serializer type.</param>
        [Theory]
        [InlineData("application/x-trx", typeof(TestBucket.Formats.MicrosoftTrx.TrxSerializer))]
        [InlineData("application/x-junit", typeof(TestBucket.Formats.JUnit.JUnitSerializer))]
        [InlineData("application/x-xunit", typeof(TestBucket.Formats.XUnit.XUnitSerializer))]
        [InlineData("application/x-ctrf", typeof(TestBucket.Formats.Ctrf.CtrfXunitSerializer))]
        public void CreateFromContentType_ReturnsCorrectSerializerType(string contentType, Type expectedType)
        {
            var serializer = TestResultSerializerFactory.CreateFromContentType(contentType);
            Assert.IsType(expectedType, serializer);
        }

        /// <summary>
        /// Verifies that <see cref="TestResultSerializerFactory.Create"/> throws <see cref="NotImplementedException"/> for unknown formats.
        /// </summary>
        [Fact]
        public void Create_ThrowsNotImplementedException_ForUnknownFormat()
        {
            Assert.Throws<NotImplementedException>(() => TestResultSerializerFactory.Create(TestResultFormat.UnknownFormat));
        }
    }
}
