using System;
using System.Linq;
using Xunit;
using TestBucket.Domain.Insights.Model;
using TestBucket.Contracts.Insights;

namespace TestBucket.Domain.UnitTests.Insights;

/// <summary>
/// Unit tests for <see cref="InsightsSeries{T, U}"/>.
/// Validates property access, data addition, updating, sorting, and retrieval.
/// </summary>
[Component("Insights")]
[Feature("Insights")]
[UnitTest]
[EnrichedTest]
[FunctionalTest]
public class InsightsSeriesTests
{
    /// <summary>
    /// Verifies that the <see cref="InsightsSeries{T, U}.Name"/> property can be set and retrieved.
    /// </summary>
    [Fact]
    public void Name_Property_Should_Set_And_Get()
    {
        var series = new InsightsSeries<string, int> { Name = "TestSeries" };
        Assert.Equal("TestSeries", series.Name);
    }

    /// <summary>
    /// Verifies <see cref="InsightsSeries{T, U}.HasData"/> is false when no data points exist.
    /// </summary>
    [Fact]
    public void HasData_Should_Be_False_When_Empty()
    {
        var series = new InsightsSeries<string, int> { Name = "EmptySeries" };
        Assert.False(series.HasData);
    }

    /// <summary>
    /// Verifies <see cref="InsightsSeries{T, U}.HasData"/> is true after adding a data point.
    /// </summary>
    [Fact]
    public void HasData_Should_Be_True_When_Data_Added()
    {
        var series = new InsightsSeries<string, int> { Name = "DataSeries" };
        series.Add("A", 1);
        Assert.True(series.HasData);
    }

    /// <summary>
    /// Verifies <see cref="InsightsSeries{T, U}.Add(T, U)"/> adds a data point.
    /// </summary>
    [Fact]
    public void Add_Should_Add_DataPoint()
    {
        var series = new InsightsSeries<string, int> { Name = "AddSeries" };
        series.Add("A", 10);
        Assert.Single(series.Data);
        Assert.Equal(10, series["A"]);
    }

    /// <summary>
    /// Verifies indexer get returns value if label exists.
    /// </summary>
    [Fact]
    public void Indexer_Get_Should_Return_Value_If_Exists()
    {
        var series = new InsightsSeries<string, int> { Name = "IndexerSeries" };
        series.Add("A", 5);
        var value = series["A"];
        Assert.Equal(5, value);
    }

    /// <summary>
    /// Verifies indexer get returns default value if label does not exist.
    /// </summary>
    [Fact]
    public void Indexer_Get_Should_Return_Default_If_Not_Exists()
    {
        var series = new InsightsSeries<string, int> { Name = "IndexerSeries" };
        var value = series["B"];
        Assert.Equal(default(int), value);
    }

    /// <summary>
    /// Verifies indexer set updates an existing value.
    /// </summary>
    [Fact]
    public void Indexer_Set_Should_Update_Existing_Value()
    {
        var series = new InsightsSeries<string, int> { Name = "SetSeries" };
        series.Add("A", 1);
        series["A"] = 2;
        Assert.Equal(2, series["A"]);
    }

    /// <summary>
    /// Verifies indexer set adds a new value if label does not exist.
    /// </summary>
    [Fact]
    public void Indexer_Set_Should_Add_New_Value_If_Not_Exists()
    {
        var series = new InsightsSeries<string, int> { Name = "SetSeries" };
        series["B"] = 3;
        Assert.Equal(3, series["B"]);
    }

    /// <summary>
    /// Verifies <see cref="InsightsSeries{T, U}.TryGetValue(T, out U)"/> returns true and value if label exists.
    /// </summary>
    [Fact]
    public void TryGetValue_Should_Return_True_And_Value_If_Exists()
    {
        var series = new InsightsSeries<string, int> { Name = "TryGetSeries" };
        series.Add("A", 7);
        var result = series.TryGetValue("A", out var value);
        Assert.True(result);
        Assert.Equal(7, value);
    }

    /// <summary>
    /// Verifies <see cref="InsightsSeries{T, U}.TryGetValue(T, out U)"/> returns false if label does not exist.
    /// </summary>
    [Fact]
    public void TryGetValue_Should_Return_False_If_Not_Exists()
    {
        var series = new InsightsSeries<string, int> { Name = "TryGetSeries" };
        var result = series.TryGetValue("B", out var value);
        Assert.False(result);
        Assert.Equal(default(int), value);
    }

    /// <summary>
    /// Verifies <see cref="InsightsSeries{T, U}.Values"/> returns all values.
    /// </summary>
    [Fact]
    public void Values_Should_Return_All_Values()
    {
        var series = new InsightsSeries<string, int> { Name = "ValuesSeries" };
        series.Add("A", 1);
        series.Add("B", 2);
        Assert.Equal(new[] { 1, 2 }, series.Values.ToArray());
    }

    /// <summary>
    /// Verifies <see cref="InsightsSeries{T, U}.Labels"/> returns all labels.
    /// </summary>
    [Fact]
    public void Labels_Should_Return_All_Labels()
    {
        var series = new InsightsSeries<string, int> { Name = "LabelsSeries" };
        series.Add("A", 1);
        series.Add("B", 2);
        Assert.Equal(new[] { "A", "B" }, series.Labels.ToArray());
    }

    /// <summary>
    /// Verifies <see cref="InsightsSeries{T, U}.Data"/> is sorted by label ascending by default.
    /// </summary>
    [Fact]
    public void Data_Should_Be_Sorted_By_Label_Ascending_By_Default()
    {
        var series = new InsightsSeries<string, int> { Name = "SortSeries" };
        series.Add("B", 2);
        series.Add("A", 1);
        var labels = series.Data.Select(x => x.Label).ToArray();
        Assert.Equal(new[] { "A", "B" }, labels);
    }

    /// <summary>
    /// Verifies <see cref="InsightsSeries{T, U}.Data"/> is sorted by label descending.
    /// </summary>
    [Fact]
    public void Data_Should_Be_Sorted_By_Label_Descending()
    {
        var series = new InsightsSeries<string, int> { Name = "SortSeries", SortBy = InsightsSort.LabelDescending };
        series.Add("A", 1);
        series.Add("B", 2);
        var labels = series.Data.Select(x => x.Label).ToArray();
        Assert.Equal(new[] { "B", "A" }, labels);
    }

    /// <summary>
    /// Verifies <see cref="InsightsSeries{T, U}.Data"/> is sorted by value ascending.
    /// </summary>
    [Fact]
    public void Data_Should_Be_Sorted_By_Value_Ascending()
    {
        var series = new InsightsSeries<string, int> { Name = "SortSeries", SortBy = InsightsSort.ValueAscending };
        series.Add("A", 2);
        series.Add("B", 1);
        var values = series.Data.Select(x => x.Value).ToArray();
        Assert.Equal(new[] { 1, 2 }, values);
    }

    /// <summary>
    /// Verifies <see cref="InsightsSeries{T, U}.Data"/> is sorted by value descending.
    /// </summary>
    [Fact]
    public void Data_Should_Be_Sorted_By_Value_Descending()
    {
        var series = new InsightsSeries<string, int> { Name = "SortSeries", SortBy = InsightsSort.ValueDescending };
        series.Add("A", 1);
        series.Add("B", 2);
        var values = series.Data.Select(x => x.Value).ToArray();
        Assert.Equal(new[] { 2, 1 }, values);
    }
}
