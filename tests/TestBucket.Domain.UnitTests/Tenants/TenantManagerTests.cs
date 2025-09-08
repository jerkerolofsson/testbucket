using System.Security.Claims;

using TestBucket.Contracts;
using TestBucket.Domain.Errors;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Projects.Models;
using TestBucket.Domain.Settings.Fakes;
using TestBucket.Domain.Tenants;
using TestBucket.Domain.Tenants.Models;
using TestBucket.Domain.UnitTests.Project;

namespace TestBucket.Domain.UnitTests.Tenants;

/// <summary>
/// Unit tests for the <see cref="TenantManager"/> class.
/// </summary>
[Component("Tenant")]
[UnitTest]
[FunctionalTest]
[EnrichedTest]
public class TenantManagerTests
{
    private readonly FakeSettingsProvider _settingsProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantManagerTests"/> class.
    /// </summary>
    public TenantManagerTests()
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
    /// Tests that <see cref="TenantManager.CreateAsync"/> returns an error when the tenant name is blank
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldReturnError_WhenTenantNameIsBlank()
    {
        // Arrange
        var principal = CreatePrincipal();
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), _settingsProvider, TimeProvider.System);

        // Act
        var result = await tenantManager.CreateAsync(principal, "");

        // Assert
        Assert.True(result.IsT1);
    }

    /// <summary>
    /// Tests that <see cref="TenantManager.CreateAsync"/> returns an error when the tenant name already exists.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldReturnError_WhenTenantNameAlreadyExists()
    {
        // Arrange
        var principal = CreatePrincipal();
        var tenantName = "DuplicateTenant";
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), _settingsProvider, TimeProvider.System);

        // Create the first tenant
        await tenantManager.CreateAsync(principal, tenantName);

        // Act
        var result = await tenantManager.CreateAsync(principal, tenantName);

        // Assert
        Assert.True(result.IsT1);
    }

    /// <summary>
    /// Tests that <see cref="TenantManager.CreateAsync"/> returns an error when the tenant slug already exists.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldReturnError_WhenTenantSlugAlreadyExists()
    {
        // Arrange
        var principal = CreatePrincipal();
        var tenantName1 = "Tenant One";
        var tenantName2 = "Tenant-One"; // Generates the same slug as "Tenant One"
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), _settingsProvider, TimeProvider.System);

        // Create the first tenant
        await tenantManager.CreateAsync(principal, tenantName1);

        // Act
        var result = await tenantManager.CreateAsync(principal, tenantName2);

        // Assert
        Assert.True(result.IsT1);
    }

    /// <summary>
    /// Tests that <see cref="TenantManager.CreateAsync"/> creates a tenant when valid input is provided.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldCreateTenant_WhenValidInput()
    {
        // Arrange
        var principal = CreatePrincipal();
        var tenantName = "TestTenant";
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), _settingsProvider, TimeProvider.System);

        // Act
        var result = await tenantManager.CreateAsync(principal, tenantName);

        // Assert
        Assert.IsType<Tenant>(result.AsT0);
        Assert.Equal(tenantName, result.AsT0.Name);
    }

    /// <summary>
    /// Tests that <see cref="TenantManager.ExistsAsync"/> returns true when the tenant exists.
    /// </summary>
    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenTenantExists()
    {
        // Arrange
        var tenantId = "tenant-id";
        var principal = CreatePrincipal();
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), _settingsProvider, TimeProvider.System);
        await tenantManager.CreateAsync(principal, tenantId);

        // Act
        var exists = await tenantManager.ExistsAsync(principal, tenantId);

        // Assert
        Assert.True(exists);
    }

    /// <summary>
    /// Tests that <see cref="TenantManager.GetTenantByIdAsync"/> returns the tenant when it exists.
    /// </summary>
    [Fact]
    public async Task GetTenantByIdAsync_ShouldReturnTenant_WhenTenantExists()
    {
        // Arrange
        var tenantId = "tenant-id";
        var principal = CreatePrincipal();
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), _settingsProvider, TimeProvider.System);
        await tenantManager.CreateAsync(principal, tenantId);

        // Act
        var tenant = await tenantManager.GetTenantByIdAsync(principal, tenantId);

        // Assert
        Assert.NotNull(tenant);
        Assert.Equal(tenantId, tenant?.Id);
    }

    /// <summary>
    /// Tests that <see cref="TenantManager.SearchAsync"/> returns a paged result when the query is valid.
    /// </summary>
    [Fact]
    public async Task SearchAsync_ShouldReturnPagedResult_WhenQueryIsValid()
    {
        // Arrange
        var principal = CreatePrincipal();
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), _settingsProvider, TimeProvider.System);
        var query = new SearchQuery { Offset = 0, Count = 10 };

        // Act
        var result = await tenantManager.SearchAsync(principal, query);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResult<Tenant>>(result);
    }

    /// <summary>
    /// Tests that TenantManager.UpdateTenantCiCdKeyAsync updates the CI/CD key when the tenant exists.
    /// </summary>
    [Fact]
    public async Task UpdateTenantCiCdKeyAsync_ShouldUpdateKey_WhenTenantExists()
    {
        // Arrange
        var tenantId = "tenant-id";
        var principal = CreatePrincipal();
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), _settingsProvider, TimeProvider.System);
        await tenantManager.CreateAsync(principal, tenantId);

        // Act
        await tenantManager.UpdateTenantCiCdKeyAsync(tenantId);
        var tenant = await tenantManager.GetTenantByIdAsync(principal, tenantId);

        // Assert
        Assert.NotNull(tenant?.CiCdAccessToken);
        Assert.NotNull(tenant?.CiCdAccessTokenExpires);
    }

    /// <summary>
    /// Tests that <see cref="TenantManager.UpdateTenantCiCdKeyAsync"/> throws an ArgumentException when the tenant ID is unknown.
    /// </summary>
    [Fact]
    public async Task UpdateTenantCiCdKeyAsync_ShouldThrowArgumentException_WhenTenantIdIsUnknown()
    {
        // Arrange
        var unknownTenantId = "unknown-tenant-id";
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), _settingsProvider, TimeProvider.System);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => tenantManager.UpdateTenantCiCdKeyAsync(unknownTenantId));
        Assert.Equal(TenantManager.TenantNotFoundExceptionMessage, exception.Message);
    }

    /// <summary>
    /// Tests that TenantManager.CreateAsync throws an InvalidDataException if the symmetric JWT key is not configured.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentException_WhenSymmetricJwtKeyIsNotSpecified()
    {
        // Arrange
        var tenantId = "tenant-id";
        var principal = CreatePrincipal();
        var settingsProvider = new FakeSettingsProvider();
        var settings = await settingsProvider.LoadGlobalSettingsAsync();
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), settingsProvider, TimeProvider.System);
        settings.SymmetricJwtKey = null;
        await settingsProvider.SaveGlobalSettingsAsync(settings);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidDataException>(() => tenantManager.CreateAsync(principal, tenantId));
        Assert.Equal(TenantManager.SymmetricKeyConfigurationError, exception.Message);
    }

    /// <summary>
    /// Tests that TenantManager.UpdateTenantCiCdKeyAsync throws an InvalidDataException when the symmetric JWT key is not configured.
    /// </summary>
    [Fact]
    public async Task UpdateTenantCiCdKeyAsync_ShouldThrowArgumentException_WhenSymmetricJwtKeyIsNotSpecified()
    {
        // Arrange
        var tenantId = "tenant-id";
        var principal = CreatePrincipal();
        var settingsProvider = new FakeSettingsProvider();
        var settings = await settingsProvider.LoadGlobalSettingsAsync();
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), settingsProvider, TimeProvider.System);
        await tenantManager.CreateAsync(principal, tenantId);

        // Remove symmetric key
        settings.SymmetricJwtKey = null;
        await settingsProvider.SaveGlobalSettingsAsync(settings);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidDataException>(() => tenantManager.UpdateTenantCiCdKeyAsync(tenantId));
        Assert.Equal(TenantManager.SymmetricKeyConfigurationError, exception.Message);
    }

    /// <summary>
    /// Tests that TenantManager.UpdateTenantCiCdKeyAsync throws an InvalidDataException when the JWT issuer is not configured.
    /// </summary>
    [Fact]
    public async Task UpdateTenantCiCdKeyAsync_ShouldThrowArgumentException_WhenJwtIssIsNotSpecified()
    {
        // Arrange
        var tenantId = "tenant-id";
        var principal = CreatePrincipal();
        var settingsProvider = new FakeSettingsProvider();
        var settings = await settingsProvider.LoadGlobalSettingsAsync();
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), settingsProvider, TimeProvider.System);
        await tenantManager.CreateAsync(principal, tenantId);

        // Clear the issuer
        settings.JwtIssuer = null;
        await settingsProvider.SaveGlobalSettingsAsync(settings);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidDataException>(() => tenantManager.UpdateTenantCiCdKeyAsync(tenantId));
        Assert.Equal(TenantManager.JwtConfigurationError, exception.Message);
    }

    /// <summary>
    /// Tests that TenantManager.UpdateTenantCiCdKeyAsync throws an InvalidDataException when the JWT audience is not configured.
    /// </summary>
    [Fact]
    public async Task UpdateTenantCiCdKeyAsync_ShouldThrowArgumentException_WhenJwtAudIsNotSpecified()
    {
        // Arrange
        var tenantId = "tenant-id";
        var principal = CreatePrincipal();
        var settingsProvider = new FakeSettingsProvider();
        var settings = await settingsProvider.LoadGlobalSettingsAsync();
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), settingsProvider, TimeProvider.System);
        await tenantManager.CreateAsync(principal, tenantId);

        // Remove JWT audience
        settings.JwtAudience = null;
        await settingsProvider.SaveGlobalSettingsAsync(settings);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidDataException>(() => tenantManager.UpdateTenantCiCdKeyAsync(tenantId));
        Assert.Equal(TenantManager.JwtConfigurationError, exception.Message);
    }


    /// <summary>
    /// Tests that <see cref="TenantManager.DeleteAsync"/> deletes a project when the project's tenant matches the deleted tenant.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldDeleteProject_WhenProjectTenantMatchesTheDeletedTenant()
    {
        // Arrange
        var tenantId = "tenant-id";
        var principal = CreatePrincipal();
        var projectRepo = new FakeProjectRepository();
        var project = new TestProject
        {
            Name = "Test Project",
            Slug = "test-project",
            TenantId = tenantId,
            ShortName = "TP"
        };
        await projectRepo.AddAsync(project);
        var tenantManager = new TenantManager(projectRepo, new FakeTenantRepository(), _settingsProvider, TimeProvider.System);
        await tenantManager.CreateAsync(principal, tenantId);

        // Act
        await tenantManager.DeleteAsync(principal, tenantId, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(await projectRepo.GetBySlugAsync(tenantId, "test-project"));
    }


    /// <summary>
    /// Tests that <see cref="TenantManager.DeleteAsync"/> does not delete a project belonging to a different tenant.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldNotDeleteProject_WhenProjectTenantDoesNotMatchesTheDeletedTenant()
    {
        // Arrange
        var tenantId = "tenant-id";
        var principal = CreatePrincipal();
        var projectRepo = new FakeProjectRepository();
        var projectTenant = "some-other-tenant";
        var project = new TestProject
        {
            Name = "Test Project",
            Slug = "test-project",
            TenantId = projectTenant,
            ShortName = "TP"
        };
        await projectRepo.AddAsync(project);
        var tenantManager = new TenantManager(projectRepo, new FakeTenantRepository(), _settingsProvider, TimeProvider.System);
        await tenantManager.CreateAsync(principal, tenantId);

        // Act
        await tenantManager.DeleteAsync(principal, tenantId, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(await projectRepo.GetBySlugAsync(projectTenant, "test-project"));
    }


    /// <summary>
    /// Tests that <see cref="TenantManager.UpdateTenantCiCdKeyAsync"/> updates the CiCdAccessTokenExpires property.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldUpdateCiCdAccessTokenExpire()
    {
        // Arrange
        var tenantId = "tenant-id";
        var principal = CreatePrincipal();
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 8, 9, 23, 59, 59, TimeSpan.Zero));
        var expectedExpiration = timeProvider.GetUtcNow().AddDays(TenantManager.CiCdAccessTokenExpiryDays);
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), _settingsProvider, timeProvider);

        // Act
        await tenantManager.CreateAsync(principal, tenantId);
        var tenantAfterCreate = await tenantManager.GetTenantByIdAsync(principal, tenantId);

        // Assert
        Assert.NotNull(tenantAfterCreate?.CiCdAccessTokenExpires);
        Assert.Equal(expectedExpiration, tenantAfterCreate.CiCdAccessTokenExpires);
    }

    /// <summary>
    /// Tests that <see cref="TenantManager.UpdateTenantCiCdKeyAsync"/> updates the CiCdAccessTokenExpires property.
    /// </summary>
    [Fact]
    public async Task UpdateTenantCiCdKeyAsync_ShouldUpdateCiCdAccessTokenExpires_WhenInvoked()
    {
        // Arrange
        var tenantId = "tenant-id";
        var principal = CreatePrincipal();
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025,8,9, 23,59,59, TimeSpan.Zero));
        var expectedExpiration = timeProvider.GetUtcNow().AddDays(TenantManager.CiCdAccessTokenExpiryDays);
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), _settingsProvider, timeProvider);
        await tenantManager.CreateAsync(principal, tenantId);

        // Act
        var tenantBeforeUpdate = await tenantManager.GetTenantByIdAsync(principal, tenantId);
        var originalExpiration = tenantBeforeUpdate?.CiCdAccessTokenExpires;

        await tenantManager.UpdateTenantCiCdKeyAsync(tenantId);
        var tenantAfterUpdate = await tenantManager.GetTenantByIdAsync(principal, tenantId);

        // Assert
        Assert.NotNull(tenantAfterUpdate?.CiCdAccessTokenExpires);
        Assert.Equal(expectedExpiration, tenantAfterUpdate.CiCdAccessTokenExpires);
    }
}