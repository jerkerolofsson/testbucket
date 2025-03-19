using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Settings.Models;

namespace TestBucket.Domain.Settings;
public interface ISettingsProvider
{
    /// <summary>
    /// Loads global settings
    /// </summary>
    /// <returns></returns>
    Task<GlobalSettings> LoadGlobalSettingsAsync();

    /// <summary>
    /// Saves global settings
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    Task SaveGlobalSettingsAsync(GlobalSettings settings);
}
