using TestBucket.CodeCoverage.Models;
using TestBucket.CodeCoverage.Parsers;

namespace TestBucket.CodeCoverage.Tests
{
    /// <summary>
    /// Contains unit tests for the <see cref="CodeCoverageParserFactory"/> class.
    /// Verifies correct parser creation and content type/format mapping logic.
    /// </summary>
    [Feature("Code Coverage")]
    [Component("Code Coverage")]
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class CodeCoverageParserFactoryTests
    {
        /// <summary>
        /// Verifies that creating a parser with an unknown format throws an <see cref="ArgumentException"/>.
        /// </summary>
        [Fact]
        public void Create_WithUnknownFormat_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => CodeCoverageParserFactory.Create(CodeCoverageFormat.UnknownFormat));
        }

        /// <summary>
        /// Verifies that the correct parser is created for the Cobertura MIME type.
        /// </summary>
        [Fact]
        public void CreateFromContentType_WithCoberturaMime_CorrectParserCreated()
        {
            var parser = CodeCoverageParserFactory.CreateFromContentType("application/x-cobertura");
            Assert.True(parser is CoberturaParser);
        }

        /// <summary>
        /// Verifies that the correct content type is returned for the Cobertura format.
        /// </summary>
        [Fact]
        public void GetContentTypeFromFormat_WithCobertura_CorrectContentTypeReturned()
        {
            var mediaType = CodeCoverageParserFactory.GetContentTypeFromFormat(CodeCoverageFormat.Cobertura);
            Assert.Equal("application/x-cobertura", mediaType);
        }

        /// <summary>
        /// Verifies that null is returned for an unknown code coverage format.
        /// </summary>
        [Fact]
        public void GetContentTypeFromFormat_WithUnknownFormat_NullReturned()
        {
            var mediaType = CodeCoverageParserFactory.GetContentTypeFromFormat(CodeCoverageFormat.UnknownFormat);
            Assert.Null(mediaType);
        }

        /// <summary>
        /// Verifies that the correct format is returned for the Cobertura content type.
        /// </summary>
        [Fact]
        public void GetFormatFromContentType_WithCobertura_CorrectContentTypeReturned()
        {
            var format = CodeCoverageParserFactory.GetFormatFromContentType("application/x-cobertura");
            Assert.Equal(CodeCoverageFormat.Cobertura, format);
        }

        /// <summary>
        /// Verifies that <see cref="CodeCoverageFormat.UnknownFormat"/> is returned when the content type is null.
        /// </summary>
        [Fact]
        public void GetFormatFromContentType_WithNull_UnknownFormatReturned()
        {
            var format = CodeCoverageParserFactory.GetFormatFromContentType(null);
            Assert.Equal(CodeCoverageFormat.UnknownFormat, format);
        }

        /// <summary>
        /// Verifies that <see cref="CodeCoverageFormat.UnknownFormat"/> is returned for an unsupported content type.
        /// </summary>
        [Fact]
        public void GetFormatFromContentType_WithUnsupportedContentType_UnknownFormatReturned()
        {
            var format = CodeCoverageParserFactory.GetFormatFromContentType("text/plain");
            Assert.Equal(CodeCoverageFormat.UnknownFormat, format);
        }
    }
}