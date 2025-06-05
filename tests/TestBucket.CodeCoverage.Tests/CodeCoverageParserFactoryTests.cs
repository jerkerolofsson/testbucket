using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.CodeCoverage.Models;
using TestBucket.CodeCoverage.Parsers;

namespace TestBucket.CodeCoverage.Tests
{
    [Feature("Code Coverage")]
    [Component("Code Coverage")]
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class CodeCoverageParserFactoryTests
    {
        [Fact]
        public void CreateFromContentType_WithCoberturaMime_CorrectParserCreated()
        {
            var parser = CodeCoverageParserFactory.CreateFromContentType("application/x-cobertura");
            Assert.True(parser is CoberturaParser);
        }

        [Fact]
        public void GetContentTypeFromFormat_WithCobertura_CorrectContentTypeReturned()
        {
            var mediaType = CodeCoverageParserFactory.GetContentTypeFromFormat(CodeCoverageFormat.Cobertura);
            Assert.Equal("application/x-cobertura", mediaType);
        }

        [Fact]
        public void GetFormatFromContentType_WithCobertura_CorrectContentTypeReturned()
        {
            var format = CodeCoverageParserFactory.GetFormatFromContentType("application/x-cobertura");
            Assert.Equal(CodeCoverageFormat.Cobertura, format);
        }
    }
}
