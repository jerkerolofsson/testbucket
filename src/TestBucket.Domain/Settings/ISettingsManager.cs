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

    ISetting[] Search(SettingContext context, string text);

    ISetting[] GetSettings(SettingContext context);
    List<SettingsLink> SearchLinks(SettingContext context, string searchPhrase);
}
