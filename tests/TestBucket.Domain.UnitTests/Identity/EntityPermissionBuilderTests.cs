using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Identity.Permissions;

namespace TestBucket.Domain.UnitTests.Identity
{
    [UnitTest]
    [EnrichedTest]
    public class EntityPermissionBuilderTests
    {
        [Fact]
        [TestDescription("Verifies that when merging permissions from two roles, the result contains permission levels from both")]
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

        [Fact]
        [TestDescription("Verifies that when merging a permission from a role it is set in the result")]
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
