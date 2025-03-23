using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Testing.Models;

[Table("issues")]
[Index(nameof(Created))]
public class Issue : ProjectEntity
{
    public long Id { get; set; }

    /// <summary>
    /// ID in external system
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// Name of external system (integration)
    /// </summary>
    public string? ExternalSystemName { get; set; }

    /// <summary>
    /// URL to issue
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Description of issue
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Milestone
    /// </summary>
    public string? Milestone { get; set; }
}
