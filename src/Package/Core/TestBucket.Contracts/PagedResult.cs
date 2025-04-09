using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts;
public class PagedResult<T>
{
    /// <summary>
    /// Total number of results
    /// </summary>
    public required long TotalCount { get; set; }

    /// <summary>
    /// Paged results
    /// </summary>
    public required T[] Items { get; set; }
}
