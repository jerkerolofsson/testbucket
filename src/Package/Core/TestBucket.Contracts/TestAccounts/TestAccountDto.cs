using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.TestAccounts
{
    public class TestAccountDto
    {
        /// <summary>
        /// Resource name
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Variables
        /// </summary>
        public Dictionary<string, string> Variables { get; set; } = [];

        /// <summary>
        /// Type of account. For example "wifi", "email"
        /// </summary>
        public required string Type { get; set; }

        /// <summary>
        /// The owner of the accounts.
        /// Accounts may be provisioned by servers. 
        /// This property identifies all accounts owned by a system so that accounts can be disabled if 
        /// </summary>
        public string? Owner { get; set; }

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
        /// Tenant identifier
        /// </summary>
        public string? Tenant { get; set; }

        /// <summary>
        /// Timestamp when the test case was created
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// Modified by user name
        /// </summary>
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Timestamp when the test case was Modified
        /// </summary>
        public DateTimeOffset Modified { get; set; }

        /// <summary>
        /// Created by user name
        /// </summary>
        public string? CreatedBy { get; set; }
    }
}
