using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Identity.Permissions.Models;

namespace TestBucket.Domain.UnitTests.Identity
{
    [EnrichedTest]
    [UnitTest]
    public class PermissionClaimSerializerTests
    {
        [InlineData(PermissionEntityType.Tenant)]
        [InlineData(PermissionEntityType.User)]
        [InlineData(PermissionEntityType.Project)]
        [InlineData(PermissionEntityType.RequirementSpecification)]
        [InlineData(PermissionEntityType.Requirement)]
        [InlineData(PermissionEntityType.TestSuite)]
        [InlineData(PermissionEntityType.TestCase)]
        [InlineData(PermissionEntityType.TestRun)]
        [InlineData(PermissionEntityType.TestCaseRun)]
        [InlineData(PermissionEntityType.TestResource)]
        [InlineData(PermissionEntityType.TestAccount)]
        [InlineData(PermissionEntityType.Runner)]
        [Theory]
        public void Serialize_ValidClaim_ReturnsExpectedString(PermissionEntityType entity)
        {
            PermissionLevel[] levels = [PermissionLevel.Read, PermissionLevel.Write, PermissionLevel.ReadWrite, PermissionLevel.None, PermissionLevel.All, PermissionLevel.Approve, PermissionLevel.ReadWriteApprove];
            foreach (var level in levels)
            {
                // Arrange
                var permissions = new UserPermissions([new EntityPermission(entity, level)]);

                // Act
                var result = PermissionClaimSerializer.Serialize(permissions);
                var deserialized = PermissionClaimSerializer.Deserialize(result);

                // Assert
                var permission = deserialized.Permisssions.Where(x => x.EntityType == entity).FirstOrDefault();
                Assert.NotNull(permission);
                Assert.Equal(level, permission.Level);

                // Make sure it didn't "grant" any other permission
                foreach (var a in deserialized.Permisssions.Where(x => x.EntityType != entity))
                {
                    Assert.Equal(PermissionLevel.None, a.Level);
                }
            }
        }
    }
}
