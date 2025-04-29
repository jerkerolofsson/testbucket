using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Code.Models;

[Index(nameof(Sha))]
[Index(nameof(Reference))]
public class Commit : ProjectEntity
{
    /// <summary>
    /// Database ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Commit message
    /// </summary>
    public string? Message { get; set; }

    public DateTimeOffset? Commited { get; set; }
    public string? CommitedBy { get; set; }

    /// <summary>
    /// Ref
    /// </summary>
    public required string Reference { get; set; }

    /// <summary>
    /// Ref
    /// </summary>
    public required string Sha { get; set; }

    // Navigation

    public long RepositoryId { get; set; }
    public Repository? Repository { get; set; }

    /// <summary>
    /// Impacted features
    /// </summary>
    public virtual List<Feature>? Features { get; set; }

    /// <summary>
    /// Impacted components
    /// </summary>
    public virtual List<Component>? Components { get; set; }

    /// <summary>
    /// Impacted layers
    /// </summary>
    public virtual List<ArchitecturalLayer>? Layers { get; set; }

    /// <summary>
    /// Files
    /// </summary>
    public virtual List<CommitFile>? CommitFiles { get; set; }

}
