using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Identity.Permissions.Models
{
    public record class EntityPermission(PermissionEntityType EntityType, PermissionLevel Level);
}
