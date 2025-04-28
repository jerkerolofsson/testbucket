using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Architecture.Models;
public class Component : ProjectEntity
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
    /// Glob patterns for file paths
    /// </summary>
    [Column(TypeName = "jsonb")]
    public required List<string> GlobPatterns { get; set; }


    /// <summary>
    /// User
    /// </summary>
    public string? DevLead { get; set; }

    /// <summary>
    /// User
    /// </summary>
    public string? TestLead { get; set; }

}
