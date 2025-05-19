using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Insights.Extensions;

namespace TestBucket.Domain.UnitTests.Insights
{
    [Component("Insights")]
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class InsightsDataTests
    {
        [Fact]
        [TestDescription("""
            Verifies that missing labels can be added to insights data series.
            """)]
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

        [Fact]
        [TestDescription("""
            Verifies that missing dates are added to insights data series.
            For example if the labels are: "2023-01-01", "2023-01-02", and "2023-01-04" then 2023-01-03 should be added.
            """)]
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

        [Fact]
        [TestDescription("""
            Verifies that the first day is returned from a sorted list
            For example if the labels are: "2023-01-01", "2023-01-02", and "2023-01-04" then 2023-01-03 should be added.
            """)]
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

        [Fact]
        [TestDescription("""
            Verifies that the last day is returned from a sorted list
            For example if the labels are: "2023-01-01", "2023-01-02", and "2023-01-04" then 2023-01-03 should be added.
            """)]
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

        [Fact]
        [TestDescription("""
            Verifies that the last day is returned from a sorted list
            For example if the labels are: "2023-01-01", "2023-01-02", and "2023-01-04" then 2023-01-03 should be added.
            """)]
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
