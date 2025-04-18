using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Automation.Runners.Jobs
{
    public class JobAddedEventSignal
    {
        private volatile TaskCompletionSource<bool> m_tcs = new TaskCompletionSource<bool>();

        public Task WaitAsync() { return m_tcs.Task; }

        public Task WaitAsync(TimeSpan timeout)
        {
            Task eventTask = WaitAsync();
            Task delayTask = Task.Delay(timeout);
            return Task.WhenAny(eventTask, delayTask);
        }

        public void Set()
        {
            var tcs = m_tcs;
            Task.Factory.StartNew(s => ((TaskCompletionSource<bool>)s!).TrySetResult(true),
                tcs, CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default);
            tcs.Task.Wait();
        }

        public void Reset()
        {
            while (true)
            {
                var tcs = m_tcs;
                if (!tcs.Task.IsCompleted ||
                    Interlocked.CompareExchange(ref m_tcs, new TaskCompletionSource<bool>(), tcs) == tcs)
                    return;
            }
        }
    }
}
