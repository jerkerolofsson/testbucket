using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Progress;
public interface IProgressManager
{
    void AddObserver(IProgressObserver observer);
    void RemoveObserver(IProgressObserver observer);

    /// <summary>
    /// Creates a reported that can be used to notify observers about the progress for a specific task
    /// </summary>
    /// <returns></returns>
    ProgressTask CreateProgressTask(string title);
    Task NotifyAsync(ProgressTask progressTask);
}
