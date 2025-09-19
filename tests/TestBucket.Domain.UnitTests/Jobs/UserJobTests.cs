using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Quartz;

using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Identity.Permissions.Models;
using TestBucket.Domain.Jobs;
using TestBucket.Domain.Shared;

namespace TestBucket.Domain.UnitTests.Jobs;

/// <summary>
/// Tests for UserJob base class
/// </summary>
[EnrichedTest]
[UnitTest]
[Component("Jobs")]
[FunctionalTest]
public class UserJobTests
{
    /// <summary>
    /// Verifies that tenant ID is set
    /// </summary>
    [Fact]
    public void GetUser_TenantSet()
    {
        // Arrange
        var data = new JobDataMap();
        var user = Impersonation.ImpersonateUser("tenant-1", "user-1", new UserPermissions());

        // Act
        data.AddUser(user);

        // Assert
        var principal =  UserJob.GetUser(data);
        var tenantId = principal.GetTenantIdOrThrow();
        Assert.Equal("tenant-1", tenantId);
    }

    /// <summary>
    /// Verifies that permissions are set
    /// </summary>
    [Fact]
    public void GetUser_PermissionsAreSet()
    {
        // Arrange
        var data = new JobDataMap();
        var permissions = new UserPermissions();
        permissions.Permisssions.Add(new EntityPermission(PermissionEntityType.Team, PermissionLevel.Read));
        var user = Impersonation.ImpersonateUser("tenant-1", "user-1", permissions);

        // Act
        data.AddUser(user);

        // Assert
        var principal = UserJob.GetUser(data);
        var tenantId = principal.GetTenantIdOrThrow();
        Assert.Equal("tenant-1", tenantId);

        principal.ThrowIfNoPermission(PermissionEntityType.Team, PermissionLevel.Read);
        var hasWrite = principal.HasPermission(PermissionEntityType.Team, PermissionLevel.Write);
        Assert.False(hasWrite);
    }


    /// <summary>
    /// Verifies that an exception is thrown if the permissions key is not set
    /// </summary>
    [Fact]
    public void CreateClaimsPrincipal_ThrowsIfPermissionsKeyIsMissing()
    {
        // Arrange
        var data = new JobDataMap();

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => UserJob.CreateClaimsPrincipal(data, "t1", "u1"));
    }
}
