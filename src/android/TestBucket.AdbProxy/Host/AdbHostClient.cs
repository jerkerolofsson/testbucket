using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using CliWrap;

using TestBucket.AdbProxy.Models;
using TestBucket.AdbProxy.Proxy;

namespace TestBucket.AdbProxy.Host;

/// <summary>
/// Implements communication with the adb host
/// </summary>
public class AdbHostClient
{
    private readonly AdbProxyOptions _adbProxyOptions;

    public AdbHostClient(AdbProxyOptions adbProxyOptions)
    {
        _adbProxyOptions = adbProxyOptions;
    }

    /// <summary>
    /// Creates a stream
    /// </summary>
    /// <param name="commands"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AdbStream> CreateStreamAsync(uint localId, uint remoteId, string[] commands, CancellationToken cancellationToken)
    {
        var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync("127.0.0.1", _adbProxyOptions.DestinationPort, cancellationToken);

        var stream = new AdbStream(tcpClient, localId, remoteId);
        foreach (var command in commands)
        {
            await stream.SendCommandAsync(command, cancellationToken);
        }
        return stream;
    }

    /// <summary>
    /// "adb devices"
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AdbDevice[]> ListDevicesAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await ListDevicesInternalAsync(cancellationToken);
        }
        catch(SocketException)
        {
            // adb daemon is not running
            await StartAdbDaemonAsync(cancellationToken);
            return await ListDevicesInternalAsync(cancellationToken);
        }
    }

    private async Task StartAdbDaemonAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Cli.Wrap("adb")
                .WithArguments("start-server")
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }
        catch { }
    }

    private async Task<AdbDevice[]> ListDevicesInternalAsync(CancellationToken cancellationToken)
    {
        uint localId = 1;
        uint remoteId = 1;
        using var adbStream = await CreateStreamAsync(localId, remoteId, ["host:devices"], cancellationToken);

        var length = await adbStream.ReadHex4Async();

        var text = await adbStream.ReadToEndAsync(cancellationToken);
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var devices = new List<AdbDevice>(lines.Length); 

        foreach(var line in lines)
        {
            var items = line.Split('\t', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var device = new AdbDevice() { DeviceId = items[0] };
            devices.Add(device);
            if(items.Length > 1)
            {
                device.Status = items[1];
            }
        }
        return devices.ToArray();
    }

}
