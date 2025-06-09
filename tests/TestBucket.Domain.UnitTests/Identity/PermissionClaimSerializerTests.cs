using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Identity.Permissions.Models;

namespace TestBucket.Domain.UnitTests.Identity
{
    /// <summary>
    /// Tests for PermissionClaimSerializer
    /// </summary>
    [EnrichedTest]
    [UnitTest]
    [Component("Identity")]
    [FunctionalTest]
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
        [InlineData(PermissionEntityType.Issue)]
        [InlineData(PermissionEntityType.Heuristic)]
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

        /// <summary>
        /// Tests that serializing and deserializing a <see cref="UserPermissions"/> instance containing multiple <see cref="EntityPermission"/>
        /// objects for different entities produces the expected string and round-trips all permission levels correctly.
        /// </summary>
        [Fact]
        public void Serialize_MultiplePermissions_RoundTripsAllEntities()
        {
            // Arrange
            var permissions = new UserPermissions([
                new EntityPermission(PermissionEntityType.Tenant, PermissionLevel.Read),
                new EntityPermission(PermissionEntityType.User, PermissionLevel.Write),
                new EntityPermission(PermissionEntityType.Project, PermissionLevel.Approve)
            ]);

            // Act
            var result = PermissionClaimSerializer.Serialize(permissions);
            var deserialized = PermissionClaimSerializer.Deserialize(result);

            // Assert
            Assert.Equal(PermissionLevel.Read, deserialized.Permisssions.First(x => x.EntityType == PermissionEntityType.Tenant).Level);
            Assert.Equal(PermissionLevel.Write, deserialized.Permisssions.First(x => x.EntityType == PermissionEntityType.User).Level);
            Assert.Equal(PermissionLevel.Approve, deserialized.Permisssions.First(x => x.EntityType == PermissionEntityType.Project).Level);
        }

        /// <summary>
        /// Tests that serializing and deserializing an empty <see cref="UserPermissions"/> instance
        /// results in all permissions being set to <see cref="PermissionLevel.None"/>.
        /// </summary>
        [Fact]
        public void Serialize_EmptyPermissions_AllPermissionsHaveNoneLevel()
        {
            // Arrange
            var permissions = new UserPermissions([]);

            // Act
            var result = PermissionClaimSerializer.Serialize(permissions);
            var deserialized = PermissionClaimSerializer.Deserialize(result);

            // Assert
            foreach (var permission in deserialized.Permisssions)
            {
                Assert.Equal(PermissionLevel.None, permission.Level);
            }
        }
    }
}
