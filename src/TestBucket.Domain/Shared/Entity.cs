using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Shared
{
    public class Entity
    {
        /// <summary>
        /// Tenant identifier
        /// </summary>
        public string? TenantId { get; set; }

        /// <summary>
        /// Timestamp when the entity was created
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// Modified by user name
        /// </summary>
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Timestamp when the entity was Modified
        /// </summary>
        public DateTimeOffset Modified { get; set; }

        /// <summary>
        /// Created by user name
        /// </summary>
        public string? CreatedBy { get; set; }

        // Navigation
        public Tenant? Tenant { get; set; }
    }
}
