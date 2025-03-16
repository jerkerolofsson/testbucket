using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.AdbProxy.Protocol;

namespace TestBucket.AdbProxy.Host;

/// <summary>
/// Reads from ADB host
/// </summary>
internal class AdbStreamReader : IDisposable
{
    private readonly IAdbStreamObserver _callback;
    private readonly AdbStream _adbStream;
    private readonly Stream _hostStream;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private Task? _task;

    public AdbStreamReader(IAdbStreamObserver callback, AdbStream adbStream, CancellationToken cancellationToken)
    {
        _callback = callback;
        _adbStream = adbStream;
        _hostStream = adbStream.GetStream();
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    }

    public void Start()
    {
        _task = Task.Factory.StartNew(ReadLoopAsync, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    private async Task ReadLoopAsync()
    {
        var token = _cancellationTokenSource.Token;
        try
        {
            var buffer = new byte[AdbProtocolConstants.MaxDataLength];
            while (!token.IsCancellationRequested)
            {
                int readbytes = await _hostStream.ReadAsync(buffer, 0, buffer.Length, token);
              
                string debug = Encoding.UTF8.GetString(buffer, 0, readbytes);

                await _callback.OnDataReceivedAsync(_adbStream, new Memory<byte>(buffer, 0, readbytes));
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            // Notify
            try
            {
                await _callback.OnAdbHostExceptionAsync(_adbStream, ex);
            } catch { }
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}
