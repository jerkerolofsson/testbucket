using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Identity.Permissions.Models;

namespace TestBucket.Domain.Identity.Permissions
{
    /// <summary>
    /// Helper that will merge permission levels from multiple roles
    /// </summary>
    internal class EntityPermissionBuilder
    {
        private readonly UserPermissions _userPermissions = new();

        public UserPermissions Build() => _userPermissions;

        public void Add(PermissionEntityType entityType, PermissionLevel level)
        {
            var permission = _userPermissions.Permisssions.Where(x => x.EntityType == entityType).FirstOrDefault();
            if (permission is null)
            {
                permission = new EntityPermission(entityType, PermissionLevel.None);
            }
            else
            {
                _userPermissions.Permisssions.Remove(permission);
            }
            _userPermissions.Permisssions.Add(new EntityPermission(entityType, permission.Level | level));
        }
        public void Add(RolePermission rolePermission)
        {
            var permission = _userPermissions.Permisssions.Where(x => x.EntityType == rolePermission.Entity).FirstOrDefault();
            if(permission is null)
            {
                permission = new EntityPermission(rolePermission.Entity, PermissionLevel.None);
            }
            else
            {
                _userPermissions.Permisssions.Remove(permission);
            }
            _userPermissions.Permisssions.Add(new EntityPermission(rolePermission.Entity, permission.Level | rolePermission.Level));
        }
    }
}
