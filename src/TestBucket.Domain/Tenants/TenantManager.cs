using TestBucket.Contracts.Identity;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Domain.Tenants;
public class TenantManager : ITenantManager
{
    public const string TenantNotFoundExceptionMessage = "Tenant not found";
    public const string SymmetricKeyConfigurationError = "No symmetric key has been configured";
    public const string JwtConfigurationError = "Invalid JWT configuration";
    public const int CiCdAccessTokenExpiryDays = 365 * 5;
    private readonly IProjectRepository _projectRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ISettingsProvider _settingsProvider;
    private readonly TimeProvider _timeProvider;

    public TenantManager(
        IProjectRepository projectRepository,
        ITenantRepository tenantRepository,
        ISettingsProvider settingsProvider,
        TimeProvider timeProvider)
    {
        _projectRepository = projectRepository;
        _tenantRepository = tenantRepository;
        _settingsProvider = settingsProvider;
        _timeProvider = timeProvider;
    }

    public async Task<OneOf<Tenant, AlreadyExistsError>> CreateAsync(ClaimsPrincipal principal, string name)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Write);

        if (string.IsNullOrWhiteSpace(name))
        {
            return new AlreadyExistsError();
        }
        var tenantId = new Slugify.SlugHelper().GenerateSlug(name);
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return new AlreadyExistsError();
        }
        var existingTenant = await _tenantRepository.GetTenantByIdAsync(tenantId);
        if(existingTenant is not null)
        {
            return new AlreadyExistsError();
        }

        var result = await _tenantRepository.CreateAsync(name, tenantId);

        // Generate key for the tenant
        await UpdateTenantCiCdKeyAsync(tenantId);

        return await _tenantRepository.GetTenantByIdAsync(tenantId) ?? throw new Exception("Failed to create tenant");
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(ClaimsPrincipal principal, string tenantId)
    {
        // Should not have any permissions here as the user is possibly not signed in
        //principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Read);
        return await _tenantRepository.ExistsAsync(tenantId);
    }

    /// <inheritdoc/>
    public async Task<Tenant?> GetTenantByIdAsync(ClaimsPrincipal principal, string tenantId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Read);
        return await _tenantRepository.GetTenantByIdAsync(tenantId);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(ClaimsPrincipal principal, string tenantId, CancellationToken cancellationToken)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Delete);

        await foreach(var project in _projectRepository.EnumerateAsync(tenantId, cancellationToken))
        {
            await _projectRepository.DeleteProjectAsync(project);
        }

        await _tenantRepository.DeleteTenantAsync(tenantId);
    }
    /// <inheritdoc/>
    public async Task<PagedResult<Tenant>> SearchAsync(ClaimsPrincipal principal, SearchQuery query)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Read);
        return await _tenantRepository.SearchAsync(query);
    }
  
    public async Task UpdateTenantCiCdKeyAsync(string tenantId)
    {
        var tenant = await _tenantRepository.GetTenantByIdAsync(tenantId);
        if (tenant is null)
        {
            throw new ArgumentException(TenantNotFoundExceptionMessage);
        }

        var settings = await _settingsProvider.LoadGlobalSettingsAsync();
        if (string.IsNullOrEmpty(settings.SymmetricJwtKey))
        {
            throw new InvalidDataException(SymmetricKeyConfigurationError);
        }
        if (string.IsNullOrEmpty(settings.JwtIssuer))
        {
            throw new InvalidDataException(JwtConfigurationError);
        }
        if (string.IsNullOrEmpty(settings.JwtAudience))
        {
            throw new InvalidDataException(JwtConfigurationError);
        }

        var expires = _timeProvider.GetUtcNow().AddDays(CiCdAccessTokenExpiryDays);

        var builder = new EntityPermissionBuilder();
        builder.Add(PermissionEntityType.Project, PermissionLevel.All);
        builder.Add(PermissionEntityType.User, PermissionLevel.All);
        builder.Add(PermissionEntityType.Team, PermissionLevel.All);
        builder.Add(PermissionEntityType.Tenant, PermissionLevel.All);
        builder.Add(PermissionEntityType.TestSuite, PermissionLevel.All);
        builder.Add(PermissionEntityType.TestCase, PermissionLevel.All);
        builder.Add(PermissionEntityType.TestRun, PermissionLevel.All);
        builder.Add(PermissionEntityType.TestCaseRun, PermissionLevel.All);
        builder.Add(PermissionEntityType.TestAccount, PermissionLevel.All);
        builder.Add(PermissionEntityType.TestResource, PermissionLevel.All);
        builder.Add(PermissionEntityType.Requirement, PermissionLevel.All);
        builder.Add(PermissionEntityType.RequirementSpecification, PermissionLevel.All);

        Claim[] claims = [
            new Claim("tenant", tenantId),
            new Claim("userid", tenantId),
            new Claim("email", $"tenant:{tenantId}"),
            new Claim(PermissionClaims.Permissions, PermissionClaimSerializer.Serialize(builder.Build())),
            ];
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        var generator = new ApiKeyGenerator(settings.SymmetricJwtKey, settings.JwtIssuer, settings.JwtAudience);
        tenant.CiCdAccessToken = generator.GenerateAccessToken(principal, expires.DateTime);
        tenant.CiCdAccessTokenExpires = expires;

        await _tenantRepository.UpdateTenantAsync(tenant);
    }
}
