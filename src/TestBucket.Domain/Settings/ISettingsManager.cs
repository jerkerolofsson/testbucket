using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Settings.Models;

namespace TestBucket.Domain.Settings;
public interface ISettingsManager
{
    public SettingsCategory[] Categories { get; }

    /// <summary>
    /// Returns all settings that can be changed
    /// </summary>
    public ISetting[] Settings { get; }

    ISetting[] Search(string text);
}
