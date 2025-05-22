using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Identity.Permissions.Models;

namespace TestBucket.Domain.UnitTests.Identity
{
    /// <summary>
    /// Tests for PermissionClaimSerializer
    /// </summary>
    [EnrichedTest]
    [UnitTest]
    public class PermissionClaimSerializerTests
    {
        /// <summary>
        /// Tests that serializing and deserializing a <see cref="UserPermissions"/> instance containing a single <see cref="EntityPermission"/>
        /// for the specified <paramref name="entity"/> produces the expected string and round-trips the permission level correctly.
        /// Also verifies that no other entity type is granted a permission level other than <see cref="PermissionLevel.None"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PermissionEntityType"/> to test serialization and deserialization for.</param>
        
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
