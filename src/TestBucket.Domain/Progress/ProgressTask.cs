using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Progress;
public class ProgressTask : IAsyncDisposable
{
    private readonly IProgressManager _progressManager;
    private readonly CancellationTokenSource _cts = new();

    public string Title { get; private set; }
    public string Status { get; private set; }
    public bool Completed { get; private set; } = false;

    /// <summary>
    /// Percent complete, from 0 to 100
    /// </summary>
    public double Percent { get; private set; } = 0;

    internal ProgressTask(string title, IProgressManager progressManager)
    {
        Title = title;
        Status = "";
        _progressManager = progressManager;
    }

    public void Cancel() => _cts.Cancel();

    /// <summary>
    /// Returns true if the user has requested cancellation
    /// </summary>
    public bool IsCancellationRequested => _cts.Token.IsCancellationRequested;

    /// <summary>
    /// Returns a cancellation token
    /// </summary>
    public CancellationToken CancellationToken => _cts.Token;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="status"></param>
    /// <param name="percent">0.0-100.0</param>
    /// <returns></returns>
    public async Task ReportStatusAsync(string status, double percent)
    {
        this.Status = status;
        this.Percent = percent;
        await _progressManager.NotifyAsync(this);
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Dispose();
        this.Percent = 100;
        this.Completed = true;
        await _progressManager.NotifyAsync(this);
    }
}
