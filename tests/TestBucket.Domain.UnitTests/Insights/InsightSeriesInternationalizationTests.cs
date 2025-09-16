using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Insights.Model;

namespace TestBucket.Domain.UnitTests.Insights;

[Component("Insights")]
[Feature("Insights")]
[UnitTest]
[EnrichedTest]
[FunctionalTest]
public class InsightSeriesInternationalizationTests
{
    [Fact]
    public void ConvertToStringLabels_WithDateFormatIso_CorrectlyFormatsLabels()
    {
        // Arrange
        var data = new InsightsData<DateOnly, double>();
        var series = data.Add("series 1");
        series.Add(new DateOnly(2023, 1, 1), 10);
        series.Add(new DateOnly(2023, 2, 1), 20);
        series.Add(new DateOnly(2023, 3, 1), 30);
        // Act
        var stringData = data.ConvertToStringLabels(label => label.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture));
        // Assert
        var labels = stringData.Series[0].Labels.ToList();
        Assert.Equal("2023-01-01", labels[0]);
        Assert.Equal("2023-02-01", labels[1]);
        Assert.Equal("2023-03-01", labels[2]);
    }
}
