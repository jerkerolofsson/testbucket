using System;
using TestBucket.Domain.Search;
using TestBucket.Domain.Search.Models;
using Xunit;

namespace TestBucket.Domain.UnitTests.Search
{
    /// <summary>
    /// Unit tests for <see cref="NumericalFilterParser"/>.
    /// </summary>
    public class NumericalFilterParserTests
    {
        /// <summary>
        /// Tests parsing the 'greater than' operator ('&gt;').
        /// </summary>
        [Fact]
        public void Parse_GreaterThanOperator_ReturnsCorrectFilter()
        {
            var filter = NumericalFilterParser.Parse(">42.5");
            Assert.Equal(FilterOperator.GreaterThan, filter.Operator);
            Assert.Equal(42.5, filter.Value);
        }

        /// <summary>
        /// Tests parsing the 'greater than or equal' operator ('&gt;=').
        /// </summary>
        [Fact]
        public void Parse_GreaterThanOrEqualOperator_ReturnsCorrectFilter()
        {
            var filter = NumericalFilterParser.Parse(">=100");
            Assert.Equal(FilterOperator.GreaterThanOrEqual, filter.Operator);
            Assert.Equal(100, filter.Value);
        }

        /// <summary>
        /// Tests parsing the 'less than' operator ('&lt;').
        /// </summary>
        [Fact]
        public void Parse_LessThanOperator_ReturnsCorrectFilter()
        {
            var filter = NumericalFilterParser.Parse("<0");
            Assert.Equal(FilterOperator.LessThan, filter.Operator);
            Assert.Equal(0, filter.Value);
        }

        /// <summary>
        /// Tests parsing the 'less than or equal' operator ('&lt;=').
        /// </summary>
        [Fact]
        public void Parse_LessThanOrEqualOperator_ReturnsCorrectFilter()
        {
            var filter = NumericalFilterParser.Parse("<=-5.25");
            Assert.Equal(FilterOperator.LessThanOrEqual, filter.Operator);
            Assert.Equal(-5.25, filter.Value);
        }

        /// <summary>
        /// Tests parsing the 'equals' operator ('==').
        /// </summary>
        [Fact]
        public void Parse_EqualsOperator_ReturnsCorrectFilter()
        {
            var filter = NumericalFilterParser.Parse("==123.456");
            Assert.Equal(FilterOperator.Equals, filter.Operator);
            Assert.Equal(123.456, filter.Value);
        }

        /// <summary>
        /// Tests parsing the 'equals' operator ('!=').
        /// </summary>
        [Fact]
        public void Parse_NotEqualsOperator_ReturnsCorrectFilter()
        {
            var filter = NumericalFilterParser.Parse("!=123.456");
            Assert.Equal(FilterOperator.NotEquals, filter.Operator);
            Assert.Equal(123.456, filter.Value);
        }

        /// <summary>
        /// Tests that parsing a non-numeric value throws a <see cref="FormatException"/>.
        /// </summary>
        [Fact]
        public void Parse_InvalidValue_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => NumericalFilterParser.Parse(">abc"));
        }

        /// <summary>
        /// Tests that parsing a null or whitespace string throws an <see cref="ArgumentException"/>.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Parse_NullOrWhitespace_ThrowsArgumentException(string input)
        {
            Assert.Throws<ArgumentException>(() => NumericalFilterParser.Parse(input));
        }
    }
}