using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Export.Models;
public abstract class ExportEntity
{
    /// <summary>
    /// Source
    /// </summary>
    public required string Source { get; set; }

    /// <summary>
    /// Entity type
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Entity ID
    /// </summary>
    public required string Id { get; set; }

    public abstract Stream Open();
}
