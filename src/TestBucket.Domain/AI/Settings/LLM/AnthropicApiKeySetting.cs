namespace TestBucket.Domain.AI.Settings.LLM
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
            Metadata.Category.Icon = SettingIcons.AI;
            Metadata.WriteOnly = true;
            Metadata.Section.Name = "ai-provider";
            Metadata.SearchText = "anthropic claude";
            Metadata.ShowDescription = true;
            Metadata.Type = FieldType.String;
            Metadata.AccessLevel = Identity.Models.AccessLevel.Admin;
            Metadata.RequiredPermission = PermissionLevel.Write;

        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            var llmSettings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(context.Principal.GetTenantIdOrThrow(), null);
            llmSettings ??= new();


            return new FieldValue { StringValue = llmSettings.AnthropicApiKey, FieldDefinitionId = 0 };

        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            context.Principal.ThrowIfNoPermission(PermissionEntityType.Tenant, Metadata.RequiredPermission);

            var llmSettings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(context.Principal.GetTenantIdOrThrow(), null);
            llmSettings ??= new();
            llmSettings.AnthropicApiKey = value.StringValue;
            await _settingsProvider.SaveDomainSettingsAsync(context.Principal.GetTenantIdOrThrow(), null, llmSettings);

            /*
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();

            if (settings.AiProvider != value.StringValue)
            {
                settings.AnthropicApiKey = value.StringValue;
                await _settingsProvider.SaveGlobalSettingsAsync(settings);
            }*/
        }
    }
}
