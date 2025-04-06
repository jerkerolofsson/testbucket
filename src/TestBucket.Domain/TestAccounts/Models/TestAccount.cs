using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.TestAccounts.Models;
public class TestAccount : Entity
{
    public long Id { get; set; }

    /// <summary>
    /// Name of the account
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Type of account. For example "wifi", "email"
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// The owner of the accounts.
    /// Accounts may be provisioned by servers. 
    /// This property identifies all accounts owned by a system so that accounts can be disabled if 
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
    /// Sub-type for the account. Suitable values depend on the main type.
    /// </summary>
    public string? SubType { get; set; }

    /// <summary>
    /// Variables (user-defined)
    /// </summary>
    public Dictionary<string, string> Variables { get; set; } = [];
}
