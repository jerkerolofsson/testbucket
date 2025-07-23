namespace TestBucket.Domain.AI.Settings.LLM
{
    class OpenAiApiKeySetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public OpenAiApiKeySetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            Metadata.Name = "openai-provider-key";
            Metadata.Description = null;
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcons.AI;
            Metadata.WriteOnly = true;
            Metadata.Section.Name = "ai-provider";
            Metadata.SearchText = "openai open-ai chatgpt";
            Metadata.ShowDescription = true;
            Metadata.Type = FieldType.String;
            Metadata.AccessLevel = Identity.Models.AccessLevel.Admin;
            Metadata.RequiredPermission = PermissionLevel.Write;

        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            var llmSettings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(context.Principal.GetTenantIdOrThrow(), null);
            llmSettings ??= new();

            return new FieldValue { StringValue = llmSettings.OpenAiApiKey, FieldDefinitionId = 0 };

        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            context.Principal.ThrowIfNoPermission(PermissionEntityType.Tenant, Metadata.RequiredPermission);

            var llmSettings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(context.Principal.GetTenantIdOrThrow(), null);
            llmSettings ??= new();
            llmSettings.OpenAiApiKey = value.StringValue;
            await _settingsProvider.SaveDomainSettingsAsync(context.Principal.GetTenantIdOrThrow(), null, llmSettings);
        }
    }
}
