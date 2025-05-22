using TestBucket.Domain.Identity.Permissions;

namespace TestBucket.Domain.UnitTests.Identity
{
    /// <summary>
    /// Contains unit tests for the <see cref="EntityPermissionBuilder"/> class, verifying correct merging and assignment of permissions.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    public class EntityPermissionBuilderTests
    {
        /// <summary>
        /// Verifies that when merging permissions from two roles for the same entity, 
        /// the resulting permissions contain permission levels from both roles.
        /// </summary>
        [Fact]
        public void BuildUserPermission_WithSameEntity_ResultMergedCorrectly()
        {
            // Arrange
            var builder = new EntityPermissionBuilder();

            // Act
            builder.Add(new RolePermission() { Role = "1", Level = PermissionLevel.Read, Entity = PermissionEntityType.TestCase });
            builder.Add(new RolePermission() { Role = "2", Level = PermissionLevel.Write, Entity = PermissionEntityType.TestCase });

            // Build
            var result = builder.Build();
            var permission = result.Permisssions.Where(x => x.EntityType == PermissionEntityType.TestCase).FirstOrDefault();
            Assert.NotNull(permission);
            Assert.Equal(PermissionLevel.Read, (permission.Level & PermissionLevel.Read));
            Assert.Equal(PermissionLevel.Write, (permission.Level & PermissionLevel.Write));
        }

        /// <summary>
        /// Verifies that when merging a permission from a role for a new entity, 
        /// the permission is correctly set in the result.
        /// </summary>
        [Fact]
        public void BuildUserPermission_WithNewEntity_ResultMergedCorrectly()
        {
            // Arrange
            var builder = new EntityPermissionBuilder();

            // Act
            builder.Add(new RolePermission() { Role = "", Level = PermissionLevel.Delete, Entity = PermissionEntityType.TestCase });

            // Build
            var result = builder.Build();
            var permission = result.Permisssions.Where(x => x.EntityType == PermissionEntityType.TestCase).FirstOrDefault();
            Assert.NotNull(permission);
            Assert.Equal(PermissionLevel.Delete, permission.Level);
        }
    }
}