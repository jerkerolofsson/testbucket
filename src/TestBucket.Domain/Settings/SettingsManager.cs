using System.Globalization;

using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Settings
{
    class SettingsManager : ISettingsManager
    {

        public SettingsCategory[] Categories { get; set; }

        private readonly IAppLocalization _localization;
        private readonly ISetting[] _settings;
        private readonly List<SettingsLink> _links = new List<SettingsLink>();

        public SettingsManager(IEnumerable<ISetting> settings, IEnumerable<SettingsLink> links, IAppLocalization localization)
        {
            _links = links.ToList();
            _settings = settings.ToArray();
            Categories = settings.Select(x=>x.Metadata.Category).Distinct().ToArray();
            _localization = localization;

            // Localize it
            foreach (var link in _links)
            {
                link.Title = _localization.Settings[link.Title];
                if (link.Description is not null)
                {
                    link.Description = _localization.Settings[link.Description];
                }
            }
            foreach(var category in Categories)
            {
                category.Name = _localization.Settings[category.Name];
            }
            var a = CultureInfo.CurrentCulture;
            var b = CultureInfo.CurrentUICulture;
            foreach(var setting in _settings)
            {
                setting.Metadata.Category.Name = _localization.Settings[setting.Metadata.Category.Name];
                setting.Metadata.Section.Name = _localization.Settings[setting.Metadata.Section.Name];
                setting.Metadata.Name = _localization.Settings[setting.Metadata.Name];
                if (setting.Metadata.Description is not null)
                {
                    setting.Metadata.Description = _localization.Settings[setting.Metadata.Description];
                }
            }
        }

        public ISetting[] Search(SettingContext context, string text)
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
                        x.Metadata.SearchText.ToLower().Contains(text) ||
                        x.Metadata.Name.ToLower().Contains(text) ||
                        x.Metadata.Category.Name.ToLower().Contains(text) ||
                        (x.Metadata.Description is not null && x.Metadata.Description.ToLower().Contains(text)))
                    .ToArray();
            }
        }

        public ISetting[] GetSettings(SettingContext context)
        {
            List<ISetting> filtered = new List<ISetting>();

            foreach(var setting in _settings)
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

        public List<SettingsLink> SearchLinks(SettingContext context, string searchPhrase)
        {
            if(searchPhrase is null)
            {
                return _links.ToList();
            }
            return _links.Where(x =>
                x.Title.Contains(searchPhrase, StringComparison.InvariantCultureIgnoreCase) ||
                x.Description?.Contains(searchPhrase, StringComparison.InvariantCultureIgnoreCase) == true ||
                x.Keywords?.Contains(searchPhrase, StringComparison.InvariantCultureIgnoreCase) == true).ToList();
        }
    }
}
