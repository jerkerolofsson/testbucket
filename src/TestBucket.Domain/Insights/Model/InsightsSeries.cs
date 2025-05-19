using System;
using System.Diagnostics.CodeAnalysis;

namespace TestBucket.Domain.Insights.Model;
public class InsightsSeries<T, U> where T : notnull
{
    private readonly List<InsightsDataPoint<T, U>> _data = [];

    public required string Name { get; set; }

    public InsightsSort SortBy { get; set; } = InsightsSort.LabelAscending;

    public IEnumerable<InsightsDataPoint<T, U>> Data
    {
        get
        {
            if (SortBy == InsightsSort.LabelAscending)
            {
                foreach (var point in _data.OrderBy(x => x.Label))
                {
                    yield return point;
                }
            }
            else if (SortBy == InsightsSort.LabelDescending)
            {
                foreach (var point in _data.OrderByDescending(x => x.Label))
                {
                    yield return point;
                }
            }
            else if (SortBy == InsightsSort.ValueAscending)
            {
                foreach (var point in _data.OrderBy(x => x.Value))
                {
                    yield return point;
                }
            }
            else if (SortBy == InsightsSort.ValueDescending)
            {
                foreach (var point in _data.OrderByDescending(x => x.Value))
                {
                    yield return point;
                }
            }
            else
            {
                foreach (var point in _data)
                {
                    yield return point;
                }
            }
        }
    }

    public void Add(T label, U value)
    {
        var point = new InsightsDataPoint<T, U>(label, value);
        _data.Add(point);
    }

    public bool TryGetValue(T label, [NotNullWhen(true)] out U? value)
    {
        value = default;

        var point = _data.FirstOrDefault(x => x.Label.Equals(label));

        if(point is not null)
        {
            value = point.Value!;
            return true;
        }
        return false;
    }
}
