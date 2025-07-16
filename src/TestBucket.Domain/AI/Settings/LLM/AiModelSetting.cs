
using Mediator;

using TestBucket.Domain.AI.Ollama;
using TestBucket.Domain.Progress;

namespace TestBucket.Domain.AI.Settings.LLM
{
    class AiModelSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IProgressManager _progressManager;
        private readonly IMediator _mediator;

        public AiModelSetting(ISettingsProvider settingsProvider, IProgressManager progressManager, IMediator mediator)
        {
            _settingsProvider = settingsProvider;
            _progressManager = progressManager;

            Metadata.Name = "ai-default-model";
            Metadata.Description = null;
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcon.AI;
            Metadata.Section.Name = "Models";
            Metadata.SearchText = "ai-models";
            Metadata.ShowDescription = true;
            Metadata.Type = FieldType.String;
            Metadata.AccessLevel = Identity.Models.AccessLevel.Admin;

            // Azure
            Metadata.Options = ["llama3.1"];
            _mediator = mediator;
        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            context.Principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Read);
            var settings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(context.Principal.GetTenantIdOrThrow(), null);
            settings ??= new();

            return new FieldValue { StringValue = settings.LlmModel, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            var settings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(context.Principal.GetTenantIdOrThrow(), null);
            settings ??= new();

            if (settings.LlmModel != value.StringValue)
            {
                settings.LlmModel = value.StringValue ?? "llama3.1";
                await _settingsProvider.SaveDomainSettingsAsync(context.Principal.GetTenantIdOrThrow(), null, settings);

                if (settings.AiProvider == "ollama" && settings.AiProviderUrl != null && settings.LlmModel is not null)
                {
                    if (!string.IsNullOrEmpty(settings.LlmModel) && settings.AiProvider == "ollama" && !string.IsNullOrEmpty(settings.AiProviderUrl))
                    {
                        await _mediator.Send(new PullModelRequest(context.Principal, settings.LlmModel));
                    }
                }
            }
        }
    }
}
