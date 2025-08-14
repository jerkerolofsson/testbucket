

namespace TestBucket.Domain.AI.Settings.LLM
{
    class AiLlmModelUsdPerMillionOutputTokensSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public AiLlmModelUsdPerMillionOutputTokensSetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            Metadata.Name = "ai-default-model-price-per-1M-tokens-output";
            Metadata.Description = "ai-default-model-price-per-1M-tokens-description";
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcons.AI;
            Metadata.Section.Name = "Models";
            Metadata.SearchText = "ai-models-shared anthropic open-ai";
            Metadata.ShowDescription = true;
            Metadata.Type = FieldType.Double;
            Metadata.AccessLevel = Identity.Models.AccessLevel.Admin;
        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            context.Principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Read);
            var settings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(context.Principal.GetTenantIdOrThrow(), null);
            settings ??= new();

            return new FieldValue { DoubleValue = settings.LlmModelUsdPerMillionOutputTokens, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            var settings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(context.Principal.GetTenantIdOrThrow(), null);
            settings ??= new();

            if (settings.LlmModelUsdPerMillionOutputTokens != value.DoubleValue)
            {
                settings.LlmModelUsdPerMillionOutputTokens = value.DoubleValue ?? 0;
                await _settingsProvider.SaveDomainSettingsAsync(context.Principal.GetTenantIdOrThrow(), null, settings);
            }
        }
    }
}
