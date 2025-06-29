using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Insights.Extensions;
using TestBucket.Contracts.Insights;

namespace TestBucket.Domain.UnitTests.Insights
{
    /// <summary>
    /// Contains unit tests for the <see cref="InsightsData{TLabel, TValue}"/> and related insights data operations.
    /// </summary>
    [Component("Insights")]
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class InsightsDataTests
    {
        /// <summary>
        /// Verifies that when insights data contains two series, a header/column is created for each series.
        /// </summary>
        [Fact]
        public void ToTable_WithMultipleSeries_TableContainsAllHeaders()
        {
            // Arrange
            var spec = new InsightsVisualizationSpecification() { Name = "name1" };
            var data = new InsightsData<string, double>();
            var series1 = data.Add("series 1");
            series1.Add("label1", 1);
            series1.Add("label2", 2);

            var series2 = data.Add("series 2");
            series2.Add("label2", 2);
            series2.Add("label3", 3);

            // Act
            var table = spec.ToTable(data);

            // Assert
            Assert.Equal(3, table.Headers.Count);
            Assert.Equal("", table.Headers[0]);
            Assert.Equal("series 1", table.Headers[1]);
            Assert.Equal("series 2", table.Headers[2]);
        }

        /// <summary>
        /// Verifies that missing labels can be added to insights data series.
        /// </summary>
        [Fact]
        public void AddMissingLabels_ToDateOnlySeries_MissingDatesAreAdded()
        {
            // Arrange
            var data = new InsightsData<string, double>();
            var series = data.Add("series 1");
            series.Add("A", 10);
            series.Add("B", 10);
            series.Add("D", 10);

            // Act
            data.AddMissingLabels(["C"]);

            // Assert
            var expected = "D";
            Assert.Equal(4, series.Data.Count());
            Assert.Equal(4, series.Labels.Count());
            Assert.Equal(4, series.Values.Count());
            Assert.Contains(expected, series.Labels);
        }

        /// <summary>
        /// Verifies that missing dates are added to insights data series.
        /// For example, if the labels are: "2023-01-01", "2023-01-02", and "2023-01-04" then 2023-01-03 should be added.
        /// </summary>
        [Fact]
        public void AddMissingDates_ToDateOnlySeries_MissingDatesAreAdded()
        {
            // Arrange
            var data = new InsightsData<DateOnly, int>();
            var series = data.Add("series 1");
            series.Add(new DateOnly(2023, 1, 1), 10);
            series.Add(new DateOnly(2023, 1, 2), 20);
            series.Add(new DateOnly(2023, 1, 4), 30);

            // Act
            data.AddMissingDays();

            // Assert
            var expected = new DateOnly(2023, 1, 3);
            Assert.Equal(4, series.Data.Count());
            Assert.Equal(4, series.Labels.Count());
            Assert.Equal(4, series.Values.Count());
            Assert.Contains(expected, series.Labels);
            Assert.Equal(2, series.Labels.ToList().IndexOf(expected));
        }

        /// <summary>
        /// Verifies that missing dates are added to insights data series when there are multiple series.
        /// Series1: "2023-01-01", "2023-01-02", and "2023-01-03"
        /// Series2: "2023-01-02", and "2023-01-03"
        /// Result: Series2 should contain "2023-01-01".
        /// </summary>
        [Fact]
        public void AddMissingDates_WithMultipleSeriesAndGapAtStart_MissingDatesAreAdded()
        {
            // Arrange
            var data = new InsightsData<DateOnly, int>();
            var series1 = data.Add("series 1");
            series1.Add(new DateOnly(2023, 1, 1), 10);
            series1.Add(new DateOnly(2023, 1, 2), 20);
            series1.Add(new DateOnly(2023, 1, 3), 30);

            var series2 = data.Add("series 2");
            series2.Add(new DateOnly(2023, 1, 2), 20);
            series2.Add(new DateOnly(2023, 1, 3), 30);

            // Act
            data.AddMissingDays();

            // Assert
            var expected = new DateOnly(2023, 1, 1);
            Assert.Equal(3, series1.Labels.Count());
            Assert.Equal(3, series1.Values.Count());
            Assert.Contains(expected, series1.Labels);
            Assert.Contains(expected, series2.Labels);
        }

        /// <summary>
        /// Verifies that missing dates are added to insights data series when there are multiple series.
        /// Series1: "2023-01-01", "2023-01-02", and "2023-01-03"
        /// Series2: "2023-01-01", and "2023-01-02"
        /// Result: Series2 should contain "2023-01-03".
        /// </summary>
        [Fact]
        public void AddMissingDates_WithMultipleSeriesAndGapAtEnd_MissingDatesAreAdded()
        {
            // Arrange
            var data = new InsightsData<DateOnly, int>();
            var series1 = data.Add("series 1");
            series1.Add(new DateOnly(2023, 1, 1), 10);
            series1.Add(new DateOnly(2023, 1, 2), 20);
            series1.Add(new DateOnly(2023, 1, 3), 30);

            var series2 = data.Add("series 2");
            series2.Add(new DateOnly(2023, 1, 1), 20);
            series2.Add(new DateOnly(2023, 1, 2), 30);

            // Act
            data.AddMissingDays();

            // Assert
            var expected = new DateOnly(2023, 1, 3);
            Assert.Equal(3, series1.Labels.Count());
            Assert.Equal(3, series1.Values.Count());
            Assert.Contains(expected, series1.Labels);
            Assert.Contains(expected, series2.Labels);
        }

        /// <summary>
        /// Verifies that the first day is returned from a sorted list.
        /// </summary>
        [Fact]
        public void GetStartDay_WithDateOnlyLabelsSeriesInSortedOrder_LastDayReturned()
        {
            // Arrange
            var data = new InsightsData<DateOnly, int>();
            var series = data.Add("series 1");
            series.Add(new DateOnly(2023, 1, 1), 10);
            series.Add(new DateOnly(2023, 1, 2), 20);
            series.Add(new DateOnly(2023, 1, 4), 30);

            // Act
            var lastDay = data.GetStartDay();

            // Assert
            var expected = new DateOnly(2023, 1, 1);
            Assert.Equal(expected, lastDay);
        }

        /// <summary>
        /// Verifies that the last day is returned from a sorted list.
        /// </summary>
        [Fact]
        public void GetEndDay_WithDateOnlyLabelsSeriesInSortedOrder_LastDayReturned()
        {
            // Arrange
            var data = new InsightsData<DateOnly, int>();
            var series = data.Add("series 1");
            series.Add(new DateOnly(2023, 1, 1), 10);
            series.Add(new DateOnly(2023, 1, 2), 20);
            series.Add(new DateOnly(2023, 1, 4), 30);

            // Act
            var endDay = data.GetEndDay();

            // Assert
            var expected = new DateOnly(2023, 1, 4);
            Assert.Equal(expected, endDay);
        }

        /// <summary>
        /// Verifies that the last day is returned from an unsorted list.
        /// </summary>
        [Fact]
        public void GetEndDay_WithDateOnlyLabelsSeriesInUnsortedOrder_LastDayReturned()
        {
            // Arrange
            var data = new InsightsData<DateOnly, int>();
            var series = data.Add("series 1");
            series.Add(new DateOnly(2023, 1, 1), 10);
            series.Add(new DateOnly(2023, 1, 4), 30);
            series.Add(new DateOnly(2023, 1, 2), 20);

            // Act
            var endDay = data.GetEndDay();

            // Assert
            var expected = new DateOnly(2023, 1, 4);
            Assert.Equal(expected, endDay);
        }
    }
}