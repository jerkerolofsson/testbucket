
namespace TestBucket.Domain.AI.Settings
{
    class AzureAiProductionKeySetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public AzureAiProductionKeySetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            Metadata.Name = "Azure AI API Key";
            Metadata.Description = "Production Key for Azure AI";
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcon.AI;
            Metadata.WriteOnly = true;
            Metadata.Section.Name = "Provider";
            Metadata.SearchText = "azure-ai";
            Metadata.ShowDescription = true;
            Metadata.Type = FieldType.String;
            Metadata.AccessLevel = Identity.Models.AccessLevel.SuperAdmin;

        }

        public override async Task<FieldValue> ReadAsync(SettingContext principal)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();
            return new FieldValue { StringValue = settings.AzureAiProductionKey, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext principal, FieldValue value)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();

            if (settings.AiProvider != value.StringValue)
            {
                settings.AzureAiProductionKey = value.StringValue;
                await _settingsProvider.SaveGlobalSettingsAsync(settings);
            }
        }
    }
}
