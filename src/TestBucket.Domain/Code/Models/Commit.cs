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

    /// <summary>
    /// Time of commit
    /// </summary>
    public DateTimeOffset? Commited { get; set; }

    /// <summary>
    /// Who authored the commit (email)
    /// </summary>
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
    /// Impacted system names
    /// </summary>
    public List<string>? SystemNames { get; set; }

    /// <summary>
    /// Impacted features
    /// </summary>
    public List<string>? FeatureNames { get; set; }

    /// <summary>
    /// Impacted components
    /// </summary>
    public List<string>? ComponentNames { get; set; }

    /// <summary>
    /// Impacted layers
    /// </summary>
    public List<string>? LayerNames { get; set; }

    /// <summary>
    /// Files
    /// </summary>
    public virtual List<CommitFile>? CommitFiles { get; set; }

    public override int GetHashCode() => Sha.GetHashCode();

    public override bool Equals(object? obj)
    {
        if(obj is Commit other)
        {
            return Sha.Equals(other.Sha);
        }
        return false;
    }
}
