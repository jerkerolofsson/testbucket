using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestLab;
public interface ITestLabObserver
{
    Task OnTestLabFolderCreatedAsync(TestLabFolder folder);
    Task OnTestLabFolderSavedAsync(TestLabFolder folder);
    Task OnTestLabFolderMovedAsync(TestLabFolder folder);
    Task OnTestLabFolderDeletedAsync(TestLabFolder folder);
}
