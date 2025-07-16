namespace TestBucket.Domain.AI.Settings.LLM
{
    class GithubModelsDeveloperKeySetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public GithubModelsDeveloperKeySetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            Metadata.Name = "ai-github-pat";
            Metadata.Description = null;
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcon.AI;
            Metadata.Section.Name = "ai-provider";
            Metadata.ShowDescription = true;
            Metadata.SearchText = "github-models";
            Metadata.Type = FieldType.String;
            Metadata.WriteOnly = true;
            Metadata.AccessLevel = Identity.Models.AccessLevel.Admin;
        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            context.Principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Read);
            var settings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(context.Principal.GetTenantIdOrThrow(), null);
            settings ??= new();
            return new FieldValue { StringValue = settings.GithubModelsDeveloperKey, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            context.Principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Write);
            var settings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(context.Principal.GetTenantIdOrThrow(), null);
            settings ??= new();


            if (settings.AiProvider != value.StringValue)
            {
                settings.GithubModelsDeveloperKey = value.StringValue;
                await _settingsProvider.SaveDomainSettingsAsync(context.Principal.GetTenantIdOrThrow(), null, settings);
            }
        }
    }
}
