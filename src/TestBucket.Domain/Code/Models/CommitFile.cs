using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Code.Models;
public class CommitFile : ProjectEntity
{
    /// <summary>
    /// Database ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public required string Path { get; set; }
    public required string Sha { get; set; }
    public int Additions { get; set; }
    public int Deletions { get; set; }
    public int Changes { get; set; }
    public string? Status { get; set; }

    // Navigation

    public long CommitId { get; set; }
    public Commit? Commit { get; set; }
}
