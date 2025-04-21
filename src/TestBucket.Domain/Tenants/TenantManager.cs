using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Identity;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;

namespace TestBucket.Domain.Tenants;
public class TenantManager : ITenantManager
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ISettingsProvider _settingsProvider;

    public TenantManager(ITenantRepository tenantRepository, ISettingsProvider settingsProvider)
    {
        _tenantRepository = tenantRepository;
        _settingsProvider = settingsProvider;
    }

    /// <summary>
    /// Generates a new api key for the tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public async Task UpdateTenantCiCdKeyAsync(string tenantId)
    {
        var tenant = await _tenantRepository.GetTenantByIdAsync(tenantId);
        if (tenant is null)
        {
            throw new ArgumentException("Tenant not found");
        }

        var settings = await _settingsProvider.LoadGlobalSettingsAsync();
        if (string.IsNullOrEmpty(settings.SymmetricJwtKey))
        {
            throw new InvalidDataException("No symmetric key has been configured");
        }
        if (string.IsNullOrEmpty(settings.JwtIssuer))
        {
            throw new InvalidDataException("No issuer has been configured");
        }
        if (string.IsNullOrEmpty(settings.JwtAudience))
        {
            throw new InvalidDataException("No audience has been configured");
        }

        var expires = DateTimeOffset.UtcNow.AddDays(365 * 5);

        var builder = new EntityPermissionBuilder();
        builder.Add(PermissionEntityType.Project, PermissionLevel.All);
        builder.Add(PermissionEntityType.User, PermissionLevel.All);
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
