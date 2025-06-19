using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestRepository;
public interface ITestRepositoryObserver
{
    Task OnTestRepositoryFolderCreatedAsync(TestRepositoryFolder folder);
    Task OnTestRepositoryFolderSavedAsync(TestRepositoryFolder folder);
    Task OnTestRepositoryFolderMovedAsync(TestRepositoryFolder folder);
    Task OnTestRepositoryFolderDeletedAsync(TestRepositoryFolder folder);
}
