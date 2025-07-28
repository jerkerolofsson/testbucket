using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

using Microsoft.Extensions.Logging;

using TestBucket.AdbProxy.Host;
using TestBucket.AdbProxy.Protocol;

namespace TestBucket.AdbProxy.Proxy
{
    /// <summary>
    /// Manages one specific device, simulating an ADB device but is proxying commands to the adb host using "smartsockets"
    /// 
    /// Communication with the client over TCP is performed using the ADB protocol.
    /// Communication with the ADB host is performed using the ADB smartsockets protocol.
    /// 
    /// See https://android.googlesource.com/platform/system/core/+/dd7bc3319deb2b77c5d07a51b7d6cd7e11b5beb0/adb/protocol.txt
    /// 
    /// [adb connect ip:port] triggers:
    /// 1. A connection handshake
    /// 
    /// [adb -s ip:port shell getprop] triggers an OPEN request with payload "shell:getprop"
    /// 
    /// We take the "shell:getprop" and opens a connection to the ADB host, and sends two commands:
    /// 1. host:device:device-id
    /// 2. shell:getprop
    /// 
    /// After that raw TCP communication is proxied between the client (adb.exe) and the adb host running locally
    /// 
    /// </summary>
    public class AdbProxyClientHandler : IDisposable, IAdbStreamObserver
    {
        private readonly EndPoint? _remoteEndPoint;
        private readonly ILogger _logger;
        private readonly AdbProxyOptions _options;
        private readonly TcpClient _client;
        private readonly ILocalIdProvider _localIdProvider;
        private readonly NetworkStream _clientStream;
        private readonly IConnectionObserver _observer;
        private readonly string _deviceSerial;
        private readonly string _deviceName;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _readTask;
        private bool _disposed;
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1);
        //private uint _remoteId;
        //private uint _localId;

        private readonly ManualResetEventSlim _isReady = new ManualResetEventSlim(false);

        private uint _version = AdbProtocolConstants.ProtocolVersion;
        private uint _maxDataLength = AdbProtocolConstants.MaxDataLength;

        /// <summary>
        /// State
        /// </summary>
        public AdbProxyClientState State { get; private set; } = AdbProxyClientState.Initial;

        /// <summary>
        /// Connection to adb host, created when OPEN command is received
        /// Key is the remote id
        /// </summary>
        private readonly ConcurrentDictionary<uint, AdbStream> _adbStreams = new();


        public EndPoint? RemoteEndPoint => _remoteEndPoint;

        public AdbProxyClientHandler(
            ILogger logger,
            AdbProxyOptions options,
            TcpClient source, ILocalIdProvider localIdProvider, IConnectionObserver observer,
            string deviceSerial, string deviceName, CancellationToken cancellationToken)
        {
            _remoteEndPoint = source.Client.RemoteEndPoint;
            _logger = logger;
            _options = options;
            _client = source;
            _localIdProvider = localIdProvider;
            _clientStream = source.GetStream();
            _observer = observer;
            _deviceSerial = deviceSerial;
            _deviceName = deviceName;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _readTask = Task.Factory.StartNew(RunProxyLoopAsync, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private async Task RunProxyLoopAsync()
        {
            await RunProxyLoopAsync("client=>proxy");
        }

        /// <summary>
        /// Reads from the client (e.g. adb shell...)
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private async Task RunProxyLoopAsync(string tag)
        {
            try
            {
                var message = new AdbProtocolMessageBuilder();

                var cancellationToken = _cancellationTokenSource.Token;
                var buffer = new byte[32 * 1024];
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        int bytesToRead = AdbProtocolConstants.HeaderLength;
                        if (message.HasReadHeader)
                        {
                            bytesToRead = buffer.Length;
                        }

                        // Read from the adb client (e.g. remote PC)
                        int readBytes = await _clientStream.ReadAsync(buffer, 0, bytesToRead, cancellationToken);
                        if (readBytes > 0)
                        {
                            _logger.LogInformation("{tag} Read {readBytes }b ", tag, readBytes);
                            message.Append(buffer, readBytes);

                            if (message.HasReadHeader && message.HasReadPayload)
                            {
                                LogMessageHeaderReceived(tag, message);

                                await HandleCompleteMessageFromClientAsync(message, cancellationToken);

                                // Reset the message and start to build a new one
                                message = new();
                            }
                        }
                        else
                        {
                            _observer.OnConnectionException(this, new Exception("No bytes read"));
                            if (!_clientStream.Socket.Connected)
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _observer.OnConnectionException(this, ex);
                    }
                }
            }
            catch (OperationCanceledException) { }
        }

        private async Task HandleCompleteMessageFromClientAsync(AdbProtocolMessageBuilder message, CancellationToken cancellationToken = default)
        {
            if (State == AdbProxyClientState.Initial)
            {
                switch (message.Header.Command)
                {
                    case AdbProtocolConstants.A_CNXN:
                        await OnCnxnReceivedAsync(message, cancellationToken);
                        break;
                    case AdbProtocolConstants.A_OPEN:
                        _logger.LogWarning("OPEN not allowed at this time");
                        break;
                    case AdbProtocolConstants.A_SYNC:
                        _logger.LogWarning("SYNC not allowed at this time");
                        break;
                    case AdbProtocolConstants.A_WRTE:
                        _logger.LogWarning("WRTE not allowed at this time");
                        break;
                    case AdbProtocolConstants.A_CLSE:
                        _logger.LogWarning("CLSE not allowed at this time");
                        break;
                    default:
                        _logger.LogWarning($"Unknown command: {message.Header.Command}");
                        break;
                }
            }
            else if (State == AdbProxyClientState.Connected)
            {
                switch (message.Header.Command)
                {
                    case AdbProtocolConstants.A_CNXN:
                        await OnCnxnReceivedAsync(message, cancellationToken);
                        break;
                    case AdbProtocolConstants.A_OPEN:
                        try
                        {
                            await OnOpenReceivedAsync(message, cancellationToken);
                        }
                        catch
                        {
                            await OnOpenReceivedAsync(message, cancellationToken);
                        }
                        break;
                    case AdbProtocolConstants.A_SYNC:
                        break;
                    case AdbProtocolConstants.A_WRTE:
                        await OnWriteReceivedAsync(message, cancellationToken);
                        break;
                    case AdbProtocolConstants.A_OKAY:
                        _logger.LogInformation("OKAY");
                        SetReadyState();
                        break;
                    case AdbProtocolConstants.A_CLSE:
                        await OnCloseReceivedAsync(message, cancellationToken);
                        break;
                    default:
                        _logger.LogWarning($"Unknown command: {message.Header.Command}");
                        break;
                }
            }
        }

        private AdbStream? GetAdbStreamFromRemoteId(uint remoteId)
        {
            _adbStreams.TryGetValue(remoteId, out var stream);
            return stream;
        }
        private async Task OnCloseReceivedAsync(AdbProtocolMessageBuilder message, CancellationToken cancellationToken = default)
        {
            uint remoteId = message.Header.Arg1;

            var adbStream = GetAdbStreamFromRemoteId(remoteId);
            if (adbStream is not null)
            {
                await CloseStreamAsync(adbStream);
            }

            // The recipient should not respond to a CLOSE message in any way.  The
            // recipient should cancel pending WRITEs or CLOSEs, but this is not a
            // requirement, since they will be ignored.
        }

        private async Task OnWriteReceivedAsync(AdbProtocolMessageBuilder message, CancellationToken cancellationToken = default)
        {
            if (message.Payload is null)
            {
                _logger.LogWarning("Empty WRITE message received");
                return;
            }

            uint remoteId = message.Header.Arg0;

            // A WRITE message containing a remote-id which does not map to an open
            // stream on the recipient's side is ignored.  The stream may have been
            // closed while this message was in-flight.
            var adbStream = GetAdbStreamFromRemoteId(remoteId);
            if (adbStream is not null)
            {
                var stream = adbStream.GetStream();
                await message.WriteToAsync(stream, _writeLock, cancellationToken);
              
                // Send OKAY to client
                await SendReadyAsync(adbStream, cancellationToken);
            }
        }

        /// <summary>
        /// Opens a new stream. Each stream is represented by IDs, so we need to pass these back and forth when sending and receiving messages.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task OnOpenReceivedAsync(AdbProtocolMessageBuilder message, CancellationToken cancellationToken = default)
        {
            // shell:getprop\0
            var path = message.DecodePayloadAsString();
            if (path is null)
            {
                throw new InvalidOperationException("OPEN received with no payload");
            }

            var remoteId = message.Header.Arg0;
            var localId = (uint)_localIdProvider.IncrementAndGetLocalId();

            var host = new AdbHostClient(_options);
            var adbStream = await host.CreateStreamAsync(localId, remoteId, [$"host:transport:{_deviceSerial}", path], cancellationToken);
            _adbStreams[remoteId] = adbStream;


            // Create reader that will read data in a background thread from the ADB host and send it to us via a callback
            adbStream.CreateReader(this, cancellationToken);

            // reply with OKAY message to client
            await SendReadyAsync(adbStream, cancellationToken);

            // initial ready state is true after open
            SetReadyState();
        }

        private async Task SendReadyAsync(AdbStream adbStream, CancellationToken cancellationToken)
        {
            var ready = AdbProtocolMessageBuilder.Create(AdbProtocolConstants.A_OKAY, adbStream.LocalId, adbStream.RemoteId, []);
            await ready.WriteToAsync(_clientStream, _writeLock, cancellationToken);
        }

        private void SetReadyState()
        {
            _isReady.Set();
        }

        private async Task OnCnxnReceivedAsync(AdbProtocolMessageBuilder message, CancellationToken cancellationToken = default)
        {
            // "host::features=shell_v2,cmd,stat_v2,ls_v2,fixed_push_mkdir,apex,abb,fixed_push_symlink_timestamp,abb_exec,remount_shell,track_app,sendrecv_v2,sendrecv_v2_brotli,sendrecv_v2_lz4,sendrecv_v2_zstd,sendrecv_v2_dry_run_send"
            var remoteSystemId = message.DecodePayloadAsString();

            var version = message.Header.Arg0;
            var maxDataLength = message.Header.Arg1;

            _version = Math.Min(version, AdbProtocolConstants.ProtocolVersion);
            _maxDataLength = Math.Min(maxDataLength, AdbProtocolConstants.MaxDataLength);

            var payload = $"device:proxied-{_deviceSerial}:{_deviceName}";

            var packet = AdbProtocolMessageBuilder.Create(AdbProtocolConstants.A_CNXN, _version, _maxDataLength, payload);
            _logger.LogInformation("< CNXN");
            await packet.WriteToAsync(_clientStream, _writeLock, cancellationToken);

            State = AdbProxyClientState.Connected;
        }

        private void LogMessageHeaderReceived(string tag, AdbProtocolMessageBuilder message)
        {
            switch (message.Header.Command)
            {
                case AdbProtocolConstants.A_CNXN:
                    _logger.LogInformation("{tag} CNXN", tag);
                    break;
                case AdbProtocolConstants.A_OPEN:
                    _logger.LogInformation("{tag} OPEN", tag);

                    //var msg = message.DecodePayloadAsString();
                    //_logger.LogInformation("{tag}: {msg}", tag, msg);

                    break;
                case AdbProtocolConstants.A_SYNC:
                    _logger.LogInformation("{tag} SYNC", tag);
                    break;
                case AdbProtocolConstants.A_WRTE:
                    _logger.LogInformation("{tag} WRTE", tag);
                    break;
                case AdbProtocolConstants.A_CLSE:
                    _logger.LogInformation("{tag} CLSE", tag);
                    break;
                case AdbProtocolConstants.A_OKAY:
                    _logger.LogInformation("{tag} OKAY", tag);
                    break;
                default:
                    _logger.LogWarning("{tag} Unknown command: {cmd}", tag, message.Header.Command.ToString("x8"));
                    break;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _cancellationTokenSource.Cancel();

                _client.Dispose();

                foreach (var adbStream in _adbStreams.Values)
                {
                    adbStream.Dispose();
                }
            }
        }

        public async Task OnDataReceivedAsync(AdbStream adbStream, Memory<byte> data)
        {
            Debug.Assert(adbStream != null);

            if (data.Length == 0)
            {
                await CloseStreamAsync(adbStream);

                adbStream.Dispose();

                // Remove from adb stream collection
                _adbStreams.TryRemove(adbStream.RemoteId, out var _);
                return;
            }
            var bytes = data.ToArray();

            // Write to client
            if (data.Length <= _maxDataLength)
            {
                var packet = AdbProtocolMessageBuilder.Create(AdbProtocolConstants.A_WRTE, adbStream.LocalId, adbStream.RemoteId, bytes);
                await packet.WriteToAsync(_clientStream, _writeLock, _cancellationTokenSource.Token);

                //await SendReadyAsync(adbStream, _cancellationTokenSource.Token);
                //WaitForReadyState();
            }
            else
            {
                // The packet size is larger than the allowed payload size, so we need to fragment the response
                int offset = 0;
                int remainingBytes = data.Length;
                while (remainingBytes > 0)
                {
                    int length = Math.Min((int)_maxDataLength, remainingBytes);
                    remainingBytes -= length;
                    offset += length;

                    var fragment = new Memory<byte>(bytes, offset, length);
                    var packet = AdbProtocolMessageBuilder.Create(AdbProtocolConstants.A_WRTE, adbStream.LocalId, adbStream.RemoteId, fragment.ToArray());
                    await packet.WriteToAsync(_clientStream, _writeLock, _cancellationTokenSource.Token);
                    //WaitForReadyState();
                }
            }
        }

        private void WaitForReadyState()
        {
            // Wait for ready state.
            // Initial state is ready.

            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, timeout.Token);

            _isReady.Wait(cts.Token);

            // After a write the ready state is reset.
            _isReady.Reset();
        }

        public async Task OnAdbHostExceptionAsync(AdbStream adbStream, Exception exception)
        {
            await CloseStreamAsync(adbStream);

        }

        private async Task CloseStreamAsync(AdbStream adbStream)
        {
            var closePacket = AdbProtocolMessageBuilder.Create(AdbProtocolConstants.A_CLSE, adbStream.LocalId, adbStream.RemoteId, []);
            await closePacket.WriteToAsync(_clientStream, _writeLock, _cancellationTokenSource.Token);
        }
    }
}
