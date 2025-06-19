using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestLab;
internal class TestLabManager : ITestLabManager
{
    private readonly TimeProvider _timeProvider;
    private readonly ITestCaseRepository _testCaseRepository;
    private readonly List<ITestLabObserver> _observers = new();

    public TestLabManager(
        TimeProvider timeProvider,
        ITestCaseRepository testCaseRepository)
    {
        _timeProvider = timeProvider;
        _testCaseRepository = testCaseRepository;
    }

    public async Task AddFolderAsync(ClaimsPrincipal principal, TestLabFolder folder)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Write);
        ArgumentNullException.ThrowIfNull(folder.TestProjectId);

        folder.TenantId = principal.GetTenantIdOrThrow();
        folder.Modified = folder.Created = _timeProvider.GetUtcNow();
        folder.CreatedBy = folder.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");

        await _testCaseRepository.AddTestLabFolderAsync(folder);

        foreach (var observer in _observers)
        {
            await observer.OnTestLabFolderCreatedAsync(folder);
        }
    }

    public async Task<IReadOnlyList<TestLabFolder>> GetFoldersAsync(ClaimsPrincipal principal, long projectId, long parentId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Read);
        return await _testCaseRepository.GetChildTestLabFoldersAsync(projectId, parentId);
    }

    public async Task<IReadOnlyList<TestLabFolder>> GetRootFoldersAsync(ClaimsPrincipal principal, long projectId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Read);
        return await _testCaseRepository.GetRootTestLabFoldersAsync(projectId);
    }
    public async Task UpdateFolderAsync(ClaimsPrincipal principal, TestLabFolder folder)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Write);

        var existingFolder = await _testCaseRepository.GetTestLabFolderByIdAsync(folder.Id);

        folder.Modified = _timeProvider.GetUtcNow();
        folder.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");

        await _testCaseRepository.UpdateTestLabFolderAsync(folder);

        if (existingFolder?.ParentId != folder.ParentId)
        {
            foreach (var observer in _observers)
            {
                await observer.OnTestLabFolderMovedAsync(folder);
            }
        }

        foreach (var observer in _observers)
        {
            await observer.OnTestLabFolderSavedAsync(folder);
        }
    }

    public async Task DeleteFolderAsync(ClaimsPrincipal principal, long folderId)
    { 
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Delete);

        TestLabFolder? folder = await GetFolderByIdAsync(principal, folderId);
        if (folder is not null)
        {
            await _testCaseRepository.DeleteTestLabFolderAsync(folder);

            foreach (var observer in _observers)
            {
                await observer.OnTestLabFolderDeletedAsync(folder);
            }
        }
    }
    private async Task<TestLabFolder?> GetFolderByIdAsync(ClaimsPrincipal principal, long folderId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Read);
        return await _testCaseRepository.GetTestLabFolderByIdAsync(folderId);
    }

    public void AddObserver(ITestLabObserver observer) => _observers.Add(observer);

    public void RemoveObserver(ITestLabObserver observer) => _observers.Remove(observer);   
}
