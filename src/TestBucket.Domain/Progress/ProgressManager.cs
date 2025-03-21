namespace TestBucket.Domain.Progress;
public class ProgressManager : IProgressManager
{
    private readonly List<IProgressObserver> _observers = new List<IProgressObserver>();
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

    public void AddObserver(IProgressObserver observer)
    {
        _semaphore.Wait();
        try
        {
            _observers.Add(observer);
        }
        finally
        {
            _semaphore.Release();
        }
    }
    public void RemoveObserver(IProgressObserver observer)
    {
        _semaphore.Wait();
        try
        {
            _observers.Remove(observer);
        }
        finally
        {
            _semaphore.Release();
        }
    }


    public ProgressTask CreateProgressTask(string title)
    {
        return new ProgressTask(title, this);
    }

    internal async Task NotifyAsync(ProgressTask progressTask)
    {
        await _semaphore.WaitAsync();
        try
        {
            foreach (var observer in _observers)
            {
                await observer.NotifyAsync(progressTask);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
