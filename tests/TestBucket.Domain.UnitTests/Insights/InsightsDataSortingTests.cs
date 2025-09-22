using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Insights.Extensions;
using TestBucket.Contracts.Insights;
using System.Globalization;

namespace TestBucket.Domain.UnitTests.Insights
{
    /// <summary>
    /// Contains unit tests for verifying the sorting behavior of <see cref="InsightsData{TLabel, TValue}"/> and its series.
    /// </summary>
    [Component("Insights")]
    [Feature("Insights")]
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class InsightsDataSortingTests
    {
        /// <summary>
        /// Verifies that the labels are returned in the correct order when sorting by label ascending.
        /// </summary>
        [Fact]
        public void SortLabels_ByLabelAscending()
        {
            // Arrange
            var data = new InsightsData<DateOnly, int>();
            var series = data.Add("series 1");
            series.Add(new DateOnly(2023, 1, 1), 10);
            series.Add(new DateOnly(2023, 1, 3), 30);
            series.Add(new DateOnly(2023, 1, 2), 20);
            series.SortBy = InsightsSort.LabelAscending;

            // Act
            var labels = data.Series[0].Labels.ToList();

            // Assert
            Assert.Equal(1, labels[0].Day);
            Assert.Equal(2, labels[1].Day);
            Assert.Equal(3, labels[2].Day);
        }

        /// <summary>
        /// Verifies that the labels are returned in the correct order when sorting dates even if DateOnly labels are converted to strings and the format is not
        /// lexicographically sortable.
        /// </summary>
        [Fact]
        public void SortDateOnlyLabels_ByLabelAscending_AfterStringConversion()
        {
            // Arrange
            var data = new InsightsData<DateOnly, int>();
            var series = data.Add("series 1");
            series.Add(new DateOnly(2023, 1, 2), 10);
            series.Add(new DateOnly(2024, 1, 3), 30);
            series.Add(new DateOnly(2025, 1, 1), 20);
            series.SortBy = InsightsSort.LabelAscending;

            // Use a data format that isn't sorted properly as strings
            var convertedData = data.ConvertToStringLabels(label => label.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

            // Act
            var labels = convertedData.Series[0].Labels.ToList();

            // Assert
            Assert.Equal("01/02/2023", labels[0]);
            Assert.Equal("01/03/2024", labels[1]);
            Assert.Equal("01/01/2025", labels[2]);
        }

        /// <summary>
        /// Verifies that the labels are returned in the correct order when sorting dates even if DateTime labels are converted to strings and the format is not
        /// lexicographically sortable.
        /// </summary>
        [Fact]
        public void SortDateTimeLabels_ByLabelAscending_AfterStringConversion()
        {
            // Arrange
            var data = new InsightsData<DateTime, int>();
            var series = data.Add("series 1");
            series.Add(new DateTime(2023, 1, 2), 10);
            series.Add(new DateTime(2024, 1, 3), 30);
            series.Add(new DateTime(2025, 1, 1), 20);
            series.SortBy = InsightsSort.LabelAscending;

            // Use a data format that isn't sorted properly as strings
            var convertedData = data.ConvertToStringLabels(label => label.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

            // Act
            var labels = convertedData.Series[0].Labels.ToList();

            // Assert
            Assert.Equal("01/02/2023", labels[0]);
            Assert.Equal("01/03/2024", labels[1]);
            Assert.Equal("01/01/2025", labels[2]);
        }


        /// <summary>
        /// Verifies that the labels are returned in the correct order when sorting dates even if DateTimeOffset labels are converted to strings and the format is not
        /// lexicographically sortable.
        /// </summary>
        [Fact]
        public void SortDateTimeOffsetLabels_ByLabelAscending_AfterStringConversion()
        {
            // Arrange
            var data = new InsightsData<DateTimeOffset, int>();
            var series = data.Add("series 1");
            series.Add(new DateTimeOffset(2023, 1, 2, 0,0,0,TimeSpan.Zero), 10);
            series.Add(new DateTimeOffset(2024, 1, 3, 0, 0, 0, TimeSpan.Zero), 30);
            series.Add(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero), 20);
            series.SortBy = InsightsSort.LabelAscending;

            // Use a data format that isn't sorted properly as strings
            var convertedData = data.ConvertToStringLabels(label => label.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

            // Act
            var labels = convertedData.Series[0].Labels.ToList();

            // Assert
            Assert.Equal("01/02/2023", labels[0]);
            Assert.Equal("01/03/2024", labels[1]);
            Assert.Equal("01/01/2025", labels[2]);
        }

        /// <summary>
        /// Verifies that the labels are returned in the correct order when sorting by label descending.
        /// </summary>
        [Fact]
        public void SortLabels_ByLabelDescending()
        {
            // Arrange
            var data = new InsightsData<DateOnly, int>();
            var series = data.Add("series 1");
            series.Add(new DateOnly(2023, 1, 1), 10);
            series.Add(new DateOnly(2023, 1, 3), 30);
            series.Add(new DateOnly(2023, 1, 2), 20);
            series.SortBy = InsightsSort.LabelDescending;

            // Act
            var labels = data.Series[0].Labels.ToList();

            // Assert
            Assert.Equal(1, labels[2].Day);
            Assert.Equal(2, labels[1].Day);
            Assert.Equal(3, labels[0].Day);
        }

        /// <summary>
        /// Verifies that the values are returned in the correct order when sorting by value descending.
        /// </summary>
        [Fact]
        public void SortLabels_ByValueDescending()
        {
            // Arrange
            var data = new InsightsData<DateOnly, int>();
            var series = data.Add("series 1");
            series.Add(new DateOnly(2023, 1, 1), 10);
            series.Add(new DateOnly(2023, 1, 3), 30);
            series.Add(new DateOnly(2023, 1, 2), 20);
            series.SortBy = InsightsSort.ValueDescending;

            // Act
            var values = data.Series[0].Values.ToList();

            // Assert
            Assert.Equal(30, values[0]);
            Assert.Equal(20, values[1]);
            Assert.Equal(10, values[2]);
        }

        /// <summary>
        /// Verifies that the values are returned in the correct order when sorting by value ascending.
        /// </summary>
        [Fact]
        public void SortLabels_ByValueAscending()
        {
            // Arrange
            var data = new InsightsData<DateOnly, int>();
            var series = data.Add("series 1");
            series.Add(new DateOnly(2023, 1, 1), 10);
            series.Add(new DateOnly(2023, 1, 3), 30);
            series.Add(new DateOnly(2023, 1, 2), 20);
            series.SortBy = InsightsSort.ValueAscending;

            // Act
            var values = data.Series[0].Values.ToList();

            // Assert
            Assert.Equal(10, values[0]);
            Assert.Equal(20, values[1]);
            Assert.Equal(30, values[2]);
        }
    }
}