
namespace TestBucket.Domain.AI.Settings
{
    class GithubModelsDeveloperKeySetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public GithubModelsDeveloperKeySetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            Metadata.Name = "Github Models API Key";
            Metadata.Description = "Personal Access Token for Github";
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcon.AI;
            Metadata.Section.Name = "Provider";
            Metadata.ShowDescription = true;
            Metadata.SearchText = "github-models";
            Metadata.Type = FieldType.String;
            Metadata.WriteOnly = true;
            Metadata.AccessLevel = Identity.Models.AccessLevel.SuperAdmin;
        }

        public override async Task<FieldValue> ReadAsync(SettingContext principal)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();
            return new FieldValue { StringValue = settings.GithubModelsDeveloperKey, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext principal, FieldValue value)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();

            if (settings.AiProvider != value.StringValue)
            {
                settings.GithubModelsDeveloperKey = value.StringValue;
                await _settingsProvider.SaveGlobalSettingsAsync(settings);
            }
        }
    }
}
