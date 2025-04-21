using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Identity.Permissions
{
    /// <summary>
    /// Permissions within a project.
    /// If a permission does not exist, it will be inherited from the role
    /// </summary>
    public class ProjectUserPermission : ProjectEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// The type of entity
        /// </summary>
        public PermissionEntityType Entity { get; set; }

        /// <summary>
        /// Access level
        /// </summary>
        public PermissionLevel Level { get; set; }

        // Navigation
        public required string ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}