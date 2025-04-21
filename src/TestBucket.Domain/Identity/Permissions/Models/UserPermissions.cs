using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Identity.Permissions.Models
{
    /// <summary>
    /// This is an in memory representation of permissions after deserialized from a claim
    /// </summary>
    internal class UserPermissions
    {
        private readonly List<EntityPermission> _permissions = [];

        internal List<EntityPermission> Permisssions => _permissions;

        public UserPermissions()
        {

        }
        public UserPermissions(UserPermissions userPermissions)
        {
            _permissions.AddRange(userPermissions.Permisssions);
        }
        public UserPermissions(IEnumerable<EntityPermission> permissions)
        {
            _permissions.AddRange(permissions);
        }
    }
}
