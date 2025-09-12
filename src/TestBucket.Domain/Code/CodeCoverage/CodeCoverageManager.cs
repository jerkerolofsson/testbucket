using TestBucket.Domain.Code.CodeCoverage.Models;

namespace TestBucket.Domain.Code.CodeCoverage;
internal class CodeCoverageManager : ICodeCoverageManager
{
    private readonly ICodeCoverageRepository _repo;
    private readonly TimeProvider _timeProvider;
    private readonly ISettingsProvider _settingsProvider;
    private CodeCoverageSettings? _settings;

    public CodeCoverageManager(ISettingsProvider settingsProvider, ICodeCoverageRepository repo, TimeProvider timeProvider)
    {
        _settingsProvider = settingsProvider;
        _repo = repo;
        _timeProvider = timeProvider;
    }

    /// <inheritdoc/>
    public async Task<CodeCoverageSettings> LoadSettingsAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        _settings ??= await _settingsProvider.GetDomainSettingsAsync<CodeCoverageSettings>(tenantId, projectId);
        _settings ??= new();

        return _settings;
    }

    public async Task<CodeCoverageGroup?> GetCodeCoverageGroupAsync(ClaimsPrincipal user, long projectId, CodeCoverageGroupType groupType, string groupName)
    {
        var tenantId = user.GetTenantIdOrThrow();
        return await _repo.GetGroupAsync(tenantId, projectId, groupType, groupName);
    }   

    public async Task UpdateCodeCoverageGroupAsync(ClaimsPrincipal user, CodeCoverageGroup group)
    {
        user.ThrowIfEntityTenantIsDifferent(group);

        group.Modified = _timeProvider.GetUtcNow();
        group.ModifiedBy = user.Identity?.Name ?? throw new ArgumentException("ClaimsPrincipal is missing identity name");

        await _repo.UpdateGroupAsync(group);

    }
    public async Task<CodeCoverageGroup> GetOrCreateCodeCoverageGroupAsync(ClaimsPrincipal user, long projectId, CodeCoverageGroupType groupType, string groupName)
    {
        var userName = user.Identity?.Name ?? throw new ArgumentException("ClaimsPrincipal is missing identity name");
        var tenantId = user.GetTenantIdOrThrow();
        CodeCoverageGroup? group = await _repo.GetGroupAsync(tenantId, projectId, groupType, groupName);
        if(group is null)
        {
            group = new CodeCoverageGroup 
            { 
                TestProjectId = projectId,
                CreatedBy = userName,
                ModifiedBy = userName,
                Created = _timeProvider.GetUtcNow(),
                Modified = _timeProvider.GetUtcNow(),
                Group = groupType, 
                Name = groupName, 
                TenantId = tenantId 
            };
            await _repo.AddGroupAsync(group);
        }
        return group;
    }
}
