namespace TestBucket.Domain.Automation.Runners.Jobs
{
    /// <summary>
    /// Singleton implementation of a lock
    /// </summary>
    public class GetJobLock
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        public Task WaitAsync(CancellationToken cancellationToken) => _semaphore.WaitAsync(cancellationToken);
        public void Release()
        {
            _semaphore.Release();
        }
    }

}
