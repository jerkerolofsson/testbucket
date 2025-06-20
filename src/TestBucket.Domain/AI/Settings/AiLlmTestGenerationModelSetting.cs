
using Mediator;
using TestBucket.Domain.AI.Models;
using TestBucket.Domain.AI.Ollama;

namespace TestBucket.Domain.AI.Settings
{
    class AiLlmTestGenerationModelSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IMediator _mediator;

        public AiLlmTestGenerationModelSetting(ISettingsProvider settingsProvider, IMediator progressManager)
        {
            _settingsProvider = settingsProvider;
            _mediator = progressManager;

            Metadata.Name = "ai-test-generator-model";
            Metadata.Description = null;
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcon.AI;
            Metadata.Section.Name = "ai-models";
            Metadata.SearchText = "ai-models";
            Metadata.ShowDescription = true;
            Metadata.Type = FieldType.String;
            Metadata.AccessLevel = Identity.Models.AccessLevel.SuperAdmin;
            Metadata.Options = LlmModels.GetNames(ModelCapability.Tools);
        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();
            return new FieldValue { StringValue = settings.LlmTestGenerationModel, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
           
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();

            if (settings.LlmTestGenerationModel != value.StringValue)
            {
                settings.LlmTestGenerationModel = value.StringValue;
                await _settingsProvider.SaveGlobalSettingsAsync(settings);

                if (settings.AiProvider == "ollama" && settings.AiProviderUrl != null && settings.LlmTestGenerationModel is not null)
                {
                    if (!string.IsNullOrEmpty(settings.LlmClassificationModel) && settings.AiProvider == "ollama" && !string.IsNullOrEmpty(settings.AiProviderUrl))
                    {
                        await _mediator.Send(new PullModelRequest(settings.LlmClassificationModel));
                    }
                }
            }
        }
    }
}
