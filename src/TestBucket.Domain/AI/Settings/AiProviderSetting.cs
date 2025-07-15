namespace TestBucket.Domain.AI.Settings
{
    public class AiProviderSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public AiProviderSetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            Metadata.Name = "ai-provider";
            Metadata.Description = null;
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcon.AI;
            Metadata.Section.Name = "ai-provider";
            Metadata.Type = FieldType.SingleSelection;
            Metadata.ShowDescription = true;
            Metadata.AccessLevel = Identity.Models.AccessLevel.SuperAdmin;

            Metadata.Options = ["ollama", "github-models", "azure-ai", "anthropic"];
        }

        public override async Task<FieldValue> ReadAsync(SettingContext principal)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();
            return new FieldValue { StringValue = settings.AiProvider, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext principal, FieldValue value)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();

            if (settings.AiProvider != value.StringValue)
            {
                settings.AiProvider = value.StringValue ?? "ollama";

                switch(settings.AiProvider)
                {
                    case "ollama":
                        settings.AiProviderUrl = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_OLLAMA_BASE_URL) ?? "http://localhost:11434";
                        break;
                    case "anthropic":
                        settings.AiProviderUrl = "https://api.anthropic.com/v1";
                        break;
                    default:
                        settings.AiProviderUrl = "https://models.inference.ai.azure.com";
                        break;
                }
                await _settingsProvider.SaveGlobalSettingsAsync(settings);
            }
        }
    }
}
