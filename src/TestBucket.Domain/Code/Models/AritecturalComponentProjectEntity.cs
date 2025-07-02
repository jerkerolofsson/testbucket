using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Code.Models;
public class AritecturalComponentProjectEntity : ProjectEntity
{
    /// <summary>
    /// Database ID
    /// </summary>
    public long Id { get; set; }


    /// <summary>
    /// Name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Glob patterns for file paths
    /// </summary>
    [Column(TypeName = "jsonb")]
    public required List<string> GlobPatterns { get; set; }

    /// <summary>
    /// Display options for visualization only
    /// </summary>
    [Column(TypeName = "jsonb")]
    public DisplayOptions? Display { get; set; }

    /// <summary>
    /// Responsible for development
    /// </summary>
    public string? DevResponsible { get; set; }

    /// <summary>
    /// Dev lead
    /// </summary>
    public string? DevLead { get; set; }

    /// <summary>
    /// Test lead
    /// </summary>
    public string? TestLead { get; set; }

    /// <summary>
    /// Text embedding for semantic search and classification.
    /// Consists of the Name and descriptions
    /// </summary>
    [Column(TypeName = "vector(384)")]
    public Pgvector.Vector? Embedding { get; set; }

}
