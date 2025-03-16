using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.AdbProxy.Models;

namespace TestBucket.AdbProxy.Inform;

public interface IDeviceInformer
{
    /// <summary>
    /// Informs another system about device changes
    /// </summary>
    /// <param name="devices"></param>
    /// <returns></returns>
    Task InformAsync(AdbDevice[] devices, CancellationToken cancellationToken);

    /// <summary>
    /// Loads settings
    /// </summary>
    /// <returns></returns>
    Task<InformSettings> LoadInformSettingsAsync();

    /// <summary>
    /// Saves settings
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    Task SaveInformSettingsAsync(InformSettings settings);
}
