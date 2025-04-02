using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Identity.Models;
public class ApplicationUserApiKey : Entity
{
    public long Id { get; set; }

    /// <summary>
    /// User provided name of the key
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// API Key
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Date when the key expires
    /// </summary>
    public required DateTimeOffset Expiry { get; set; }

    // Navigation

    /// <summary>
    /// The user associated with the key
    /// </summary>
    public string? ApplicationUserId { get; set; }

    public ApplicationUser? ApplicationUser { get; set; }
}
