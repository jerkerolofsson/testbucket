using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Quartz;

using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Jobs;
using TestBucket.Domain.Shared;

namespace TestBucket.Domain.UnitTests.Jobs;

/// <summary>
/// Unit tests for the <see cref="JobDataMapExtensions"/> class, verifying correct behavior
/// when adding user information and permissions to a <see cref="JobDataMap"/>.
/// </summary>
[EnrichedTest]
[UnitTest]
[Component("Jobs")]
[FunctionalTest]
public class JobDataMapExtensionsTests
{
    /// <summary>
    /// Verifies that the tenant ID is added correctly to the <see cref="JobDataMap"/> when a user is added.
    /// </summary>
    [Fact]
    public void AddUser_TenantIdAddedCorrectly()
    {
        // Arrange
        var data = new JobDataMap();
        var user = Impersonation.Impersonate("tenant-1");

        // Act
        data.AddUser(user);

        // Assert
        Assert.True(data.Contains(UserJob.KEY_TENANT_ID));
        Assert.Equal("tenant-1", data[UserJob.KEY_TENANT_ID]);
    }

    /// <summary>
    /// Verifies that an <see cref="UnauthorizedAccessException"/> is thrown if the tenant is missing when adding a user.
    /// </summary>
    [Fact]
    public void AddUser_ThrowsIfTenantIsMissing()
    {
        // Arrange
        var data = new JobDataMap();
        var user = Impersonation.Impersonate(builder =>
        {
            builder.UserName = "user1";
            builder.Email = "user@user.com";
        });

        // Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() => data.AddUser(user));
    }

    /// <summary>
    /// Verifies that the user name is added correctly to the <see cref="JobDataMap"/> when a user is added.
    /// </summary>
    [Fact]
    public void AddUser_UserNameAddedCorrectly()
    {
        // Arrange
        var data = new JobDataMap();
        var user = Impersonation.Impersonate(builder =>
        {
            builder.TenantId = "tenant-1";
            builder.UserName = "user1";
            builder.Email = "user@user.com";
        });

        // Act
        data.AddUser(user);

        // Assert
        Assert.True(data.Contains(UserJob.KEY_USERNAME));
        Assert.Equal("user1", data[UserJob.KEY_USERNAME]);
    }

    /// <summary>
    /// Verifies that all relevant user properties are added to the <see cref="JobDataMap"/>.
    /// </summary>
    [Fact]
    public void AddUser_PropertiesAddedToJobDataMap()
    {
        // Arrange
        var data = new JobDataMap();
        var user = Impersonation.Impersonate("tenant-1");

        // Act
        data.AddUser(user);

        // Assert
        Assert.True(data.Contains(UserJob.KEY_USERNAME));
        Assert.True(data.Contains(UserJob.KEY_TENANT_ID));
        Assert.True(data.Contains(UserJob.KEY_PERMSISSIONS));
    }

    /// <summary>
    /// Verifies that the tenant ID is correct when a user with a tenant is added.
    /// </summary>
    [Fact]
    public void AddUser_WithTenant_TenantIdCorrect()
    {
        // Arrange
        var data = new JobDataMap();
        var user = Impersonation.Impersonate("tenant-1");

        // Act
        data.AddUser(user);

        // Assert
        Assert.True(data.Contains(UserJob.KEY_PERMSISSIONS));
        var permissionsSerialized = data.GetString(UserJob.KEY_PERMSISSIONS);
        var principal = UserJob.CreateClaimsPrincipal(data, "tenant-1", "user1");
        Assert.NotNull(principal);
        principal.ThrowIfEntityTenantIsDifferent("tenant-1");
    }

    /// <summary>
    /// Verifies that the correct write permission is added for each <see cref="PermissionEntityType"/>.
    /// </summary>
    /// <param name="entityType">The entity type to test permission for.</param>
    [Theory]
    [InlineData(PermissionEntityType.Team)]
    [InlineData(PermissionEntityType.Project)]
    [InlineData(PermissionEntityType.Issue)]
    [InlineData(PermissionEntityType.TestCase)]
    [InlineData(PermissionEntityType.TestAccount)]
    [InlineData(PermissionEntityType.Architecture)]
    [InlineData(PermissionEntityType.Dashboard)]
    [InlineData(PermissionEntityType.Runner)]
    [InlineData(PermissionEntityType.Requirement)]
    [InlineData(PermissionEntityType.RequirementSpecification)]
    [InlineData(PermissionEntityType.Heuristic)]
    [InlineData(PermissionEntityType.McpServer)]
    [InlineData(PermissionEntityType.Tenant)]
    [InlineData(PermissionEntityType.TestResource)]
    [InlineData(PermissionEntityType.TestRun)]
    [InlineData(PermissionEntityType.TestSuite)]
    [InlineData(PermissionEntityType.TestCaseRun)]
    [InlineData(PermissionEntityType.User)]
    public void AddUser_WithWritePermission_CorrectPermissionAdded(PermissionEntityType entityType)
    {
        // Arrange
        var data = new JobDataMap();
        var user = Impersonation.Impersonate(builder =>
        {
            builder.UserName = "user1";
            builder.TenantId = "tenant-1";
            builder.Email = "user@user.com";
            builder.Add(entityType, PermissionLevel.Write);
        });

        // Act
        data.AddUser(user);

        // Assert
        Assert.True(data.Contains(UserJob.KEY_PERMSISSIONS));
        var permissionsSerialized = data.GetString(UserJob.KEY_PERMSISSIONS);
        var principal = UserJob.CreateClaimsPrincipal(data, "tenant-1", "user1");
        Assert.NotNull(principal);

        Assert.True(principal.HasPermission(entityType, PermissionLevel.Write));
        Assert.False(principal.HasPermission(entityType, PermissionLevel.Delete));
    }

    /// <summary>
    /// Verifies that the correct read permission is added for each <see cref="PermissionEntityType"/>.
    /// </summary>
    /// <param name="entityType">The entity type to test permission for.</param>
    [Theory]
    [InlineData(PermissionEntityType.Team)]
    [InlineData(PermissionEntityType.Project)]
    [InlineData(PermissionEntityType.Issue)]
    [InlineData(PermissionEntityType.TestCase)]
    [InlineData(PermissionEntityType.TestAccount)]
    [InlineData(PermissionEntityType.Architecture)]
    [InlineData(PermissionEntityType.Dashboard)]
    [InlineData(PermissionEntityType.Runner)]
    [InlineData(PermissionEntityType.Requirement)]
    [InlineData(PermissionEntityType.RequirementSpecification)]
    [InlineData(PermissionEntityType.Heuristic)]
    [InlineData(PermissionEntityType.McpServer)]
    [InlineData(PermissionEntityType.Tenant)]
    [InlineData(PermissionEntityType.TestResource)]
    [InlineData(PermissionEntityType.TestRun)]
    [InlineData(PermissionEntityType.TestSuite)]
    [InlineData(PermissionEntityType.TestCaseRun)]
    [InlineData(PermissionEntityType.User)]
    public void AddUser_WithReadPermission_CorrectPermissionAdded(PermissionEntityType entityType)
    {
        // Arrange
        var data = new JobDataMap();
        var user = Impersonation.Impersonate(builder =>
        {
            builder.UserName = "user1";
            builder.TenantId = "tenant-1";
            builder.Email = "user@user.com";
            builder.Add(entityType, PermissionLevel.Read);
        });

        // Act
        data.AddUser(user);

        // Assert
        Assert.True(data.Contains(UserJob.KEY_PERMSISSIONS));
        var permissionsSerialized = data.GetString(UserJob.KEY_PERMSISSIONS);
        var principal = UserJob.CreateClaimsPrincipal(data, "tenant-1", "user1");
        Assert.NotNull(principal);

        Assert.True(principal.HasPermission(entityType, PermissionLevel.Read));
        Assert.False(principal.HasPermission(entityType, PermissionLevel.Write));
    }
}