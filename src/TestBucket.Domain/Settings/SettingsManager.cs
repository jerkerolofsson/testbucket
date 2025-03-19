using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Settings.Models;

namespace TestBucket.Domain.Settings
{
    class SettingsManager : ISettingsManager
    {

        public SettingsCategory[] Categories { get; set; }

        public ISetting[] Settings { get; set; }

        public SettingsManager(IEnumerable<ISetting> settings)
        {
            Settings = settings.ToArray();
            Categories = settings.Select(x => x.Metadata.Category).Distinct().ToArray();
        }

        public ISetting[] Search(string text)
        {
            if(string.IsNullOrWhiteSpace(text))
            {
                return Settings;
            }
            else
            {
                text = text.ToLower();
                return Settings
                    .Where(x =>
                        x.Metadata.Name.ToLower().Contains(text) ||
                        (x.Metadata.Description is not null && x.Metadata.Description.ToLower().Contains(text)))
                    .ToArray();
            }
        }
    }
}
