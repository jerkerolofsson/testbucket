using System.Security.Claims;

using Mediator;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

using TestBucket.Contracts.Code.Models;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Projects.Models;

namespace TestBucket.Domain.UnitTests.Code;

/// <summary>
/// Unit tests for <see cref="ArchitectureManager"/> covering add and timestamp logic for systems, layers, components, and features.
/// </summary>
[UnitTest]
[EnrichedTest]
[Component("Code")]
public class ArchitectureManagerTests
{
    private readonly FakeArchitectureRepository _repository = new();
    private readonly ILogger<ArchitectureManager> _logger = NullLogger<ArchitectureManager>.Instance;
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly FakeTimeProvider _timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 22, 1, 2, 3, TimeSpan.Zero));

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> for the default tenant.
    /// </summary>
    /// <returns>A <see cref="ClaimsPrincipal"/> instance.</returns>
    private ClaimsPrincipal CreatePrincipal() => Impersonation.Impersonate(TENANT_ID);

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> for the default tenant with a specific <see cref="PermissionLevel"/>.
    /// </summary>
    /// <param name="level">The permission level to assign.</param>
    /// <returns>A <see cref="ClaimsPrincipal"/> instance.</returns>
    private ClaimsPrincipal CreatePrincipal(PermissionLevel level) => Impersonation.Impersonate(x =>
    {
        x.UserName = x.Email = "user@user.com";
        x.TenantId = TENANT_ID;
        x.Add(PermissionEntityType.Architecture, level);
    });

    private const string TENANT_ID = "tenant-1";

    /// <summary>
    /// Creates an <see cref="ArchitectureManager"/> using the default time provider.
    /// </summary>
    /// <returns>An <see cref="ArchitectureManager"/> instance.</returns>
    private ArchitectureManager CreateManager() =>
        new(_repository, _logger, _mediator, _timeProvider);

    /// <summary>
    /// Creates an <see cref="ArchitectureManager"/> using a specified <see cref="TimeProvider"/>.
    /// </summary>
    /// <param name="timeProvider">The time provider to use.</param>
    /// <returns>An <see cref="ArchitectureManager"/> instance.</returns>
    private ArchitectureManager CreateManager(TimeProvider timeProvider) =>
        new(_repository, _logger, _mediator, timeProvider);

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.AddSystemAsync"/> adds a system.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddSystemAsync_AddsSystem()
    {
        var principal = CreatePrincipal();
        var manager = CreateManager();
        var system = new ProductSystem { Name = "System1", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddSystemAsync(principal, system);

        var systems = await manager.GetSystemsAsync(principal, 1);
        Assert.Contains(systems, s => s.Name == "System1");
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.AddLayerAsync"/> adds a layer.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddLayerAsync_AddsLayer()
    {
        var principal = CreatePrincipal();
        var manager = CreateManager();
        var layer = new ArchitecturalLayer { Name = "Layer1", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddLayerAsync(principal, layer);

        var layers = await manager.GetLayersAsync(principal, 1);
        Assert.Contains(layers, l => l.Name == "Layer1");
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.AddComponentAsync"/> adds a component.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddComponentAsync_AddsComponent()
    {
        var principal = CreatePrincipal();
        var manager = CreateManager();
        var component = new Component { Name = "Component1", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddComponentAsync(principal, component);

        var components = await manager.GetComponentsAsync(principal, 1);
        Assert.Contains(components, c => c.Name == "Component1");
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.AddFeatureAsync"/> adds a feature.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddFeatureAsync_AddsFeature()
    {
        var principal = CreatePrincipal();
        var manager = CreateManager();
        var feature = new Feature { Name = "Feature1", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddFeatureAsync(principal, feature);

        var features = await manager.GetFeaturesAsync(principal, 1);
        Assert.Contains(features, f => f.Name == "Feature1");
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.AddSystemAsync"/> sets correct timestamps and user info.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddSystemAsync_SetsCorrectTimestamps()
    {
        var principal = CreatePrincipal();
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 22, 1, 2, 3, TimeSpan.Zero));
        var manager = CreateManager(timeProvider);
        var now = _timeProvider.GetUtcNow();
        var system = new ProductSystem { Name = "SystemTimestamp", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddSystemAsync(principal, system);

        var systems = await manager.GetSystemsAsync(principal, 1);
        var added = systems.First(s => s.Name == "SystemTimestamp");
        Assert.Equal(now, added.Created);
        Assert.Equal(now, added.Modified);
        Assert.Equal(principal.Identity?.Name, added.CreatedBy);
        Assert.Equal(principal.Identity?.Name, added.ModifiedBy);
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.AddLayerAsync"/> sets correct timestamps and user info.
    /// </summary>
    [FunctionalTest]
    [Fact]
    public async Task AddLayerAsync_SetsCorrectTimestamps()
    {
        var principal = CreatePrincipal();
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 22, 1, 2, 3, TimeSpan.Zero));
        var manager = CreateManager(timeProvider);
        var now = _timeProvider.GetUtcNow();
        var layer = new ArchitecturalLayer { Name = "LayerTimestamp", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddLayerAsync(principal, layer);

        var layers = await manager.GetLayersAsync(principal, 1);
        var added = layers.First(l => l.Name == "LayerTimestamp");
        Assert.Equal(now, added.Created);
        Assert.Equal(now, added.Modified);
        Assert.Equal(principal.Identity?.Name, added.CreatedBy);
        Assert.Equal(principal.Identity?.Name, added.ModifiedBy);
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.AddComponentAsync"/> sets correct timestamps and user info.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddComponentAsync_SetsCorrectTimestamps()
    {
        var principal = CreatePrincipal();
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 22, 1, 2, 3, TimeSpan.Zero));
        var manager = CreateManager(timeProvider);
        var now = _timeProvider.GetUtcNow();
        var component = new Component { Name = "ComponentTimestamp", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddComponentAsync(principal, component);

        var components = await manager.GetComponentsAsync(principal, 1);
        var added = components.First(c => c.Name == "ComponentTimestamp");
        Assert.Equal(now, added.Created);
        Assert.Equal(now, added.Modified);
        Assert.Equal(principal.Identity?.Name, added.CreatedBy);
        Assert.Equal(principal.Identity?.Name, added.ModifiedBy);
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.AddFeatureAsync"/> sets correct timestamps and user info.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddFeatureAsync_SetsCorrectTimestamps()
    {
        var principal = CreatePrincipal();
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 22, 1, 2, 3, TimeSpan.Zero));
        var manager = CreateManager(timeProvider);
        var now = timeProvider.GetUtcNow();
        var feature = new Feature { Name = "FeatureTimestamp", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddFeatureAsync(principal, feature);

        var features = await manager.GetFeaturesAsync(principal, 1);
        var added = features.First(f => f.Name == "FeatureTimestamp");
        // If AddFeatureAsync does not set timestamps, this will fail and the implementation should be updated.
        Assert.Equal(now, added.Created);
        Assert.Equal(now, added.Modified);
        Assert.Equal(principal.Identity?.Name, added.CreatedBy);
        Assert.Equal(principal.Identity?.Name, added.ModifiedBy);
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.UpdateSystemAsync"/> updates a system.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateSystemAsync_UpdatesSystem()
    {
        var principal = CreatePrincipal();
        var manager = CreateManager();
        var system = new ProductSystem { Name = "SystemToUpdate", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddSystemAsync(principal, system);

        var systems = await manager.GetSystemsAsync(principal, 1);
        var added = systems.First(s => s.Name == "SystemToUpdate");
        added.Description = "Updated description";
        await manager.UpdateSystemAsync(principal, added);

        var updated = (await manager.GetSystemsAsync(principal, 1)).First(s => s.Name == "SystemToUpdate");
        Assert.Equal("Updated description", updated.Description);
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.UpdateLayerAsync"/> updates a layer.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateLayerAsync_UpdatesLayer()
    {
        var principal = CreatePrincipal();
        var manager = CreateManager();
        var layer = new ArchitecturalLayer { Name = "LayerToUpdate", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddLayerAsync(principal, layer);

        var layers = await manager.GetLayersAsync(principal, 1);
        var added = layers.First(l => l.Name == "LayerToUpdate");
        added.Description = "Updated description";
        await manager.UpdateLayerAsync(principal, added);

        var updated = (await manager.GetLayersAsync(principal, 1)).First(l => l.Name == "LayerToUpdate");
        Assert.Equal("Updated description", updated.Description);
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.UpdateComponentAsync"/> updates a component.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateComponentAsync_UpdatesComponent()
    {
        var principal = CreatePrincipal();
        var manager = CreateManager();
        var component = new Component { Name = "ComponentToUpdate", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddComponentAsync(principal, component);

        var components = await manager.GetComponentsAsync(principal, 1);
        var added = components.First(c => c.Name == "ComponentToUpdate");
        added.Description = "Updated description";
        await manager.UpdateComponentAsync(principal, added);

        var updated = (await manager.GetComponentsAsync(principal, 1)).First(c => c.Name == "ComponentToUpdate");
        Assert.Equal("Updated description", updated.Description);
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.UpdateFeatureAsync"/> updates a feature.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateFeatureAsync_UpdatesFeature()
    {
        var principal = CreatePrincipal();
        var manager = CreateManager();
        var feature = new Feature { Name = "FeatureToUpdate", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddFeatureAsync(principal, feature);

        var features = await manager.GetFeaturesAsync(principal, 1);
        var added = features.First(f => f.Name == "FeatureToUpdate");
        added.Description = "Updated description";
        await manager.UpdateFeatureAsync(principal, added);

        var updated = (await manager.GetFeaturesAsync(principal, 1)).First(f => f.Name == "FeatureToUpdate");
        Assert.Equal("Updated description", updated.Description);
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.UpdateSystemAsync"/> sets correct Modified timestamp and user info.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateSystemAsync_SetsCorrectModifiedTimestamp()
    {
        var principal = CreatePrincipal();
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 22, 1, 2, 3, TimeSpan.Zero));
        var manager = CreateManager(timeProvider);
        var system = new ProductSystem { Name = "SystemTimestampUpdate", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddSystemAsync(principal, system);

        var added = (await manager.GetSystemsAsync(principal, 1)).First(s => s.Name == "SystemTimestampUpdate");
        timeProvider.SetDateTime(new DateTimeOffset(2025, 9, 23, 2, 3, 4, TimeSpan.Zero));
        added.Description = "Updated";
        await manager.UpdateSystemAsync(principal, added);

        var updated = (await manager.GetSystemsAsync(principal, 1)).First(s => s.Name == "SystemTimestampUpdate");
        var now = timeProvider.GetUtcNow();
        Assert.Equal(now, updated.Modified);
        Assert.Equal(principal.Identity?.Name, updated.ModifiedBy);
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.UpdateLayerAsync"/> sets correct Modified timestamp and user info.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateLayerAsync_SetsCorrectModifiedTimestamp()
    {
        var principal = CreatePrincipal();
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 22, 1, 2, 3, TimeSpan.Zero));
        var manager = CreateManager(timeProvider);
        var layer = new ArchitecturalLayer { Name = "LayerTimestampUpdate", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddLayerAsync(principal, layer);

        var added = (await manager.GetLayersAsync(principal, 1)).First(l => l.Name == "LayerTimestampUpdate");
        timeProvider.SetDateTime(new DateTimeOffset(2025, 9, 23, 2, 3, 4, TimeSpan.Zero));
        added.Description = "Updated";
        await manager.UpdateLayerAsync(principal, added);

        var updated = (await manager.GetLayersAsync(principal, 1)).First(l => l.Name == "LayerTimestampUpdate");
        var now = timeProvider.GetUtcNow();
        Assert.Equal(now, updated.Modified);
        Assert.Equal(principal.Identity?.Name, updated.ModifiedBy);
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.UpdateComponentAsync"/> sets correct Modified timestamp and user info.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateComponentAsync_SetsCorrectModifiedTimestamp()
    {
        var principal = CreatePrincipal();
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 22, 1, 2, 3, TimeSpan.Zero));
        var manager = CreateManager(timeProvider);
        var component = new Component { Name = "ComponentTimestampUpdate", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddComponentAsync(principal, component);

        var added = (await manager.GetComponentsAsync(principal, 1)).First(c => c.Name == "ComponentTimestampUpdate");
        timeProvider.SetDateTime(new DateTimeOffset(2025, 9, 23, 2, 3, 4, TimeSpan.Zero));
        added.Description = "Updated";
        await manager.UpdateComponentAsync(principal, added);

        var updated = (await manager.GetComponentsAsync(principal, 1)).First(c => c.Name == "ComponentTimestampUpdate");
        var now = timeProvider.GetUtcNow();
        Assert.Equal(now, updated.Modified);
        Assert.Equal(principal.Identity?.Name, updated.ModifiedBy);
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.UpdateFeatureAsync"/> sets correct Modified timestamp and user info.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateFeatureAsync_SetsCorrectModifiedTimestamp()
    {
        var principal = CreatePrincipal();
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 22, 1, 2, 3, TimeSpan.Zero));
        var manager = CreateManager(timeProvider);
        var feature = new Feature { Name = "FeatureTimestampUpdate", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddFeatureAsync(principal, feature);

        var added = (await manager.GetFeaturesAsync(principal, 1)).First(f => f.Name == "FeatureTimestampUpdate");
        timeProvider.SetDateTime(new DateTimeOffset(2025, 9, 23, 2, 3, 4, TimeSpan.Zero));
        added.Description = "Updated";
        await manager.UpdateFeatureAsync(principal, added);

        var updated = (await manager.GetFeaturesAsync(principal, 1)).First(f => f.Name == "FeatureTimestampUpdate");
        var now = timeProvider.GetUtcNow();
        Assert.Equal(now, updated.Modified);
        Assert.Equal(principal.Identity?.Name, updated.ModifiedBy);
    }
    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.DeleteSystemAsync"/> deletes a system.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task DeleteSystemAsync_DeletesSystem()
    {
        var principal = CreatePrincipal(PermissionLevel.All);
        var manager = CreateManager();
        var system = new ProductSystem { Name = "SystemToDelete", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddSystemAsync(principal, system);

        var systems = await manager.GetSystemsAsync(principal, 1);
        var added = systems.First(s => s.Name == "SystemToDelete");
        await manager.DeleteSystemAsync(principal, added);

        var afterDelete = await manager.GetSystemsAsync(principal, 1);
        Assert.DoesNotContain(afterDelete, s => s.Name == "SystemToDelete");
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.DeleteLayerAsync"/> deletes a layer.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task DeleteLayerAsync_DeletesLayer()
    {
        var principal = CreatePrincipal(PermissionLevel.All);
        var manager = CreateManager();
        var layer = new ArchitecturalLayer { Name = "LayerToDelete", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddLayerAsync(principal, layer);

        var layers = await manager.GetLayersAsync(principal, 1);
        var added = layers.First(l => l.Name == "LayerToDelete");
        await manager.DeleteLayerAsync(principal, added);

        var afterDelete = await manager.GetLayersAsync(principal, 1);
        Assert.DoesNotContain(afterDelete, l => l.Name == "LayerToDelete");
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.DeleteComponentAsync"/> deletes a component.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task DeleteComponentAsync_DeletesComponent()
    {
        var principal = CreatePrincipal(PermissionLevel.All);
        var manager = CreateManager();
        var component = new Component { Name = "ComponentToDelete", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddComponentAsync(principal, component);

        var components = await manager.GetComponentsAsync(principal, 1);
        var added = components.First(c => c.Name == "ComponentToDelete");
        await manager.DeleteComponentAsync(principal, added);

        var afterDelete = await manager.GetComponentsAsync(principal, 1);
        Assert.DoesNotContain(afterDelete, c => c.Name == "ComponentToDelete");
    }

    /// <summary>
    /// Verifies that <see cref="ArchitectureManager.DeleteFeatureAsync"/> deletes a feature.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task DeleteFeatureAsync_DeletesFeature()
    {
        var principal = CreatePrincipal(PermissionLevel.All);
        var manager = CreateManager();
        var feature = new Feature { Name = "FeatureToDelete", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await manager.AddFeatureAsync(principal, feature);

        var features = await manager.GetFeaturesAsync(principal, 1);
        var added = features.First(f => f.Name == "FeatureToDelete");
        await manager.DeleteFeatureAsync(principal, added);

        var afterDelete = await manager.GetFeaturesAsync(principal, 1);
        Assert.DoesNotContain(afterDelete, f => f.Name == "FeatureToDelete");
    }


    /// <summary>
    /// Verifies that UnauthorizedAccessException is thrown if the principal does not have the required PermissionLevel.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task AddSystemAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksPermission()
    {
        var principal = CreatePrincipal(PermissionLevel.Read); // Only Read, not Write
        var manager = CreateManager();
        var system = new ProductSystem { Name = "UnauthorizedSystem", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.AddSystemAsync(principal, system);
        });
    }

    /// <summary>
    /// Verifies that AddSystemAsync throws UnauthorizedAccessException if the principal does not have Write permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task AddSystemAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksWritePermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithRead = CreatePrincipal(PermissionLevel.Read);
        var manager = CreateManager();
        var system = new ProductSystem { Name = "SystemUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions to ensure tenant is correct
        await manager.AddSystemAsync(principalWithAll, system);

        // Test: Try to add with insufficient permissions
        var newSystem = new ProductSystem { Name = "SystemUnauthorized2", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.AddSystemAsync(principalWithRead, newSystem);
        });
    }

    /// <summary>
    /// Verifies that UpdateSystemAsync throws UnauthorizedAccessException if the principal does not have Write permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task UpdateSystemAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksWritePermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithRead = CreatePrincipal(PermissionLevel.Read);
        var manager = CreateManager();
        var system = new ProductSystem { Name = "SystemToUpdateUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions
        await manager.AddSystemAsync(principalWithAll, system);

        // Test: Try to update with insufficient permissions
        var systems = await manager.GetSystemsAsync(principalWithAll, 1);
        var added = systems.First(s => s.Name == "SystemToUpdateUnauthorized");
        added.Description = "Should not update";

        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.UpdateSystemAsync(principalWithRead, added);
        });
    }

    /// <summary>
    /// Verifies that AddLayerAsync throws UnauthorizedAccessException if the principal does not have Write permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task AddLayerAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksWritePermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithRead = CreatePrincipal(PermissionLevel.Read);
        var manager = CreateManager();
        var layer = new ArchitecturalLayer { Name = "LayerUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions to ensure tenant is correct
        await manager.AddLayerAsync(principalWithAll, layer);

        // Test: Try to add with insufficient permissions
        var newLayer = new ArchitecturalLayer { Name = "LayerUnauthorized2", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.AddLayerAsync(principalWithRead, newLayer);
        });
    }

    /// <summary>
    /// Verifies that UpdateLayerAsync throws UnauthorizedAccessException if the principal does not have Write permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task UpdateLayerAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksWritePermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithRead = CreatePrincipal(PermissionLevel.Read);
        var manager = CreateManager();
        var layer = new ArchitecturalLayer { Name = "LayerToUpdateUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions
        await manager.AddLayerAsync(principalWithAll, layer);

        // Test: Try to update with insufficient permissions
        var layers = await manager.GetLayersAsync(principalWithAll, 1);
        var added = layers.First(l => l.Name == "LayerToUpdateUnauthorized");
        added.Description = "Should not update";

        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.UpdateLayerAsync(principalWithRead, added);
        });
    }

    /// <summary>
    /// Verifies that AddComponentAsync throws UnauthorizedAccessException if the principal does not have Write permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task AddComponentAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksWritePermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithRead = CreatePrincipal(PermissionLevel.Read);
        var manager = CreateManager();
        var component = new Component { Name = "ComponentUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions to ensure tenant is correct
        await manager.AddComponentAsync(principalWithAll, component);

        // Test: Try to add with insufficient permissions
        var newComponent = new Component { Name = "ComponentUnauthorized2", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.AddComponentAsync(principalWithRead, newComponent);
        });
    }

    /// <summary>
    /// Verifies that UpdateComponentAsync throws UnauthorizedAccessException if the principal does not have Write permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task UpdateComponentAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksWritePermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithRead = CreatePrincipal(PermissionLevel.Read);
        var manager = CreateManager();
        var component = new Component { Name = "ComponentToUpdateUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions
        await manager.AddComponentAsync(principalWithAll, component);

        // Test: Try to update with insufficient permissions
        var components = await manager.GetComponentsAsync(principalWithAll, 1);
        var added = components.First(c => c.Name == "ComponentToUpdateUnauthorized");
        added.Description = "Should not update";

        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.UpdateComponentAsync(principalWithRead, added);
        });
    }

    /// <summary>
    /// Verifies that AddFeatureAsync throws UnauthorizedAccessException if the principal does not have Write permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task AddFeatureAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksWritePermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithRead = CreatePrincipal(PermissionLevel.Read);
        var manager = CreateManager();
        var feature = new Feature { Name = "FeatureUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions to ensure tenant is correct
        await manager.AddFeatureAsync(principalWithAll, feature);

        // Test: Try to add with insufficient permissions
        var newFeature = new Feature { Name = "FeatureUnauthorized2", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.AddFeatureAsync(principalWithRead, newFeature);
        });
    }

    /// <summary>
    /// Verifies that UpdateFeatureAsync throws UnauthorizedAccessException if the principal does not have Write permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task UpdateFeatureAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksWritePermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithRead = CreatePrincipal(PermissionLevel.Read);
        var manager = CreateManager();
        var feature = new Feature { Name = "FeatureToUpdateUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions
        await manager.AddFeatureAsync(principalWithAll, feature);

        // Test: Try to update with insufficient permissions
        var features = await manager.GetFeaturesAsync(principalWithAll, 1);
        var added = features.First(f => f.Name == "FeatureToUpdateUnauthorized");
        added.Description = "Should not update";

        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.UpdateFeatureAsync(principalWithRead, added);
        });
    }
    /// <summary>
    /// Verifies that DeleteSystemAsync throws UnauthorizedAccessException if the principal does not have Delete permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task DeleteSystemAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksDeletePermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithRead = CreatePrincipal(PermissionLevel.Read);
        var principalWithWrite = CreatePrincipal(PermissionLevel.Write);
        var manager = CreateManager();
        var system = new ProductSystem { Name = "SystemToDeleteUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions
        await manager.AddSystemAsync(principalWithAll, system);
        var added = (await manager.GetSystemsAsync(principalWithAll, 1)).First(s => s.Name == "SystemToDeleteUnauthorized");

        // Test: Try to delete with insufficient permissions (Read)
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.DeleteSystemAsync(principalWithRead, added);
        });

        // Test: Try to delete with insufficient permissions (Write)
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.DeleteSystemAsync(principalWithWrite, added);
        });
    }

    /// <summary>
    /// Verifies that DeleteLayerAsync throws UnauthorizedAccessException if the principal does not have Delete permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task DeleteLayerAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksDeletePermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithRead = CreatePrincipal(PermissionLevel.Read);
        var principalWithWrite = CreatePrincipal(PermissionLevel.Write);
        var manager = CreateManager();
        var layer = new ArchitecturalLayer { Name = "LayerToDeleteUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions
        await manager.AddLayerAsync(principalWithAll, layer);
        var added = (await manager.GetLayersAsync(principalWithAll, 1)).First(l => l.Name == "LayerToDeleteUnauthorized");

        // Test: Try to delete with insufficient permissions (Read)
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.DeleteLayerAsync(principalWithRead, added);
        });

        // Test: Try to delete with insufficient permissions (Write)
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.DeleteLayerAsync(principalWithWrite, added);
        });
    }

    /// <summary>
    /// Verifies that DeleteComponentAsync throws UnauthorizedAccessException if the principal does not have Delete permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task DeleteComponentAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksDeletePermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithRead = CreatePrincipal(PermissionLevel.Read);
        var principalWithWrite = CreatePrincipal(PermissionLevel.Write);
        var manager = CreateManager();
        var component = new Component { Name = "ComponentToDeleteUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions
        await manager.AddComponentAsync(principalWithAll, component);
        var added = (await manager.GetComponentsAsync(principalWithAll, 1)).First(c => c.Name == "ComponentToDeleteUnauthorized");

        // Test: Try to delete with insufficient permissions (Read)
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.DeleteComponentAsync(principalWithRead, added);
        });

        // Test: Try to delete with insufficient permissions (Write)
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.DeleteComponentAsync(principalWithWrite, added);
        });
    }

    /// <summary>
    /// Verifies that DeleteFeatureAsync throws UnauthorizedAccessException if the principal does not have Delete permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task DeleteFeatureAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksDeletePermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithRead = CreatePrincipal(PermissionLevel.Read);
        var principalWithWrite = CreatePrincipal(PermissionLevel.Write);
        var manager = CreateManager();
        var feature = new Feature { Name = "FeatureToDeleteUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions
        await manager.AddFeatureAsync(principalWithAll, feature);
        var added = (await manager.GetFeaturesAsync(principalWithAll, 1)).First(f => f.Name == "FeatureToDeleteUnauthorized");

        // Test: Try to delete with insufficient permissions (Read)
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.DeleteFeatureAsync(principalWithRead, added);
        });

        // Test: Try to delete with insufficient permissions (Write)
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.DeleteFeatureAsync(principalWithWrite, added);
        });
    }

    /// <summary>
    /// Verifies that GetSystemsAsync throws UnauthorizedAccessException if the principal does not have Read permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task GetSystemsAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksReadPermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithNone = CreatePrincipal(PermissionLevel.None);
        var manager = CreateManager();
        var system = new ProductSystem { Name = "SystemGetUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions
        await manager.AddSystemAsync(principalWithAll, system);

        // Test: Try to get with insufficient permissions
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.GetSystemsAsync(principalWithNone, 1);
        });
    }

    /// <summary>
    /// Verifies that GetLayersAsync throws UnauthorizedAccessException if the principal does not have Read permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task GetLayersAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksReadPermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithNone = CreatePrincipal(PermissionLevel.None);
        var manager = CreateManager();
        var layer = new ArchitecturalLayer { Name = "LayerGetUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions
        await manager.AddLayerAsync(principalWithAll, layer);

        // Test: Try to get with insufficient permissions
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.GetLayersAsync(principalWithNone, 1);
        });
    }

    /// <summary>
    /// Verifies that GetComponentsAsync throws UnauthorizedAccessException if the principal does not have Read permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task GetComponentsAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksReadPermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithNone = CreatePrincipal(PermissionLevel.None);
        var manager = CreateManager();
        var component = new Component { Name = "ComponentGetUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions
        await manager.AddComponentAsync(principalWithAll, component);

        // Test: Try to get with insufficient permissions
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.GetComponentsAsync(principalWithNone, 1);
        });
    }

    /// <summary>
    /// Verifies that GetFeaturesAsync throws UnauthorizedAccessException if the principal does not have Read permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task GetFeaturesAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksReadPermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithNone = CreatePrincipal(PermissionLevel.None);
        var manager = CreateManager();
        var feature = new Feature { Name = "FeatureGetUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions
        await manager.AddFeatureAsync(principalWithAll, feature);

        // Test: Try to get with insufficient permissions
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.GetFeaturesAsync(principalWithNone, 1);
        });
    }

    /// <summary>
    /// Verifies that GetSystemByNameAsync throws UnauthorizedAccessException if the principal does not have Read permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task GetSystemByNameAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksReadPermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithNone = CreatePrincipal(PermissionLevel.None);
        var manager = CreateManager();
        var system = new ProductSystem { Name = "SystemByNameUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions
        await manager.AddSystemAsync(principalWithAll, system);

        // Test: Try to get by name with insufficient permissions
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.GetSystemByNameAsync(principalWithNone, 1, "SystemByNameUnauthorized");
        });
    }

    /// <summary>
    /// Verifies that GetLayerByNameAsync throws UnauthorizedAccessException if the principal does not have Read permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task GetLayerByNameAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksReadPermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithNone = CreatePrincipal(PermissionLevel.None);
        var manager = CreateManager();
        var layer = new ArchitecturalLayer { Name = "LayerByNameUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions
        await manager.AddLayerAsync(principalWithAll, layer);

        // Test: Try to get by name with insufficient permissions
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.GetLayerByNameAsync(principalWithNone, 1, "LayerByNameUnauthorized");
        });
    }

    /// <summary>
    /// Verifies that GetComponentByNameAsync throws UnauthorizedAccessException if the principal does not have Read permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task GetComponentByNameAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksReadPermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithNone = CreatePrincipal(PermissionLevel.None);
        var manager = CreateManager();
        var component = new Component { Name = "ComponentByNameUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions
        await manager.AddComponentAsync(principalWithAll, component);

        // Test: Try to get by name with insufficient permissions
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.GetComponentByNameAsync(principalWithNone, 1, "ComponentByNameUnauthorized");
        });
    }

    /// <summary>
    /// Verifies that GetFeatureByNameAsync throws UnauthorizedAccessException if the principal does not have Read permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task GetFeatureByNameAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksReadPermission()
    {
        var principalWithAll = CreatePrincipal(PermissionLevel.All);
        var principalWithNone = CreatePrincipal(PermissionLevel.None);
        var manager = CreateManager();
        var feature = new Feature { Name = "FeatureByNameUnauthorized", GlobPatterns = new(), TenantId = TENANT_ID, TestProjectId = 1 };

        // Setup: Add with All permissions
        await manager.AddFeatureAsync(principalWithAll, feature);

        // Test: Try to get by name with insufficient permissions
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.GetFeatureByNameAsync(principalWithNone, 1, "FeatureByNameUnauthorized");
        });
    }
    /// <summary>
    /// Verifies that ImportProductArchitectureAsync imports systems, layers, components, and features.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task ImportProductArchitectureAsync_ImportsAllEntities()
    {
        var principal = CreatePrincipal(PermissionLevel.All);
        var manager = CreateManager();
        var project = new TestProject { Name = "Test Project", ShortName = "TP", Slug = "test_project", Id = 5 };

        var model = new ProjectArchitectureModel
        {
            Systems = new Dictionary<string, ArchitecturalComponent>
            {
                ["Rocket"] = new ArchitecturalComponent
                {
                    Description = "Rocket system",
                    DevLead = "Alice",
                    TestLead = "Bob",
                    Paths = new List<string> { "src/rocket/**/*" }
                }
            },
            Features = new Dictionary<string, ArchitecturalComponent>
            {
                ["Launch Control"] = new ArchitecturalComponent
                {
                    Description = "Launch control feature",
                    DevLead = "Alice",
                    TestLead = "Bob",
                    Paths = new List<string> { "src/rocket/lanch/control/**/*" }
                }
            },

            Components = new Dictionary<string, ArchitecturalComponent>
            {
                ["Engine"] = new ArchitecturalComponent
                {
                    Description = "Rocket engine",
                    DevLead = "Alice",
                    TestLead = "Bob",
                    Paths = new List<string> { "src/engine/**/*" }
                }
            },

            Layers = new Dictionary<string, ArchitecturalComponent>
            {
                ["Lunar Lander"] = new ArchitecturalComponent
                {
                    Description = "",
                    DevLead = "Alice",
                    TestLead = "Bob",
                    Paths = new List<string> { "src/lander/**/*" }
                }
            },
        };


        await manager.ImportProductArchitectureAsync(principal, project, model);

        var systems = await manager.GetSystemsAsync(principal, 5);
        var layers = await manager.GetLayersAsync(principal, 5);
        var components = await manager.GetComponentsAsync(principal, 5);
        var features = await manager.GetFeaturesAsync(principal, 5);

        Assert.Contains(systems, s => s.Name == "Rocket");
        Assert.Contains(layers, l => l.Name == "Lunar Lander");
        Assert.Contains(components, c => c.Name == "Engine");
        Assert.Contains(features, f => f.Name == "Launch Control");
    }

    /// <summary>
    /// Verifies that ImportProductArchitectureAsync throws UnauthorizedAccessException if the principal lacks permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task ImportProductArchitectureAsync_ThrowsUnauthorizedAccessException_IfPrincipalLacksPermission()
    {
        var principal = CreatePrincipal(PermissionLevel.Read);
        var manager = CreateManager();
        var project = new TestProject { Name = "Test Project", ShortName = "TP", Slug = "test_project" };
        var model = new ProjectArchitectureModel
        {
            Systems = new Dictionary<string, ArchitecturalComponent>
            {
                ["Rocket"] = new ArchitecturalComponent
                {
                    Description = "Rocket system",
                    DevLead = "Alice",
                    TestLead = "Bob",
                    Paths = new List<string> { "src/rocket/**/*" }
                }
            },
            Components = new Dictionary<string, ArchitecturalComponent>(),
            Layers = new Dictionary<string, ArchitecturalComponent>(),
            Features = new Dictionary<string, ArchitecturalComponent>()
        };


        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await manager.ImportProductArchitectureAsync(principal, project, model);
        });
    }
    /// <summary>
    /// Verifies that ImportProductArchitectureAsync updates existing entities if they already exist.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task ImportProductArchitectureAsync_UpdatesExistingEntities()
    {
        var principal = CreatePrincipal(PermissionLevel.All);
        var manager = CreateManager();
        var project = new TestProject { Name = "Test Project", ShortName = "TP", Slug = "test_project", Id = 10 };

        // First import
        var model1 = new ProjectArchitectureModel
        {
            Systems = new Dictionary<string, ArchitecturalComponent>
            {
                ["Rocket"] = new ArchitecturalComponent
                {
                    Description = "Rocket system",
                    DevLead = "Alice",
                    TestLead = "Bob",
                    Paths = new List<string> { "src/rocket/**/*" }
                }
            },
            Features = new Dictionary<string, ArchitecturalComponent>(),
            Components = new Dictionary<string, ArchitecturalComponent>(),
            Layers = new Dictionary<string, ArchitecturalComponent>()
        };
        await manager.ImportProductArchitectureAsync(principal, project, model1);

        // Second import with updated description
        var model2 = new ProjectArchitectureModel
        {
            Systems = new Dictionary<string, ArchitecturalComponent>
            {
                ["Rocket"] = new ArchitecturalComponent
                {
                    Description = "Updated Rocket system",
                    DevLead = "Alice",
                    TestLead = "Bob",
                    Paths = new List<string> { "src/rocket/**/*" }
                }
            },
            Features = new Dictionary<string, ArchitecturalComponent>(),
            Components = new Dictionary<string, ArchitecturalComponent>(),
            Layers = new Dictionary<string, ArchitecturalComponent>()
        };
        await manager.ImportProductArchitectureAsync(principal, project, model2);

        var systems = await manager.GetSystemsAsync(principal, 10);
        var rocket = systems.FirstOrDefault(s => s.Name == "Rocket");
        Assert.NotNull(rocket);
        Assert.Equal("Updated Rocket system", rocket.Description);
    }

    /// <summary>
    /// Verifies that ImportProductArchitectureAsync handles empty model gracefully (no entities imported).
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task ImportProductArchitectureAsync_HandlesEmptyModel()
    {
        var principal = CreatePrincipal(PermissionLevel.All);
        var manager = CreateManager();
        var project = new TestProject { Name = "Empty Project", ShortName = "EP", Slug = "empty_project", Id = 11 };

        var model = new ProjectArchitectureModel
        {
            Systems = new Dictionary<string, ArchitecturalComponent>(),
            Features = new Dictionary<string, ArchitecturalComponent>(),
            Components = new Dictionary<string, ArchitecturalComponent>(),
            Layers = new Dictionary<string, ArchitecturalComponent>()
        };

        await manager.ImportProductArchitectureAsync(principal, project, model);

        var systems = await manager.GetSystemsAsync(principal, 11);
        var layers = await manager.GetLayersAsync(principal, 11);
        var components = await manager.GetComponentsAsync(principal, 11);
        var features = await manager.GetFeaturesAsync(principal, 11);

        Assert.Empty(systems);
        Assert.Empty(layers);
        Assert.Empty(components);
        Assert.Empty(features);
    }

    /// <summary>
    /// Verifies that ImportProductArchitectureAsync sets correct timestamps and user info for imported entities.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task ImportProductArchitectureAsync_SetsTimestampsAndUserInfo()
    {
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 22, 1, 2, 3, TimeSpan.Zero));
        var principal = CreatePrincipal(PermissionLevel.All);
        var manager = CreateManager(timeProvider);
        var project = new TestProject { Name = "Timestamp Project", ShortName = "TP", Slug = "timestamp_project", Id = 12 };

        var model = new ProjectArchitectureModel
        {
            Systems = new Dictionary<string, ArchitecturalComponent>
            {
                ["Rocket"] = new ArchitecturalComponent
                {
                    Description = "Rocket system",
                    DevLead = "Alice",
                    TestLead = "Bob",
                    Paths = new List<string> { "src/rocket/**/*" }
                }
            },
            Features = new Dictionary<string, ArchitecturalComponent>(),
            Components = new Dictionary<string, ArchitecturalComponent>(),
            Layers = new Dictionary<string, ArchitecturalComponent>()
        };

        await manager.ImportProductArchitectureAsync(principal, project, model);

        var now = timeProvider.GetUtcNow();
        var systems = await manager.GetSystemsAsync(principal, 12);
        var rocket = systems.FirstOrDefault(s => s.Name == "Rocket");
        Assert.NotNull(rocket);
        Assert.Equal(now, rocket.Created);
        Assert.Equal(now, rocket.Modified);
        Assert.Equal(principal.Identity?.Name, rocket.CreatedBy);
        Assert.Equal(principal.Identity?.Name, rocket.ModifiedBy);
    }

    /// <summary>
    /// Verifies that ImportProductArchitectureAsync deletes systems, layers, components, and features that are removed from the model.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task ImportProductArchitectureAsync_DeletesEntitiesRemovedFromModel()
    {
        var principal = CreatePrincipal(PermissionLevel.All);
        var manager = CreateManager();
        var project = new TestProject { Name = "Delete Entities Project", ShortName = "DEP", Slug = "delete_entities_project", Id = 20 };

        // Initial import with all entities
        var initialModel = new ProjectArchitectureModel
        {
            Systems = new Dictionary<string, ArchitecturalComponent>
            {
                ["Rocket"] = new ArchitecturalComponent
                {
                    Description = "Rocket system",
                    DevLead = "Alice",
                    TestLead = "Bob",
                    Paths = new List<string> { "src/rocket/**/*" }
                }
            },
            Features = new Dictionary<string, ArchitecturalComponent>
            {
                ["Launch Control"] = new ArchitecturalComponent
                {
                    Description = "Launch control feature",
                    DevLead = "Alice",
                    TestLead = "Bob",
                    Paths = new List<string> { "src/rocket/launch/control/**/*" }
                }
            },
            Components = new Dictionary<string, ArchitecturalComponent>
            {
                ["Engine"] = new ArchitecturalComponent
                {
                    Description = "Rocket engine",
                    DevLead = "Alice",
                    TestLead = "Bob",
                    Paths = new List<string> { "src/engine/**/*" }
                }
            },
            Layers = new Dictionary<string, ArchitecturalComponent>
            {
                ["Lunar Lander"] = new ArchitecturalComponent
                {
                    Description = "",
                    DevLead = "Alice",
                    TestLead = "Bob",
                    Paths = new List<string> { "src/lander/**/*" }
                }
            },
        };

        await manager.ImportProductArchitectureAsync(principal, project, initialModel);

        // Verify all entities exist
        Assert.Contains((await manager.GetSystemsAsync(principal, 20)), s => s.Name == "Rocket");
        Assert.Contains((await manager.GetLayersAsync(principal, 20)), l => l.Name == "Lunar Lander");
        Assert.Contains((await manager.GetComponentsAsync(principal, 20)), c => c.Name == "Engine");
        Assert.Contains((await manager.GetFeaturesAsync(principal, 20)), f => f.Name == "Launch Control");

        // Second import with all entities removed
        var emptyModel = new ProjectArchitectureModel
        {
            Systems = new Dictionary<string, ArchitecturalComponent>(),
            Features = new Dictionary<string, ArchitecturalComponent>(),
            Components = new Dictionary<string, ArchitecturalComponent>(),
            Layers = new Dictionary<string, ArchitecturalComponent>()
        };

        await manager.ImportProductArchitectureAsync(principal, project, emptyModel);

        // Verify all entities are deleted
        Assert.Empty(await manager.GetSystemsAsync(principal, 20));
        Assert.Empty(await manager.GetLayersAsync(principal, 20));
        Assert.Empty(await manager.GetComponentsAsync(principal, 20));
        Assert.Empty(await manager.GetFeaturesAsync(principal, 20));
    }
}