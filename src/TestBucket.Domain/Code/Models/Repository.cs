using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Code.Models;

[Index(nameof(Url))]
public class Repository : ProjectEntity
{
    /// <summary>
    /// Database ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Repo URL
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// External ID
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// The time when the repo was indexed the last time
    /// </summary>
    public DateTimeOffset? LastIndexTimestamp { get; set; }

    // Navigation

    /// <summary>
    /// ExternalSystem table
    /// </summary>
    public long ExternalSystemId { get; set; }

    public ExternalSystem? ExternalSystem { get; set; }
}
