
using TestBucket.Domain.Code.CodeCoverage;
using TestBucket.Domain.Code.CodeCoverage.Models;

namespace TestBucket.Components.Code.CodeCoverage.Controllers;

internal class CodeCoverageController : TenantBaseService
{
    private readonly ICodeCoverageManager _codeCoverageManager;
    public CodeCoverageController(AuthenticationStateProvider authenticationStateProvider, ICodeCoverageManager codeCoverageManager) : base(authenticationStateProvider)
    {
        _codeCoverageManager = codeCoverageManager;
    }

    public async Task<CodeCoverageSettings> LoadSettingsAsync(long projectId)
    {
        var user = await GetUserClaimsPrincipalAsync();
        return await _codeCoverageManager.LoadSettingsAsync(user, projectId);
    }
}
