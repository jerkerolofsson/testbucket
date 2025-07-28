using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Host;

/// <summary>
/// This is a stream from the proxy to a service on the device, via the host.
/// A client can have multiple streams open, identified with "remote id".
/// 
/// AdbProxyClientHandler will create a new AdbStream when OPEN is received.
/// </summary>
public class AdbStream : IDisposable
{
    private readonly TcpClient _tcpClient;
    private bool _disposedValue;
    private AdbStreamReader? _reader;
    private const string OKAY = "OKAY";
    private const string FAIL = "FAIL";
    public uint LocalId { get; }
    public uint RemoteId { get; }

    internal AdbStream(TcpClient tcpClient, uint localId, uint remoteId)
    {
        LocalId = localId;
        RemoteId = remoteId;
        _tcpClient = tcpClient;
    }

    /// <summary>
    /// Returns a stream
    /// </summary>
    /// <returns></returns>
    internal Stream GetStream() => _tcpClient.GetStream();

    /// <summary>
    /// Reads all remaining data from the stream as a string and returns it
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal async Task<string> ReadToEndAsync(CancellationToken cancellationToken)
    {
        using var textReader = new StreamReader(GetStream());
        var text = await textReader.ReadToEndAsync();
        return text;
    }

    /// <summary>
    /// Reads all remaining data from the stream as a string and returns it
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal async Task<byte[]> ReadByteArrayAsync(CancellationToken cancellationToken)
    {
        var dest = new MemoryStream();
        var source = GetStream();
        await source.CopyToAsync(dest);

        return dest.ToArray();
    }

    public virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _tcpClient.Dispose();

                _reader?.Dispose();
            }

            _disposedValue = true;
        }
        _reader = null;
    }

    internal async Task SendCommandAsync(string command, CancellationToken cancellationToken)
    {
        var bytes = Encoding.UTF8.GetBytes(command);

        var stream = _tcpClient.GetStream();

        int length = bytes.Length;
        var lengthHex = length.ToString("x4");
        var lengthBytes = Encoding.UTF8.GetBytes(lengthHex);

        await stream.WriteAsync(lengthBytes, cancellationToken);
        await stream.WriteAsync(bytes, cancellationToken);
        await stream.FlushAsync(cancellationToken);

        var responseCode = await ReadResponseCodeAsync(stream, cancellationToken);
        if (responseCode == FAIL)
        {
            var errorMessage = await ReadErrorAsync(stream, cancellationToken);
            throw new InvalidOperationException(errorMessage);
        }
        else if (responseCode != OKAY)
        {
            throw new InvalidOperationException($"Unknown response code from ADB host: {responseCode}");
        }
    }

    private async Task<string> ReadErrorAsync(NetworkStream stream, CancellationToken cancellationToken)
    {
        var sizeBytes = new byte[4];
        await stream.ReadExactlyAsync(sizeBytes);

        var size = (int)BinaryPrimitives.ReadUInt32LittleEndian(sizeBytes);
        if(size > 1_000_000)
        {
            throw new Exception($"Overflow reading error from stream, size was reported as {size} which is too large");
        }

        var messageBytes = new byte[size];
        await stream.ReadExactlyAsync(messageBytes);
        return Encoding.UTF8.GetString(messageBytes);
    }

    internal async Task<string> ReadResponseCodeAsync(Stream stream, CancellationToken cancellationToken)
    {
        var bytes = new byte[4];
        await stream.ReadExactlyAsync(bytes, cancellationToken);
        return Encoding.UTF8.GetString(bytes);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    internal AdbStreamReader? CreateReader(IAdbStreamObserver callback, CancellationToken cancellationToken)
    {
        _reader = new AdbStreamReader(callback, this, cancellationToken);
        _reader.Start();

        return _reader;
    }

    internal async Task<int> ReadHex4Async(CancellationToken cancellationToken = default)
    {
        var response = await ReadResponseCodeAsync(GetStream(), cancellationToken);
        var bytes = Convert.FromHexString(response);
        return bytes[0] << 8 | bytes[1];
    }
}
