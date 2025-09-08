using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Settings.Fakes;
using TestBucket.Domain.Tenants;
using TestBucket.Domain.Tenants.Models;
using TestBucket.Domain.UnitTests.Project;

namespace TestBucket.Domain.UnitTests.Tenants;
/// <summary>
/// Unit tests for ITenantRepository
/// </summary>
[Component("Tenant")]
[UnitTest]
[FunctionalTest]
[EnrichedTest]
public class ITenantRepositoryTests
{
    private readonly FakeSettingsProvider _settingsProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantManagerTests"/> class.
    /// </summary>
    public ITenantRepositoryTests()
    {
        _settingsProvider = new FakeSettingsProvider();
    }

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> with permissions to manage tenants and projects.
    /// </summary>
    /// <returns>A <see cref="ClaimsPrincipal"/> with the specified permissions.</returns>
    private ClaimsPrincipal CreatePrincipal()
    {
        var builder = new EntityPermissionBuilder();
        builder.Add(PermissionEntityType.Tenant, PermissionLevel.All);
        builder.Add(PermissionEntityType.Project, PermissionLevel.All);
        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(PermissionClaims.Permissions, PermissionClaimSerializer.Serialize(builder.Build())));
        return new ClaimsPrincipal(identity);
    }

    /// <summary>
    /// Verifies that EnumerateAsync returns all tenants
    /// </summary>
    [Fact]
    public async Task EnumerateAsync_ShouldReturnAllTenants()
    {
        // Arrange
        var principal = CreatePrincipal();
        ITenantRepository repo = new FakeTenantRepository();
        var tenantManager = new TenantManager(new FakeProjectRepository(), repo, _settingsProvider, TimeProvider.System);
        await tenantManager.CreateAsync(principal, "tenant-1");
        await tenantManager.CreateAsync(principal, "tenant-2");

        // Act
        List<Tenant> tenants = [];
        await foreach(var tenant in repo.EnumerateAsync(TestContext.Current.CancellationToken))
        {
            tenants.Add(tenant);
        }

        // Assert
        Assert.Equal(2, tenants.Count);
        Assert.Contains(tenants, x => x.Id == "tenant-1");
        Assert.Contains(tenants, x => x.Id == "tenant-2");
    }


    /// <summary>
    /// Verifies that EnumerateAsync returns all tenants with various counts
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(19)]
    [InlineData(20)]
    [InlineData(21)]
    [InlineData(100)]
    public async Task EnumerateAsync_ShouldReturnAllTenants_WithVariousCounts(int numberOfTenants)
    {
        // Arrange
        var principal = CreatePrincipal();
        ITenantRepository repo = new FakeTenantRepository();
        var tenantManager = new TenantManager(new FakeProjectRepository(), repo, _settingsProvider, TimeProvider.System);
        for (int i = 0; i < numberOfTenants; i++)
        {
            await tenantManager.CreateAsync(principal, "tenant-" + i);
        }

        // Act
        List<Tenant> tenants = [];
        await foreach (var tenant in repo.EnumerateAsync(TestContext.Current.CancellationToken))
        {
            tenants.Add(tenant);
        }

        // Assert
        Assert.Equal(numberOfTenants, tenants.Count);
    }
}
