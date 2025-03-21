using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Identity.Models;
using TestBucket.Domain.Settings.Models;

namespace TestBucket.Domain.Settings
{
    class SettingsManager : ISettingsManager
    {

        public SettingsCategory[] Categories { get; set; }

        private ISetting[] Settings { get; set; }

        public SettingsManager(IEnumerable<ISetting> settings)
        {
            Settings = settings.ToArray();
            Categories = settings.Select(x => x.Metadata.Category).Distinct().ToArray();
        }

        public ISetting[] Search( SettingContext context, string text)
        {
            if(string.IsNullOrWhiteSpace(text))
            {
                return GetSettings(context);
            }
            else
            {
                text = text.ToLower();
                return GetSettings(context)
                    .Where(x =>
                        x.Metadata.Name.ToLower().Contains(text) ||
                        (x.Metadata.Description is not null && x.Metadata.Description.ToLower().Contains(text)))
                    .ToArray();
            }
        }

        public ISetting[] GetSettings(SettingContext context)
        {
            List<ISetting> filtered = new List<ISetting>();

            foreach(var setting in Settings)
            {
                switch(setting.Metadata.AccessLevel)
                {
                    case AccessLevel.User:
                        filtered.Add(setting);
                        break;
                    case AccessLevel.Admin:
                        if (context.Principal.IsInRole("ADMIN"))
                        {
                            filtered.Add(setting);
                        }
                        break;
                    case AccessLevel.SuperAdmin:
                        if (context.Principal.IsInRole("SUPERADMIN"))
                        {
                            filtered.Add(setting);
                        }
                        break;
                }
            }

            return filtered.ToArray();
        }
    }
}
