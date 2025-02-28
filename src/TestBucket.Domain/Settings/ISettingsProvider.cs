using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Settings.Models;

namespace TestBucket.Domain.Settings;
public interface ISettingsProvider
{
    Task<GlobalSettings> LoadGlobalSettingsAsync();
    Task SaveGlobalSettingsAsync(GlobalSettings settings);
}
