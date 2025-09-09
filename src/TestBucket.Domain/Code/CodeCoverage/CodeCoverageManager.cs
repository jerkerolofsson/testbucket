using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Code.CodeCoverage.Models;

namespace TestBucket.Domain.Code.CodeCoverage;
internal class CodeCoverageManager : ICodeCoverageManager
{
    private readonly ISettingsProvider _settingsProvider;
    private CodeCoverageSettings? _settings;

    public CodeCoverageManager(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }

    public async Task<CodeCoverageSettings> LoadSettingsAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        _settings ??= await _settingsProvider.GetDomainSettingsAsync<CodeCoverageSettings>(tenantId, projectId);
        _settings ??= new();

        return _settings;
    }
}
