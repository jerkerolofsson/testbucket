using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestLab;

namespace TestBucket.Domain.Testing.TestRepository;
internal class TestRepositoryManager : ITestRepositoryManager
{
    private readonly TimeProvider _timeProvider;
    private readonly ITestCaseRepository _testCaseRepository;
    private readonly List<ITestRepositoryObserver> _observers = new();

    public TestRepositoryManager(
        TimeProvider timeProvider,
        ITestCaseRepository testCaseRepository)
    {
        _timeProvider = timeProvider;
        _testCaseRepository = testCaseRepository;
    }

    public async Task AddFolderAsync(ClaimsPrincipal principal, TestRepositoryFolder folder)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Write);
        ArgumentNullException.ThrowIfNull(folder.TestProjectId);

        folder.TenantId = principal.GetTenantIdOrThrow();
        folder.Modified = folder.Created = _timeProvider.GetUtcNow();
        folder.CreatedBy = folder.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");

        await _testCaseRepository.AddTestRepositoryFolderAsync(folder);
        foreach (var observer in _observers)
        {
            await observer.OnTestRepositoryFolderCreatedAsync(folder);
        }

    }

    public async Task<IReadOnlyList<TestRepositoryFolder>> GetRootFoldersAsync(ClaimsPrincipal principal, long projectId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Read);
        return await _testCaseRepository.GetRootTestRepositoryFoldersAsync(projectId);
    }
    public async Task<IReadOnlyList<TestRepositoryFolder>> GetFoldersAsync(ClaimsPrincipal principal, long projectId, long parentId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Read);
        return await _testCaseRepository.GetChildTestRepositoryFoldersAsync(projectId, parentId);
    }

    public async Task UpdateFolderAsync(ClaimsPrincipal principal, TestRepositoryFolder folder)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Write);

        var existingFolder = await _testCaseRepository.GetTestRepositoryFolderByIdAsync(folder.Id);

        folder.Modified = _timeProvider.GetUtcNow();
        folder.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");

        await _testCaseRepository.UpdateTestRepositoryFolderAsync(folder);

        if(existingFolder?.ParentId != folder.ParentId)
        {
            foreach (var observer in _observers)
            {
                await observer.OnTestRepositoryFolderMovedAsync(folder);
            }
        }

        foreach(var observer in _observers)
        {
            await observer.OnTestRepositoryFolderSavedAsync(folder);
        }
    }

    public async Task DeleteFolderAsync(ClaimsPrincipal principal, long folderId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Delete);

        TestRepositoryFolder? folder = await GetFolderByIdAsync(principal, folderId);
        if (folder is not null)
        {
            await _testCaseRepository.DeleteTestRepositoryFolderAsync(folder);

            foreach (var observer in _observers)
            {
                await observer.OnTestRepositoryFolderDeletedAsync(folder);
            }
        }
    }

    private async Task<TestRepositoryFolder?> GetFolderByIdAsync(ClaimsPrincipal principal, long folderId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Read);
        return await _testCaseRepository.GetTestRepositoryFolderByIdAsync(folderId);
    }

    public void AddObserver(ITestRepositoryObserver observer) => _observers.Add(observer);

    public void RemoveObserver(ITestRepositoryObserver observer) => _observers.Remove(observer);
}
