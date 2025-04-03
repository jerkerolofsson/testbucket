using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Identity.Permissions
{
    public class RolePermission : Entity
    {
        public long Id { get; set; }

        /// <summary>
        /// Role
        /// </summary>
        public required string Role { get; set; }

        /// <summary>
        /// The type of entity
        /// </summary>
        public PermissionEntityType Entity { get; set; }

        /// <summary>
        /// Access level
        /// </summary>
        public PermissionLevel Level { get; set; }
    }
}
