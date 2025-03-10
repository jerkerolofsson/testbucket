using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts;

/// <summary>
/// Base query
/// </summary>
public class SearchQuery
{
    /// <summary>
    /// Filter by team
    /// </summary>
    public long? TeamId { get; set; }

    /// <summary>
    /// Filter by project
    /// </summary>
    public long? ProjectId { get; set; }

    /// <summary>
    /// Free text search
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Offset for paged results (default 0)
    /// </summary>
    public int Offset { get; set; } = 0;

    /// <summary>
    /// Number of results to return (default 20)
    /// </summary>
    public int Count { get; set; } = 20;

    /// <summary>
    /// Inclusive
    /// </summary>
    public DateTimeOffset? CreatedFrom { get; set; }

    /// <summary>
    /// Inclusive
    /// </summary>
    public DateTimeOffset? CreatedUntil { get; set; }
}
