using System.Globalization;
using TestBucket.Contracts;
using TestBucket.Domain.Search;

namespace TestBucket.Domain.UnitTests.Search
{
    /// <summary>
    /// Contains unit tests for the <see cref="BaseQueryParser"/> class, verifying serialization and parsing of <see cref="SearchQuery"/> objects.
    /// </summary>
    [Feature("Search")]
    [UnitTest]
    [Component("Search")]
    [EnrichedTest]
    [FunctionalTest]
    public class BaseQueryParserTests
    {
        /// <summary>
        /// Tests that <see cref="BaseQueryParser.Serialize"/> includes all set properties of a <see cref="SearchQuery"/> in the output list.
        /// </summary>
        [Fact]
        public void Serialize_ShouldIncludeAllSetProperties()
        {
            var query = new SearchQuery
            {
                TeamId = 42,
                ProjectId = 99,
                Since = "2d",
                CreatedUntil = new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero)
            };
            var items = new List<string>();

            BaseQueryParser.Serialize(query, items);

            Assert.Contains("since:2d", items);
            Assert.Contains("team-id:42", items);
            Assert.Contains("project-id:99", items);
            Assert.Contains("until:\"2024-06-01 12:00:00\"", items);
        }

        /// <summary>
        /// Tests that <see cref="BaseQueryParser.Parse"/> correctly sets properties on a <see cref="SearchQuery"/> from a dictionary of values.
        /// </summary>
        [Fact]
        public void Parse_WithSince_ShouldSetPropertiesFromDictionary()
        {
            var dict = new Dictionary<string, string>
            {
                { "since", "1d" },
                { "team-id", "123" },
                { "project-id", "456" },
                { "until", "2024-06-01" }
            };
            var query = new SearchQuery();
            var fakeNow = new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);
            var provider = new FakeTimeProvider(fakeNow, TimeSpan.Zero);

            BaseQueryParser.Parse(query, dict, provider);

            Assert.Equal(123, query.TeamId);
            Assert.Equal(456, query.ProjectId);
            Assert.Equal("1d", query.Since);

            var expectedFrom = new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero) - TimeSpan.FromDays(1);
            Assert.Equal(expectedFrom, query.CreatedFrom);
            Assert.Equal(new DateTimeOffset(2024, 6, 1, 0, 0, 0, TimeSpan.Zero), query.CreatedUntil);
        }

        /// <summary>
        /// Tests that <see cref="BaseQueryParser.Parse"/> correctly sets properties on a <see cref="SearchQuery"/> from a dictionary of values.
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(8)]
        [InlineData(-12)]
        public void Parse_WithFrom_ShouldSetPropertiesFromDictionary(int timeZoneOffsetUtcInHours)
        {
            var dict = new Dictionary<string, string>
            {
                { "team-id", "123" },
                { "project-id", "456" },
                { "from", "2024-05-01 12:00:00" },
                { "until", "2024-06-01" }
            };
            var query = new SearchQuery();
            var fakeNow = new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);
            var provider = new FakeTimeProvider(fakeNow, TimeSpan.FromHours(timeZoneOffsetUtcInHours));

            BaseQueryParser.Parse(query, dict, provider);

            Assert.Equal(123, query.TeamId);
            Assert.Equal(456, query.ProjectId);

            var expectedFrom = new DateTimeOffset(2024, 5, 1, 12, 0, 0, provider.LocalTimeZone.BaseUtcOffset);
            var expectedUntil = new DateTimeOffset(2024, 6, 1, 0, 0, 0, provider.LocalTimeZone.BaseUtcOffset);
            Assert.Equal(expectedFrom, query.CreatedFrom);
            Assert.Equal(expectedUntil, query.CreatedUntil);
        }

        /// <summary>
        /// Tests that <see cref="BaseQueryParser.ParseSince(string)"/> correctly parses valid time span strings with supported units.
        /// </summary>
        [Theory]
        [InlineData("2w", 14, 0, 0, 0)] // 2 weeks = 14 days
        [InlineData("3d", 3, 0, 0, 0)]  // 3 days
        [InlineData("4h", 0, 4, 0, 0)]  // 4 hours
        [InlineData("30m", 0, 0, 30, 0)] // 30 minutes
        [InlineData("45s", 0, 0, 0, 45)] // 45 seconds
        [InlineData("1y", 365, 0, 0, 0)] // 1 year = 365 days
        public void ParseSince_ShouldParseValidInputs(string input, int days, int hours, int minutes, int seconds)
        {
            var expected = new TimeSpan(days, hours, minutes, seconds);
            var actual = BaseQueryParser.ParseSince(input);
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Tests that <see cref="BaseQueryParser.ParseSince(string)"/> throws <see cref="FormatException"/> for empty input.
        /// </summary>
        [Fact]
        public void ParseSince_ShouldThrowFormatException_OnEmptyInput()
        {
            Assert.Throws<FormatException>(() => BaseQueryParser.ParseSince(""));
        }

        /// <summary>
        /// Tests that <see cref="BaseQueryParser.ParseSince(string)"/> throws <see cref="FormatException"/> for input with invalid number.
        /// </summary>
        [Fact]
        public void ParseSince_ShouldThrowFormatException_OnInvalidNumber()
        {
            Assert.Throws<FormatException>(() => BaseQueryParser.ParseSince("xd"));
        }

        /// <summary>
        /// Tests that <see cref="BaseQueryParser.ParseSince(string)"/> throws <see cref="FormatException"/> for input with unsupported unit.
        /// </summary>
        [Fact]
        public void ParseSince_ShouldThrowFormatException_OnUnsupportedUnit()
        {
            Assert.Throws<FormatException>(() => BaseQueryParser.ParseSince("5q"));
        }


        /// <summary>
        /// Tests that <see cref="BaseQueryParser.TryParseDateTimeOffset"/> returns true and parses a valid date string without a format.
        /// </summary>
        [Fact]
        public void TryParseDateTimeOffset_ValidDateWithoutFormat_ReturnsTrue()
        {
            var provider = new FakeTimeProvider(new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.FromHours(2)), TimeSpan.FromHours(2));
            var result = BaseQueryParser.TryParseDateTimeOffset("2024-06-01 15:30:00", null, provider, out var dto);

            Assert.True(result);
            Assert.NotNull(dto);
            Assert.Equal(new DateTimeOffset(2024, 6, 1, 15, 30, 0, provider.LocalTimeZone.BaseUtcOffset), dto);
        }

        /// <summary>
        /// Tests that <see cref="BaseQueryParser.TryParseDateTimeOffset"/> returns true and parses a valid date string with a specific format.
        /// </summary>
        [Fact]
        public void TryParseDateTimeOffset_ValidDateWithFormat_ReturnsTrue()
        {
            var provider = new FakeTimeProvider(new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.FromHours(1)), TimeSpan.FromHours(1));
            var format = "yyyy-MM-dd HH:mm:ss";
            var result = BaseQueryParser.TryParseDateTimeOffset("2024-06-01 08:00:00", format, provider, out var dto);

            Assert.True(result);
            Assert.NotNull(dto);
            Assert.Equal(new DateTimeOffset(2024, 6, 1, 8, 0, 0, provider.LocalTimeZone.BaseUtcOffset), dto);
        }

        /// <summary>
        /// Tests that <see cref="BaseQueryParser.TryParseDateTimeOffset"/> returns false for an invalid date string.
        /// </summary>
        [Fact]
        public void TryParseDateTimeOffset_InvalidDate_ReturnsFalse()
        {
            var provider = new FakeTimeProvider(new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero), TimeSpan.Zero);
            var result = BaseQueryParser.TryParseDateTimeOffset("not-a-date", null, provider, out var dto);

            Assert.False(result);
            Assert.Null(dto);
        }

        /// <summary>
        /// Tests that <see cref="BaseQueryParser.TryParseDateTimeOffset"/> returns false for a valid date string with a mismatched format.
        /// </summary>
        [Fact]
        public void TryParseDateTimeOffset_ValidDateWithWrongFormat_ReturnsFalse()
        {
            var provider = new FakeTimeProvider(new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero), TimeSpan.Zero);
            var format = "yyyy/MM/dd";
            var result = BaseQueryParser.TryParseDateTimeOffset("2024-06-01", format, provider, out var dto);

            Assert.False(result);
            Assert.Null(dto);
        }
    }
}