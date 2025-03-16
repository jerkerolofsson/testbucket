using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.AdbProxy.Models;

namespace TestBucket.AdbProxy.DeviceHandling;
public interface IAdbDeviceRepository
{
    IReadOnlyList<AdbDevice> Devices { get; }

    /// <summary>
    /// Scans for device changes and starts/stops proxies for connected devices
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateAsync(CancellationToken cancellationToken);
}
