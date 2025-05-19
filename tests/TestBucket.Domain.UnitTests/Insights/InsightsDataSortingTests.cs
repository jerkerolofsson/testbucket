using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Insights.Extensions;

namespace TestBucket.Domain.UnitTests.Insights
{
    [Component("Insights")]
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class InsightsDataSortingTests
    {
        [Fact]
        [TestDescription("""
            Verifies that the labels are returned in the correct order when sorting by label ascending
            """)]
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

        [Fact]
        [TestDescription("""
            Verifies that the labels are returned in the correct order when sorting by label descending
            """)]
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

        [Fact]
        [TestDescription("""
            Verifies that the labels are returned in the correct order when sorting by value descending
            """)]
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

        [Fact]
        [TestDescription("""
            Verifies that the labels are returned in the correct order when sorting by value ascending
            """)]
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
