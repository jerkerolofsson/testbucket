
using OllamaSharp;

using TestBucket.Domain.Progress;

namespace TestBucket.Domain.AI.Settings
{
    class AiLlmClassificationModelSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IProgressManager _progressManager;

        public AiLlmClassificationModelSetting(ISettingsProvider settingsProvider, IProgressManager progressManager)
        {
            _settingsProvider = settingsProvider;
            _progressManager = progressManager;

            Metadata.Name = "Classification Model";
            Metadata.Description = "Model to use for classification and categorization. DeepSeek-R1 and llama3.1 are both supported. If empty the default model will be used";
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcon.AI;
            Metadata.Section.Name = "Models";
            Metadata.ShowDescription = true;
            Metadata.SearchText = "ai-models";
            Metadata.Type = FieldType.String;
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

                if (settings.AiProvider == "ollama" && settings.AiProviderUrl != null && settings.LlmClassificationModel is not null)
                {
                    // Todo: IMediator notification when setting changed?

                    // Report progress of downloading the model in background task
                    var task = Task.Run(async () =>
                    {
                        await using var progress = _progressManager.CreateProgressTask("Downloading " + settings.LlmClassificationModel);

                        var ollama = new OllamaApiClient(settings.AiProviderUrl, settings.LlmClassificationModel);
                        await foreach (var response in ollama.PullModelAsync(settings.LlmClassificationModel))
                        {
                            if (response is not null)
                            {
                                await progress.ReportStatusAsync($"{response.Status}", response.Percent);
                            }
                        }
                    });
                }
            }
        }
    }
}
