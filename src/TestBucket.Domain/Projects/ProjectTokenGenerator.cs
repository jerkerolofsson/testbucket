using System.Security.Claims;

using TestBucket.Contracts.Identity;
using TestBucket.Domain.Identity.Permissions;

namespace TestBucket.Domain.Projects;
internal class ProjectTokenGenerator : IProjectTokenGenerator
{
    private readonly ISettingsProvider _settingsProvider;

    public ProjectTokenGenerator(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }

    public async Task<string> GenerateCiCdAccessTokenAsync(string tenantId, long projectId, long? testRunId, long? testSuiteId)
    {
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

        var expires = DateTimeOffset.UtcNow.AddDays(3);

        // Note: The name claim type must match the authenticator, e.g. ApiKeyAuthenticator

        var builder = new EntityPermissionBuilder();
        builder.Add(PermissionEntityType.TestSuite, PermissionLevel.ReadWrite);
        builder.Add(PermissionEntityType.TestCase, PermissionLevel.ReadWrite);
        builder.Add(PermissionEntityType.TestCaseRun, PermissionLevel.ReadWrite);
        builder.Add(PermissionEntityType.TestRun, PermissionLevel.ReadWrite);
        builder.Add(PermissionEntityType.TestAccount, PermissionLevel.Read);
        builder.Add(PermissionEntityType.TestResource, PermissionLevel.Read);
        builder.Add(PermissionEntityType.Project, PermissionLevel.Read);
        builder.Add(PermissionEntityType.User, PermissionLevel.Read);
        builder.Add(PermissionEntityType.Team, PermissionLevel.Read);
        builder.Add(PermissionEntityType.Tenant, PermissionLevel.None);
        builder.Add(PermissionEntityType.Requirement, PermissionLevel.Read);
        builder.Add(PermissionEntityType.RequirementSpecification, PermissionLevel.Read);

        var nameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        var username = $"urn:tb-project-id:{projectId.ToString()}";
        List<Claim> claims = [
            new Claim("tenant", tenantId),
            new Claim(nameClaimType, username),
            new Claim("project", projectId.ToString()),
            new Claim("userid", projectId.ToString()),
            new Claim("email", username),
            new Claim(PermissionClaims.Permissions, PermissionClaimSerializer.Serialize(builder.Build()))
            ];

        if(testRunId is not null)
        {
            claims.Add(new Claim("run", testRunId.Value.ToString()));
        }

        if (testSuiteId is not null)
        {
            claims.Add(new Claim("testsuite", testSuiteId.Value.ToString()));
        }

        var identity = new ClaimsIdentity(claims:claims, authenticationType: "testbucket", nameType: nameClaimType, roleType: null);
        var principal = new ClaimsPrincipal(identity);

        var generator = new ApiKeyGenerator(settings.SymmetricJwtKey, settings.JwtIssuer, settings.JwtAudience);
        return generator.GenerateAccessToken(principal, expires.DateTime);
    }
}
