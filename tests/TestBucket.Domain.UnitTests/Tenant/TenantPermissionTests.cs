using NSubstitute;
using System.Security.Claims;
using TestBucket.Contracts;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Tenants;

namespace TestBucket.Domain.UnitTests.Tenant
{
    /// <summary>
    /// Unit tests for <see cref="TenantManager"/> permission enforcement.
    /// Verifies that operations throw <see cref="UnauthorizedAccessException"/> when the principal lacks required permissions.
    /// </summary>
    [Component("Tenant")]
    [UnitTest]
    [SecurityTest]
    [EnrichedTest]
    public class TenantManagerPermissionTests
    {
        private readonly IProjectRepository _projectRepo = Substitute.For<IProjectRepository>();
        private readonly ITenantRepository _tenantRepo = Substitute.For<ITenantRepository>();
        private readonly ISettingsProvider _settingsProvider = Substitute.For<ISettingsProvider>();

        /// <summary>
        /// Creates a new instance of <see cref="TenantManager"/> using mocked dependencies.
        /// </summary>
        /// <returns>A <see cref="TenantManager"/> instance.</returns>
        private TenantManager CreateManager() =>
            new TenantManager(_projectRepo, _tenantRepo, _settingsProvider);

        /// <summary>
        /// Creates a <see cref="ClaimsPrincipal"/> with optional permission configuration.
        /// </summary>
        /// <param name="configure">Optional action to configure permissions using <see cref="EntityPermissionBuilder"/>.</param>
        /// <returns>A <see cref="ClaimsPrincipal"/> with the specified permissions.</returns>
        private ClaimsPrincipal CreatePrincipal(Action<EntityPermissionBuilder>? configure = null)
        {
            var builder = new EntityPermissionBuilder();
            configure?.Invoke(builder);
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(PermissionClaims.Permissions, PermissionClaimSerializer.Serialize(builder.Build())));
            return new ClaimsPrincipal(identity);
        }

        /// <summary>
        /// Verifies that <see cref="TenantManager.CreateAsync"/> throws <see cref="UnauthorizedAccessException"/> when the principal lacks write permission.
        /// </summary>
        [Fact]
        public async Task CreateAsync_WithoutWritePermission_Throws()
        {
            var manager = CreateManager();
            var principal = CreatePrincipal(b => b.Add(PermissionEntityType.Tenant, PermissionLevel.Read));
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => manager.CreateAsync(principal, "test"));
        }

        /// <summary>
        /// Verifies that <see cref="TenantManager.DeleteAsync"/> throws <see cref="UnauthorizedAccessException"/> when the principal lacks delete permission.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WithoutDeletePermission_Throws()
        {
            var manager = CreateManager();
            var principal = CreatePrincipal(b => b.Add(PermissionEntityType.Tenant, PermissionLevel.ReadWrite));
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => manager.DeleteAsync(principal, "tenant1", CancellationToken.None));
        }

        /// <summary>
        /// Verifies that <see cref="TenantManager.ExistsAsync"/> DOES NOT throw UnauthorizedAccessException when the principal lacks read permission.
        /// </summary>
        [Fact]
        public async Task ExistsAsync_WithoutReadPermission_Throws()
        {
            var manager = CreateManager();
            var principal = CreatePrincipal(); // No permissions
            await manager.ExistsAsync(principal, "tenant1");
        }

        /// <summary>
        /// Verifies that <see cref="TenantManager.GetTenantByIdAsync"/> throws <see cref="UnauthorizedAccessException"/> when the principal lacks read permission.
        /// </summary>
        [Fact]
        public async Task GetTenantByIdAsync_WithoutReadPermission_Throws()
        {
            var manager = CreateManager();
            var principal = CreatePrincipal(); // No permissions
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => manager.GetTenantByIdAsync(principal, "tenant1"));
        }

        /// <summary>
        /// Verifies that <see cref="TenantManager.SearchAsync"/> throws <see cref="UnauthorizedAccessException"/> when the principal lacks read permission.
        /// </summary>
        [Fact]
        public async Task SearchAsync_WithoutReadPermission_Throws()
        {
            var manager = CreateManager();
            var principal = CreatePrincipal(); // No permissions
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => manager.SearchAsync(principal, new SearchQuery()));
        }
    }
}