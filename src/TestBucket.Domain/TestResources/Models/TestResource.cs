using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.TestResources.Models;
public class TestResource : Entity
{
    public long Id { get; set; }

    /// <summary>
    /// Name of the resource
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Serial number / Device ID / External ID
    /// </summary>
    public required string ResourceId { get; set; }

    /// <summary>
    /// The owner of the resource.
    /// Resources may be provisioned by resource servers, physically connected to a device or similar. 
    /// This property identifies all resources owned by a system.
    /// </summary>
    public required string Owner { get; set; }

    /// <summary>
    /// Flag that indicates that the account is enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Flag that indicates that the account is locked
    /// </summary>
    public bool Locked { get; set; }

    /// <summary>
    /// Owner of the lock
    /// </summary>
    public string? LockOwner { get; set; }

    /// <summary>
    /// Timestamp when the lock expires
    /// </summary>
    public DateTimeOffset? LockExpires { get; set; }

    /// <summary>
    /// If this resource is part of a larger system, this refers to the parent
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// Types of resource (for example "phone"). A resource should have atleast one, but could have multiple types defined
    /// </summary>
    public required string[] Types { get; set; }

    /// <summary>
    /// Manufacturer/maker of the resource
    /// </summary>
    public string? Manufacturer { get; set; }

    /// <summary>
    /// Model of the resource
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Icon (data uri)
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Variables
    /// </summary>
    [Column(TypeName = "jsonb")] 
    public Dictionary<string, string> Variables { get; set; } = [];
}
