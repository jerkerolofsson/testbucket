
namespace TestBucket.Domain.AI.Settings
{
    class AnthropicApiKeySetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public AnthropicApiKeySetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            Metadata.Name = "anthropic-provider-key";
            Metadata.Description = null;
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcon.AI;
            Metadata.WriteOnly = true;
            Metadata.Section.Name = "ai-provider";
            Metadata.SearchText = "anthropic claude";
            Metadata.ShowDescription = true;
            Metadata.Type = FieldType.String;
            Metadata.AccessLevel = Identity.Models.AccessLevel.SuperAdmin;

        }

        public override async Task<FieldValue> ReadAsync(SettingContext principal)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();
            return new FieldValue { StringValue = settings.AnthropicApiKey, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext principal, FieldValue value)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();

            if (settings.AiProvider != value.StringValue)
            {
                settings.AnthropicApiKey = value.StringValue;
                await _settingsProvider.SaveGlobalSettingsAsync(settings);
            }
        }
    }
}
