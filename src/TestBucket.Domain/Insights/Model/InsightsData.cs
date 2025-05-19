using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Insights.Model;
public class InsightsData<T,U> where T : notnull
{
    private readonly List<InsightsSeries<T,U>> _series = [];
    public string? Name { get; set; }

    public IReadOnlyList<InsightsSeries<T, U>> Series => _series;

    /// <summary>
    /// Adds a series
    /// </summary>
    /// <param name="series"></param>
    public void Add(InsightsSeries<T,U> series)
    {
        _series.Add(series);
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
}
