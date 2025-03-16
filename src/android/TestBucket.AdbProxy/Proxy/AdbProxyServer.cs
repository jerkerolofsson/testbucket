using System.Net.Sockets;
using System.Net;
using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using TestBucket.AdbProxy.Models;

namespace TestBucket.AdbProxy.Proxy
{
    public class AdbProxyServer : IDisposable, IConnectionObserver, ILocalIdProvider
    {
        private readonly TcpListener _tcpServer;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly AdbProxyOptions _adbProxyOptions;
        private readonly ILogger<AdbProxyServer> _logger;
        private bool _disposing = false;
        private int _nextLocalId = 1;

        private readonly Lock _lock = new();
        private readonly List<AdbProxyClientHandler> _connections = new();

        public AdbProxyServer(
            AdbDevice device,
            IOptions<AdbProxyOptions> options, ILogger<AdbProxyServer> logger, IPortGenerator portGenerator)
        {
            _adbProxyOptions = options.Value;

            Port = portGenerator.GetNextPort();
            _tcpServer = new TcpListener(_adbProxyOptions.ListenAddress, Port);
            _logger = logger;
            DeviceSerial = device.DeviceId;
            DeviceName = device.ModelInfo.Name ?? device.DeviceId;
            Device = device;

            // Port / URL
            Device.Port = Port;

            // If running a docker container, use environment variable
            string? hostname = Environment.GetEnvironmentVariable("ADB_PROXY_PUBLIC_IP");
            if (hostname is null)
            {
                foreach (var address in Dns.GetHostAddresses(Dns.GetHostName(), AddressFamily.InterNetwork))
                {
                    if (!IPAddress.IsLoopback(address))
                    {
                        hostname = address.ToString();
                        break;
                    }
                }
            }

            Device.Url = $"{hostname??"localhost"}:{Port}";
        }

        /// <summary>
        /// The ADB device
        /// </summary>
        public AdbDevice Device { get; }

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Device ID
        /// </summary>
        public string DeviceSerial { get; }

        /// <summary>
        /// Device Mame
        /// </summary>
        public string DeviceName { get; }

        /// <summary>
        /// Starts the server, waiting for clients to connect (adb connect)
        /// </summary>
        /// <param name="cancellationToken"></param>
        public void Start(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                await AcceptClientAsync(cancellationToken);
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        /// <summary>
        /// Accepts a client and starts a handler for the client
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task AcceptClientAsync(CancellationToken cancellationToken)
        {
            try
            {
                _tcpServer.Start();
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    // Someone connected to us
                    var source = await _tcpServer.AcceptTcpClientAsync(_cancellationTokenSource.Token);

                    _logger.LogInformation("Client connected: {RemoteEndPoint}", source.Client.RemoteEndPoint);

                    var connection = new AdbProxyClientHandler(_logger, _adbProxyOptions, source, this, this, DeviceSerial, DeviceName, _cancellationTokenSource.Token);
                    lock (_lock)
                    {
                        _connections.Add(connection);
                    }
                }
            }
            catch (OperationCanceledException) { }
        }

        public void Dispose()
        {
            if (!_disposing)
            {
                _disposing = true;

                _cancellationTokenSource.Cancel();

                _tcpServer.Stop();
                _tcpServer.Dispose();

                foreach (var connection in _connections)
                {
                    connection.Dispose();
                }
            }
        }

        public void OnConnectionException(AdbProxyClientHandler connection, Exception exception)
        {
            _logger.LogWarning("Client disconnected: {RemoteEndPoint}", connection.RemoteEndPoint);
            lock (_lock)
            {
                _connections.Remove(connection);
            }
            connection.Dispose();
        }

        public void OnClose(AdbProxyClientHandler connection)
        {
            _logger.LogWarning("Client closed: {RemoteEndPoint}", connection.RemoteEndPoint);
            lock (_lock)
            {
                _connections.Remove(connection);
            }
            connection.Dispose();
        }

        public int IncrementAndGetLocalId()
        {
            var localId = _nextLocalId;
            _nextLocalId++;
            if (_nextLocalId == int.MaxValue)
            {
                _nextLocalId = 1;
            }
            return localId;
        }
    }
}
