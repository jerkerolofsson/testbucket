using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Insights.Model;
public class InsightsTable
{
    private readonly List<string> _headers = [];
    private readonly List<List<string>> _rows = [];

    /// <summary>
    /// Header labels
    /// </summary>
    public IReadOnlyList<string> Headers => _headers;

    /// <summary>
    /// Rows of data
    /// </summary>
    public IEnumerable<IReadOnlyList<string>> Rows
    {
        get
        {
            foreach (var row in _rows)
            {
                yield return row;
            }
        }
    }

    internal void AddHeader(string text)
    {
        _headers.Add(text);
    }

    internal List<string> AddRow(int capacity)
    {
        List<string> row = new List<string>(capacity);
        _rows.Add(row);
        return row;
    }
}
