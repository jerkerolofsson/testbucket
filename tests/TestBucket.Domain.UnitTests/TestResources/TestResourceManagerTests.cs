using System.Security.Claims;

using Mediator;

using NSubstitute;

using TestBucket.Contracts.TestResources;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.TestResources;
using TestBucket.Domain.TestResources.Events;
using TestBucket.Domain.TestResources.Mapping;
using TestBucket.Domain.TestResources.Models;
using TestBucket.Domain.UnitTests.TestResources.Fakes;

namespace TestBucket.Domain.UnitTests.TestResources;

/// <summary>
/// Unit tests for the <see cref="TestResourceManager"/> class.
/// </summary>
[UnitTest]
[EnrichedTest]
[Component("Test Resources")]
public class TestResourceManagerTests
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestResourceManagerTests"/> class.
    /// </summary>
    public TestResourceManagerTests()
    {
        _mediator = Substitute.For<IMediator>();
    }

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> with the specified permission level.
    /// </summary>
    /// <param name="level">The permission level to assign to the principal.</param>
    /// <returns>A <see cref="ClaimsPrincipal"/> with the specified permissions.</returns>
    private ClaimsPrincipal CreatePrincipal(PermissionLevel level)
    {
        return Impersonation.Impersonate(builder =>
        {
            builder.TenantId = "tenant-1";
            builder.UserName = "user1@testbucket.io";
            builder.Add(PermissionEntityType.TestResource, level);
        });
    }

    /// <summary>
    /// Tests that <see cref="TestResourceManager.AddAsync"/> adds a resource and publishes an event.
    /// </summary>
    [Fact]
    [FunctionalTest]

    public async Task AddAsync_ShouldAddResourceAndPublishEvent()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.All);
        var resource = new TestResource { Name = "Resource1", Owner = "owner-1", ResourceId = "123", Types = ["phone"] };
        var manager = new TestResourceManager(new FakeTestResourceRepository(), TimeProvider.System, _mediator);

        // Act
        await manager.AddAsync(principal, resource);

        // Assert
        var addedResource = await manager.GetByIdAsync(principal, resource.Id);
        Assert.NotNull(addedResource);
        Assert.Equal("Resource1", addedResource.Name);
        await _mediator.Received(1).Publish(Arg.Any<TestResourceAdded>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that <see cref="TestResourceManager.UpdateAsync"/> updates a resource and publishes an event.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateAsync_ShouldUpdateResourceAndPublishEvent()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.All);
        var resource = new TestResource { Name = "Resource1", Owner = "owner-1", ResourceId = "123", Types = ["phone"] };
        var manager = new TestResourceManager(new FakeTestResourceRepository(), TimeProvider.System, _mediator);
        await manager.AddAsync(principal, resource);

        resource.Name = "UpdatedResource";

        // Act
        await manager.UpdateAsync(principal, resource);

        // Assert
        var updatedResource = await manager.GetByIdAsync(principal, resource.Id);
        Assert.NotNull(updatedResource);
        Assert.Equal("UpdatedResource", updatedResource.Name);
        await _mediator.Received(1).Publish(Arg.Any<TestResourceUpdated>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that <see cref="TestResourceManager.DeleteAsync"/> removes a resource and publishes an event.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task DeleteAsync_ShouldRemoveResourceAndPublishEvent()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.All);
        var resource = new TestResource { Name = "Resource1", Owner = "owner-1", ResourceId = "123", Types = ["phone"] };
        var manager = new TestResourceManager(new FakeTestResourceRepository(), TimeProvider.System, _mediator);
        await manager.AddAsync(principal, resource);

        // Act
        await manager.DeleteAsync(principal, resource);

        // Assert
        var deletedResource = await manager.GetByIdAsync(principal, resource.Id);
        Assert.Null(deletedResource);
        await _mediator.Received(1).Publish(Arg.Any<TestResourceRemoved>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that <see cref="TestResourceManager.BrowseAsync"/> returns paged resources.
    /// </summary>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="totalCount">The total number of items to create.</param>
    [Theory]
    [InlineData(10, 9)]
    [InlineData(10, 10)]
    [InlineData(10, 11)]
    [InlineData(20, 19)]
    [InlineData(20, 20)]
    [InlineData(20, 21)]
    [FunctionalTest]
    public async Task BrowseAsync_ShouldReturnPagedResources(int pageSize, int totalCount)
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.All);
        var manager = new TestResourceManager(new FakeTestResourceRepository(), TimeProvider.System, _mediator);
        for (int i = 0; i < totalCount; i++)
        {
            var resource1 = new TestResource { Name = "Resource1", Owner = "owner-1", ResourceId = "123", Types = ["phone"] };
            await manager.AddAsync(principal, resource1);
        }

        // Act
        var result = await manager.BrowseAsync(principal, 0, pageSize);

        // Assert
        Assert.NotNull(result);
        if (pageSize <= totalCount)
        {
            Assert.Equal(pageSize, result.Items.Length);
        }
        else
        {
            Assert.Equal(totalCount, result.Items.Length);
        }
        Assert.Equal(totalCount, result.TotalCount);
    }

    /// <summary>
    /// Tests that <see cref="TestResourceManager.GetByIdAsync"/> retrieves a resource by its ID.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task GetByIdAsync_ShouldReturnResourceById()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.All);
        var manager = new TestResourceManager(new FakeTestResourceRepository(), TimeProvider.System, _mediator);
        var resource1 = new TestResource { Name = "Resource1", Owner = "owner-1", ResourceId = "123", Types = ["phone"] };
        await manager.AddAsync(principal, resource1);

        // Act
        var result = await manager.GetByIdAsync(principal, resource1.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Resource1", result.Name);
    }

    /// <summary>
    /// Tests that <see cref="TestResourceManager.UpdateResourcesFromResourceServerAsync"/> enables healthy resources.
    /// </summary>
    [Fact]
    public async Task UpdateResourcesFromResourceServerAsync_ShouldEnableHealthyResources()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var resourceDto = new TestResourceDto
        {
            Name = "Resource1",
            Owner = "owner-1",
            ResourceId = "123",
            Health = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
            Types = ["phone"]
        };
        var manager = new TestResourceManager(new FakeTestResourceRepository(), TimeProvider.System, _mediator);

        // Act
        await manager.UpdateResourcesFromResourceServerAsync(principal, new List<TestResourceDto> { resourceDto });

        // Assert
        var updatedResource = await manager.GetByResourceIdAsync(principal, resourceDto.Owner, resourceDto.ResourceId);
        Assert.NotNull(updatedResource);
        Assert.True(updatedResource.Enabled);
    }

    /// <summary>
    /// Tests that <see cref="TestResourceManager.UpdateResourcesFromResourceServerAsync"/> disables degraded resources.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateResourcesFromResourceServerAsync_ShouldDisableDegradedResources()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var resourceDto = new TestResourceDto
        {
            Name = "Resource1",
            Owner = "owner-1",
            ResourceId = "123",
            Health = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
            Types = ["phone"]
        };
        var manager = new TestResourceManager(new FakeTestResourceRepository(), TimeProvider.System, _mediator);

        // Act
        await manager.UpdateResourcesFromResourceServerAsync(principal, new List<TestResourceDto> { resourceDto });

        // Assert
        var updatedResource = await manager.GetByResourceIdAsync(principal, resourceDto.Owner, resourceDto.ResourceId);
        Assert.NotNull(updatedResource);
        Assert.False(updatedResource.Enabled);
    }

    /// <summary>
    /// Tests that <see cref="TestResourceManager.UpdateResourcesFromResourceServerAsync"/> disables removed resources.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateResourcesFromResourceServerAsync_ShouldDisableRemovedResources()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var existingResource1 = new TestResource
        {
            Name = "Resource1",
            Owner = "owner-1",
            ResourceId = "123",
            Enabled = true,
            Types = ["phone"],
            TenantId = "tenant-1"
        };
        var existingResource2 = new TestResource
        {
            Name = "Resource2",
            Owner = "owner-1",
            ResourceId = "2345",
            Enabled = true,
            Types = ["phone"],
            TenantId = "tenant-1"
        };
        var manager = new TestResourceManager(new FakeTestResourceRepository(), TimeProvider.System, _mediator);
        await manager.AddAsync(principal, existingResource1);
        await manager.AddAsync(principal, existingResource2);

        // Act
        await manager.UpdateResourcesFromResourceServerAsync(principal, [existingResource2.ToDto()]);

        // Assert
        var updatedResource = await manager.GetByResourceIdAsync(principal, existingResource1.Owner, existingResource1.ResourceId);
        Assert.NotNull(updatedResource);
        Assert.False(updatedResource.Enabled);
    }

    /// <summary>
    /// Tests that <see cref="TestResourceManager.UpdateResourcesFromResourceServerAsync"/> adds new resources.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateResourcesFromResourceServerAsync_ShouldAddNewResources()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var resourceDto = new TestResourceDto
        {
            Name = "NewResource",
            Owner = "owner-1",
            ResourceId = "456",
            Health = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
            Types = ["tablet"]
        };
        var manager = new TestResourceManager(new FakeTestResourceRepository(), TimeProvider.System, _mediator);

        // Act
        await manager.UpdateResourcesFromResourceServerAsync(principal, new List<TestResourceDto> { resourceDto });

        // Assert
        var addedResource = await manager.GetByResourceIdAsync(principal, resourceDto.Owner, resourceDto.ResourceId);
        Assert.NotNull(addedResource);
        Assert.Equal("NewResource", addedResource.Name);
        Assert.True(addedResource.Enabled);
    }

    /// <summary>
    /// Tests that <see cref="TestResourceManager.UpdateResourcesFromResourceServerAsync"/> updates existing resources.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateResourcesFromResourceServerAsync_ShouldUpdateExistingResources()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var existingResource = new TestResource
        {
            Name = "Resource1",
            Owner = "owner-1",
            ResourceId = "123",
            Enabled = true,
            Types = ["phone"],
            TenantId = "tenant-1"
        };
        var repository = new FakeTestResourceRepository();
        await repository.AddAsync(existingResource);
        var resourceDto = new TestResourceDto
        {
            Name = "UpdatedResource",
            Owner = "owner-1",
            ResourceId = "123",
            Health = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
            Types = ["tablet"],
        };
        var manager = new TestResourceManager(repository, TimeProvider.System, _mediator);

        // Act
        await manager.UpdateResourcesFromResourceServerAsync(principal, new List<TestResourceDto> { resourceDto });

        // Assert
        var updatedResource = await manager.GetByIdAsync(principal, existingResource.Id);
        Assert.NotNull(updatedResource);
        Assert.Equal("UpdatedResource", updatedResource.Name);
        Assert.Contains("tablet", updatedResource.Types);
    }

    /// <summary>
    /// Tests that <see cref="TestResourceManager.UpdateResourcesFromResourceServerAsync"/> throws an exception for insufficient permissions.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task UpdateResourcesFromResourceServerAsync_ShouldThrowForInsufficientPermissions()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.None);
        var resourceDto = new TestResourceDto
        {
            Name = "Resource1",
            Owner = "owner-1",
            ResourceId = "123",
            Health = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
            Types = ["phone"]
        };
        var manager = new TestResourceManager(new FakeTestResourceRepository(), TimeProvider.System, _mediator);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            manager.UpdateResourcesFromResourceServerAsync(principal, new List<TestResourceDto> { resourceDto }));
    }

    /// <summary>
    /// Tests that <see cref="TestResourceManager.AddAsync"/> throws an exception for insufficient permissions
    /// </summary>
    [Fact]
    [FunctionalTest]

    public async Task AddAsync_ShouldThrowForInsufficientPermissions()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.None);
        var resource = new TestResource { Name = "Resource1", Owner = "owner-1", ResourceId = "123", Types = ["phone"] };
        var manager = new TestResourceManager(new FakeTestResourceRepository(), TimeProvider.System, _mediator);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            manager.AddAsync(principal, resource));
    }

    /// <summary>
    /// Tests that <see cref="TestResourceManager.DeleteAsync"/> throws an exception for insufficient permissions
    /// </summary>
    [Fact]
    [FunctionalTest]

    public async Task DeleteAsync_ShouldThrowForInsufficientPermissions()
    {
        // Arrange
        var admin = CreatePrincipal(PermissionLevel.All);
        var user = CreatePrincipal(PermissionLevel.None);
        var resource = new TestResource { Name = "Resource1", Owner = "owner-1", ResourceId = "123", Types = ["phone"] };
        var manager = new TestResourceManager(new FakeTestResourceRepository(), TimeProvider.System, _mediator);
        await manager.AddAsync(admin, resource);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            manager.DeleteAsync(user, resource));
    }

    /// <summary>
    /// Tests that <see cref="TestResourceManager.DeleteAsync"/> throws an exception for insufficient permissions
    /// </summary>
    [Fact]
    [FunctionalTest]

    public async Task BrowseAsync_ShouldThrowForInsufficientPermissions()
    {
        // Arrange
        var admin = CreatePrincipal(PermissionLevel.All);
        var user = CreatePrincipal(PermissionLevel.None);
        var resource = new TestResource { Name = "Resource1", Owner = "owner-1", ResourceId = "123", Types = ["phone"] };
        var manager = new TestResourceManager(new FakeTestResourceRepository(), TimeProvider.System, _mediator);
        await manager.AddAsync(admin, resource);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            manager.BrowseAsync(user, 0, 10));
    }
}