namespace TestBucket.Domain.AI.Settings
{
    public class EmbeddingAiProviderSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public EmbeddingAiProviderSetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            Metadata.Name = "embedding-ai-provider";
            Metadata.Description = null;
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcon.AI;
            Metadata.Section.Name = "ai-provider";
            Metadata.Type = FieldType.SingleSelection;
            Metadata.ShowDescription = true;
            Metadata.AccessLevel = Identity.Models.AccessLevel.SuperAdmin;

            Metadata.Options = ["ollama"];
        }

        public override async Task<FieldValue> ReadAsync(SettingContext principal)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();
            return new FieldValue { StringValue = settings.EmbeddingAiProvider, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext principal, FieldValue value)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();

            if (settings.EmbeddingAiProvider != value.StringValue)
            {
                settings.EmbeddingAiProvider = value.StringValue ?? "ollama";

                switch(settings.EmbeddingAiProvider)
                {
                    case "ollama":
                        settings.EmbeddingAiProviderUrl = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_OLLAMA_BASE_URL) ?? "http://localhost:11434";
                        break;
                }
                await _settingsProvider.SaveGlobalSettingsAsync(settings);
            }
        }
    }
}
