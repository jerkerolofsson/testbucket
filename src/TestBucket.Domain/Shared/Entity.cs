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
        
        // Navigation
        public Tenant? Tenant { get; set; }
    }
}
