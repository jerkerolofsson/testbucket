using TestBucket.Domain.Insights.Model;

namespace TestBucket.Domain.Insights.Extensions;
public static class InsightsDataExtensions
{
    /// <summary>
    /// Adds missing string labels
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <param name="data"></param>
    /// <param name="labels"></param>
    public static void AddMissingLabels<U>(this InsightsData<string, U> data, IEnumerable<string?> labels)
    {
        foreach (var label in labels)
        {
            if (label is not null)
            {
                foreach (var series in data.Series)
                {
                    if (!series.TryGetValue(label, out _))
                    {
                        series.Add(label, default!);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Returns the first day in any data series (inclusive)
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static DateOnly? GetStartDay<U>(this InsightsData<DateOnly, U> data)
    {
        if (data.Series.Count == 0)
        {
            return null;
        }
        var start = DateOnly.MaxValue;
        foreach (var series in data.Series)
        {
            var min = series.Labels.Min();
            if (min < start)
            {
                start = min;
            }
        }
        return start;
    }

    /// <summary>
    /// Returns the last day in any data series (inclusive)
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static DateOnly? GetEndDay<U>(this InsightsData<DateOnly, U> data)
    {
        if (data.Series.Count == 0)
        {
            return null;
        }
        var end = DateOnly.MinValue;
        foreach (var series in data.Series)
        {
            var max = series.Labels.Max();
            if (max > end)
            {
                end = max;
            }
        }
        return end;
    }

    public static void AddMissingDays<U>(this InsightsData<DateOnly, U> data)
    {
        var start = data.GetStartDay();
        var end = data.GetEndDay();
        if(start is null || end is null)
        {
            return;
        }

        var date = start.Value;
        while(date != end)
        {
            date = date.AddDays(1);

            foreach(var series in data.Series)
            {
                if (!series.Labels.Contains(date))
                {
                    series.Add(date, default!);
                }
            }
        }
    }
}
