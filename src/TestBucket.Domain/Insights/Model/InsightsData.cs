using System.Diagnostics.CodeAnalysis;

namespace TestBucket.Domain.Insights.Model;
public class InsightsData<T,U> where T : notnull
{
    private readonly List<InsightsSeries<T,U>> _series = [];
    public string? Name { get; set; }

    /// <summary>
    /// Returns the data series containing labels and points
    /// </summary>
    public IReadOnlyList<InsightsSeries<T, U>> Series => _series;

    /// <summary>
    /// Adds a series
    /// </summary>
    /// <param name="series"></param>
    public void Add(InsightsSeries<T, U> series)
    {
        _series.Add(series);
    }

    /// <summary>
    /// Adds a new series
    /// </summary>
    /// <param name="series"></param>
    public InsightsSeries<T, U> Add(string name)
    {
        var series = new InsightsSeries<T, U>() { Name = name };
        Add(series);
        return series;
    }

    public bool TryGetValue(T key,  [NotNullWhen(true)] out U? value)
    {
        value = default;

        if(_series.Count > 0)
        {
            foreach(var series in _series)
            {
                if(series.TryGetValue(key, out value))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public InsightsData<string, double> ConvertToStringLabels()
    {
        return ConvertToStringLabels((label) => label.ToString() ?? "(null)");
    }

    /// <summary>
    /// Converts the labels to strings
    /// </summary>
    /// <returns></returns>
    internal InsightsData<string, double> ConvertToStringLabels(Func<T, string> convert)
    {
        var copy = new InsightsData<string, double>();
        copy.Name = this.Name;

        foreach(var series in _series)
        {
            var newSeries = new InsightsSeries<string, double>()
            {
                Name = series.Name,
                SortBy = series.SortBy
            };
            foreach (var point in series.Data)
            {
                newSeries.Add(convert(point.Label), point.ToDouble());
            }
            copy.Add(newSeries);
        }

        return copy;
    }
}
