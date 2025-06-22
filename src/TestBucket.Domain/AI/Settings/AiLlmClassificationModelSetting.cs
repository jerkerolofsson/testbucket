
using Mediator;

using TestBucket.Domain.AI.Models;
using TestBucket.Domain.AI.Ollama;

namespace TestBucket.Domain.AI.Settings
{
    class AiLlmClassificationModelSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IMediator _mediator;

        public AiLlmClassificationModelSetting(ISettingsProvider settingsProvider, IMediator mediator)
        {
            _settingsProvider = settingsProvider;
            _mediator = mediator;

            Metadata.Name = "ai-classification-model";
            Metadata.Description = null;
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcon.AI;
            Metadata.Section.Name = "ai-models";
            Metadata.ShowDescription = true;
            Metadata.SearchText = "ai-models";
            Metadata.Type = FieldType.String;
            Metadata.Options = LlmModels.GetNames(ModelCapability.Classification);
            Metadata.AccessLevel = Identity.Models.AccessLevel.SuperAdmin;
        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();
            return new FieldValue { StringValue = settings.LlmClassificationModel, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();

            if (settings.LlmClassificationModel != value.StringValue)
            {
                settings.LlmClassificationModel = value.StringValue;
                await _settingsProvider.SaveGlobalSettingsAsync(settings);

                if (!string.IsNullOrEmpty(settings.LlmClassificationModel) && settings.AiProvider == "ollama" && !string.IsNullOrEmpty(settings.AiProviderUrl))
                {
                    await _mediator.Send(new PullModelRequest(settings.LlmClassificationModel));
                }
            }
        }
    }
}
