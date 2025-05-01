using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Identity.Permissions.Models;

namespace TestBucket.Domain.Identity.Permissions
{
    /// <summary>
    /// Serializes permissions as a claim
    /// 
    /// Serialized permissions are a flags, with the PermissionLevel as 2-hex digits
    /// </summary>
    public class PermissionClaimSerializer
    {
        /// <summary>
        /// Note: Only add to the end. The order matters for serialization
        /// </summary>
        private static readonly List<PermissionEntityType> _types = [
            PermissionEntityType.Tenant,
            PermissionEntityType.User,
            PermissionEntityType.Project,

            PermissionEntityType.RequirementSpecification,
            PermissionEntityType.Requirement,

            PermissionEntityType.TestSuite,
            PermissionEntityType.TestCase,

            PermissionEntityType.TestRun,
            PermissionEntityType.TestCaseRun,

            PermissionEntityType.TestAccount,
            PermissionEntityType.TestResource,
            PermissionEntityType.Runner,

            PermissionEntityType.Team,

            PermissionEntityType.Architecture,
            ];

        internal static UserPermissions Deserialize(string hex)
        {
            var userPermissions = new UserPermissions();

            var bytes = Convert.FromHexString(hex);
            int index = 0;
            foreach (var entityType in _types)
            {
                if (bytes.Length > index)
                {
                    var level = (PermissionLevel)bytes[index];
                    userPermissions.Permisssions.Add(new EntityPermission(entityType, level));
                }
                else
                {
                    userPermissions.Permisssions.Add(new EntityPermission(entityType, PermissionLevel.None));
                }
                index++;
            }
            return userPermissions;
        }

        internal static string Serialize(UserPermissions userPermsission)
        {
            var sb = new StringBuilder();

            foreach(var entityType in _types)
            {
                var permission = userPermsission.Permisssions.Where(x => x.EntityType == entityType).FirstOrDefault();
                var level = (int)(permission?.Level ?? PermissionLevel.None);

                // Convert to hex
                if(level > 255)
                {
                    throw new InvalidDataException("Permission level cannot be > 255");
                }
                string value = level.ToString("x2");
                sb.Append(value);
            }

            return sb.ToString();
        }



    }
}
