namespace TestBucket.Domain.AI.Settings.LLM
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
            Metadata.Category.Icon = SettingIcons.AI;
            Metadata.Section.Name = "ai-provider";
            Metadata.Type = FieldType.SingleSelection;
            Metadata.ShowDescription = true;
            Metadata.AccessLevel = Identity.Models.AccessLevel.Admin;

            Metadata.Options = ["ollama"];
        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            context.Principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Read);
            var settings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(context.Principal.GetTenantIdOrThrow(), null);
            settings ??= new();

            return new FieldValue { StringValue = settings.EmbeddingAiProvider, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            context.Principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Write);
            var settings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(context.Principal.GetTenantIdOrThrow(), null);
            settings ??= new();

            if (settings.EmbeddingAiProvider != value.StringValue)
            {
                settings.EmbeddingAiProvider = value.StringValue ?? "ollama";

                switch(settings.EmbeddingAiProvider)
                {
                    case "ollama":
                        settings.EmbeddingAiProviderUrl = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_OLLAMA_BASE_URL) ?? "http://localhost:11434";
                        break;
                }
                await _settingsProvider.SaveDomainSettingsAsync(context.Principal.GetTenantIdOrThrow(), null, settings);
            }
        }
    }
}
