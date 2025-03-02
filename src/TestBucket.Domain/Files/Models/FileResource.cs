using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Files.Models;
public class FileResource
{
    /// <summary>
    /// DB ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Original file name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Tenant ID
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// Content Type
    /// </summary>
    public required string ContentType { get; set; }

    /// <summary>
    /// Bytes
    /// </summary>
    public required byte[] Data { get; set; }
}
