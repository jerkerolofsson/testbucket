using System.Threading;
using System.Threading.Tasks;

using TestBucket.Domain.TestResources.Allocation;
using TestBucket.Domain.TestResources.Models;

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
}