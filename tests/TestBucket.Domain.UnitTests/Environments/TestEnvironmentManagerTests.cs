using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using TestBucket.Domain.Environments;
using TestBucket.Domain.Environments.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;

using Xunit;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TestBucket.Domain.UnitTests.Environments;

/// <summary>
/// Exhaustive tests for <see cref="TestEnvironmentManager"/>
/// </summary>
[EnrichedTest]
[UnitTest]
[Component("Environments")]
public class TestEnvironmentManagerTests
{
    private const string TENANT_ID = "tenant-1";
    private const long PROJECT_ID = 42;

    private (TestEnvironmentManager, FakeTimeProvider) CreateSut()
    {
        var repo = new Fakes.FakeTestEnvironmentRepository();
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 18, 0, 1, 2, TimeSpan.Zero));
        var manager = new TestEnvironmentManager(repo, timeProvider);
        return (manager, timeProvider);
    }

    private ClaimsPrincipal CreateUser(PermissionLevel level, string? name = null, string? tenantId = null) => Impersonation.Impersonate(x =>
    {
        x.TenantId = tenantId ?? TENANT_ID;
        x.UserName = x.Email = name ?? "user@nasa.gov";
        x.Add(PermissionEntityType.Project, level);
    });

    /// <summary>
    /// Verifies that AddTestEnvironmentAsync adds a new environment with correct metadata.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddTestEnvironmentAsync_AddsEnvironmentWithMetadata()
    {
        var (sut, timeProvider) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);
        var env = new TestEnvironment { Name = "QA", TestProjectId = PROJECT_ID, Variables = new() };

        await sut.AddTestEnvironmentAsync(user, env);

        var added = await sut.GetTestEnvironmentByIdAsync(user, env.Id);
        Assert.NotNull(added);
        Assert.Equal(TENANT_ID, added.TenantId);
        Assert.Equal("user@nasa.gov", added.CreatedBy);
        Assert.Equal("user@nasa.gov", added.ModifiedBy);
        Assert.Equal(timeProvider.GetUtcNow(), added.Created);
        Assert.Equal(timeProvider.GetUtcNow(), added.Modified);
    }

    /// <summary>
    /// Verifies that AddTestEnvironmentAsync throws if there is no permission
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task AddTestEnvironmentAsync_ThrowsIfNoPermission()
    {
        var (sut, timeProvider) = CreateSut();
        var user = CreateUser(PermissionLevel.Read);
        var env = new TestEnvironment { Name = "QA", TestProjectId = PROJECT_ID, Variables = new() };

        // Act
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await sut.AddTestEnvironmentAsync(user, env));
    }


    /// <summary>
    /// Verifies that AddTestEnvironmentAsync throws if the user claims principal has no identity
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task AddTestEnvironmentAsync_ThrowsIfNoIdentity()
    {
        var (sut, timeProvider) = CreateSut();
        var user = new ClaimsPrincipal();
        var env = new TestEnvironment { Name = "QA", TestProjectId = PROJECT_ID, Variables = new() };

        // Act
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await sut.AddTestEnvironmentAsync(user, env));
    }

    /// <summary>
    /// Verifies that UpdateTestEnvironmentAsync adds a new environment with correct metadata.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateTestEnvironmentAsync_UpdatesMetadata()
    {
        var (sut, timeProvider) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);
        var env = new TestEnvironment { Name = "QA", TestProjectId = PROJECT_ID, Variables = new() };

        var createdTime = timeProvider.GetUtcNow();
        await sut.AddTestEnvironmentAsync(user, env);
        var toBeUpdated = await sut.GetTestEnvironmentByIdAsync(user, env.Id);

        // Act
        var updateUser = CreateUser(PermissionLevel.ReadWrite, "admin@nasa.gov");
        var updateTime = timeProvider.Advance(TimeSpan.FromHours(1.1));
        await sut.UpdateTestEnvironmentAsync(updateUser, env);

        // Assert
        var updated = await sut.GetTestEnvironmentByIdAsync(user, env.Id);

        Assert.NotNull(updated);
        Assert.Equal(TENANT_ID,updated.TenantId);
        Assert.Equal("user@nasa.gov", updated.CreatedBy);
        Assert.Equal("admin@nasa.gov", updated.ModifiedBy);
        Assert.Equal(createdTime, updated.Created);
        Assert.Equal(updateTime, updated.Modified);
    }

    /// <summary>
    /// Verifies that UpdateTestEnvironmentAsync adds a new environment with correct metadata.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task UpdateTestEnvironmentAsync_ThrowsIfTenantIsDifferent()
    {
        var (sut, timeProvider) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);
        var env = new TestEnvironment { Name = "QA", TestProjectId = PROJECT_ID, Variables = new() };

        var createdTime = timeProvider.GetUtcNow();
        await sut.AddTestEnvironmentAsync(user, env);
        var toBeUpdated = await sut.GetTestEnvironmentByIdAsync(user, env.Id);

        // Act & Assert
        var updateUser = CreateUser(PermissionLevel.ReadWrite, "admin@nasa.gov", tenantId: "TENANT132");
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await sut.UpdateTestEnvironmentAsync(updateUser, env));
    }

    /// <summary>
    /// Verifies that DeleteTestEnvironmentAsync removes the environment.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task DeleteTestEnvironmentAsync_RemovesEnvironment()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.All);
        var env = new TestEnvironment { Name = "QA", TestProjectId = PROJECT_ID, Variables = new() };

        await sut.AddTestEnvironmentAsync(user, env);
        Assert.NotNull(await sut.GetTestEnvironmentByIdAsync(user, env.Id));

        await sut.DeleteTestEnvironmentAsync(user, env.Id);

        var deleted = await sut.GetTestEnvironmentByIdAsync(user, env.Id);
        Assert.Null(deleted);
    }

    /// <summary>
    /// Verifies that DeleteTestEnvironmentAsync throws if user has insufficient permissions.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task DeleteTestEnvironmentAsync_ThrowsIfNoPermission()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.All);
        var env = new TestEnvironment { Name = "QA", TestProjectId = PROJECT_ID, Variables = new() };

        await sut.AddTestEnvironmentAsync(user, env);

        var noDeleteUser = CreateUser(PermissionLevel.Read);  // Note: Write Project permission is needed to delete a test environment
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await sut.DeleteTestEnvironmentAsync(noDeleteUser, env.Id));
    }

    /// <summary>
    /// Verifies that GetTestEnvironmentsAsync returns all environments for the tenant.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task GetTestEnvironmentsAsync_ReturnsAllForTenant()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);

        var env1 = new TestEnvironment { Name = "QA", TestProjectId = PROJECT_ID, Variables = new() };
        var env2 = new TestEnvironment { Name = "Staging", TestProjectId = PROJECT_ID, Variables = new() };

        await sut.AddTestEnvironmentAsync(user, env1);
        await sut.AddTestEnvironmentAsync(user, env2);

        var all = await sut.GetTestEnvironmentsAsync(user);
        Assert.Contains(all, e => e.Name == "QA");
        Assert.Contains(all, e => e.Name == "Staging");
    }

    /// <summary>
    /// Verifies that GetProjectTestEnvironmentsAsync returns only environments for the specified project.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task GetProjectTestEnvironmentsAsync_ReturnsOnlyForProject()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);

        var env1 = new TestEnvironment { Name = "QA", TestProjectId = PROJECT_ID, Variables = new() };
        var env2 = new TestEnvironment { Name = "Other", TestProjectId = 999, Variables = new() };

        await sut.AddTestEnvironmentAsync(user, env1);
        await sut.AddTestEnvironmentAsync(user, env2);

        var projectEnvs = await sut.GetProjectTestEnvironmentsAsync(user, PROJECT_ID);
        Assert.Single(projectEnvs);
        Assert.Equal("QA", projectEnvs[0].Name);
    }

    /// <summary>
    /// Verifies that GetDefaultTestEnvironmentAsync returns the default environment for a project.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task GetDefaultTestEnvironmentAsync_ReturnsDefault()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);

        var env1 = new TestEnvironment { Name = "QA", TestProjectId = PROJECT_ID, Variables = new(), Default = false };
        var env2 = new TestEnvironment { Name = "Staging", TestProjectId = PROJECT_ID, Variables = new(), Default = true };

        await sut.AddTestEnvironmentAsync(user, env1);
        await sut.AddTestEnvironmentAsync(user, env2);

        var def = await sut.GetDefaultTestEnvironmentAsync(user, PROJECT_ID);
        Assert.NotNull(def);
        Assert.Equal("Staging", def.Name);
        Assert.True(def.Default);
    }

    /// <summary>
    /// Verifies that GetTestEnvironmentByIdAsync returns null for non-existent environment.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task GetTestEnvironmentByIdAsync_ReturnsNullIfNotFound()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);

        var result = await sut.GetTestEnvironmentByIdAsync(user, 99999);
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that AddTestEnvironmentAsync throws if environment is null.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddTestEnvironmentAsync_ThrowsIfNull()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await sut.AddTestEnvironmentAsync(user, null!));
    }

    /// <summary>
    /// Verifies that UpdateTestEnvironmentAsync throws if environment is null.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateTestEnvironmentAsync_ThrowsIfNull()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await sut.UpdateTestEnvironmentAsync(user, null!));
    }

    /// <summary>
    /// Verifies that AddTestEnvironmentAsync propagates repository exceptions.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddTestEnvironmentAsync_PropagatesRepositoryException()
    {
        var repo = new Fakes.FakeTestEnvironmentRepository();
        repo.ThrowOnAdd = true; // Fake an exception during add
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        var sut = new TestEnvironmentManager(repo, timeProvider);
        var user = CreateUser(PermissionLevel.ReadWrite);
        var env = new TestEnvironment { Name = "QA", TestProjectId = PROJECT_ID, Variables = new() };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await sut.AddTestEnvironmentAsync(user, env));
    }

}