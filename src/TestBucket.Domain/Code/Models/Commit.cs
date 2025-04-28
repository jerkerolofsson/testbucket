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
    /// Ref
    /// </summary>
    public required string Reference { get; set; }

    /// <summary>
    /// Ref
    /// </summary>
    public required string Sha { get; set; }

    // Navigation


    public long RepositoryId { get; set; }
    public required Repository Repository { get; set; }

    public virtual List<Feature>? Features { get; set; }
    public virtual List<Component>? Components { get; set; }
    public virtual List<ArchitecturalLayer>? Layers { get; set; }

    public virtual List<CommitFile>? CommitFiles { get; set; }

}
