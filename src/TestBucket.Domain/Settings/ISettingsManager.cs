using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Settings.Models;

namespace TestBucket.Domain.Settings;
public interface ISettingsManager
{
    /// <summary>
    /// Gets all categories
    /// </summary>
    public SettingsCategory[] Categories { get; }

    /// <summary>
    /// Searches for text
    /// </summary>
    /// <param name="context"></param>
    /// <param name="searchPhrase"></param>
    /// <returns></returns>
    ISetting[] Search(SettingContext context, string searchPhrase);

    /// <summary>
    /// Gets all settings
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    ISetting[] GetSettings(SettingContext context);

    /// <summary>
    /// Searches for links
    /// </summary>
    /// <param name="context"></param>
    /// <param name="searchPhrase"></param>
    /// <returns></returns>
    List<SettingsLink> SearchLinks(SettingContext context, string searchPhrase);
}
