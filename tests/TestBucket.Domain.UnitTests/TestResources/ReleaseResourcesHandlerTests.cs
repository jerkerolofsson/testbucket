using System.Threading;
using System.Threading.Tasks;

using TestBucket.Domain.TestAccounts.Allocation;
using TestBucket.Domain.TestAccounts.Models;
using TestBucket.Domain.TestResources.Allocation;
using TestBucket.Domain.TestResources.Models;
using TestBucket.Domain.UnitTests.TestAccounts.Fakes;
using TestBucket.Domain.UnitTests.TestResources.Fakes;

using Xunit;

namespace TestBucket.Domain.UnitTests.TestResources;

/// <summary>
/// Unit tests for the <see cref="ReleaseResourcesHandler"/> class.
/// </summary>
[EnrichedTest]
[UnitTest]
[FunctionalTest]
[Component("Test Resources")]
public class ReleaseResourcesHandlerTests
{
    /// <summary>
    /// Verifies that the handler releases locked resources for a specific owner.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReleaseLockedResources_ForSpecificOwner()
    {
        // Arrange
        var repository = new FakeTestResourceRepository();
        var handler = new ReleaseResourcesHandler(repository);

        var resource1 = new TestResource
        {
            Types = ["type1"],
            ResourceId = "res1",
            Owner = "resource-owner",
            Name = "Resource1",
            TenantId = "tenant1",
            Locked = true,
            LockOwner = "owner1"
        };

        var resource2 = new TestResource
        {
            Types = ["type1"],
            ResourceId = "res2",
            Owner = "resource-owner",
            Name = "Resource2",
            TenantId = "tenant1",
            Locked = true,
            LockOwner = "owner1"
        };

        var resource3 = new TestResource
        {
            Types = [ "type1" ],
            ResourceId = "res3",
            Owner = "resource-owner",
            Name = "Resource3",
            TenantId = "tenant1",
            Locked = true,
            LockOwner = "owner2"
        };

        await repository.AddAsync(resource1);
        await repository.AddAsync(resource2);
        await repository.AddAsync(resource3);

        var request = new ReleaseResourcesRequest("owner1", "tenant1");

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        var updatedResource1 = await repository.GetByIdAsync("tenant1", resource1.Id);
        var updatedResource2 = await repository.GetByIdAsync("tenant1", resource2.Id);
        var unchangedResource3 = await repository.GetByIdAsync("tenant1", resource3.Id);

        Assert.False(updatedResource1!.Locked);
        Assert.Null(updatedResource1.LockOwner);

        Assert.False(updatedResource2!.Locked);
        Assert.Null(updatedResource2.LockOwner);

        Assert.True(unchangedResource3!.Locked);
        Assert.Equal("owner2", unchangedResource3.LockOwner);
    }

    /// <summary>
    /// Verifies that all resources are released when the number of locked resources are larger than a single page
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WithMoreResourcesThanASinglePage_AllResourcesReleased()
    {
        int lockedCount = 25;
        string tenant1 = "tenant-1";
        string lockOwner = "lock-owner123";

        var repository = new FakeTestResourceRepository();

        // Arrange
        var lockedResources = Enumerable.Range(0, lockedCount).Select(i => new TestResource
        {
            Id = i + 1,
            ResourceId = "123" + i,
            Types = ["type"],
            Name = $"TestResource{i}",
            Owner = "owner1",
            Locked = true,
            LockOwner = lockOwner,
            TenantId = tenant1
        }).ToArray();

        foreach (var respirce in lockedResources)
        {
            await repository.AddAsync(respirce);
        }

        var request = new ReleaseResourcesRequest(lockOwner, tenant1);
        var handler = new ReleaseResourcesHandler(repository);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        var resources = await repository.SearchAsync([], 0, lockedCount + 100);
        Assert.Equal(lockedCount, resources.Items.Length);
        foreach (var resource in resources.Items)
        {
            Assert.False(resource.Locked);
            Assert.Null(resource.LockOwner);
            Assert.Null(resource.LockExpires);
        }
    }
}