
using OllamaSharp;

using TestBucket.Domain.Progress;

namespace TestBucket.Domain.AI.Settings
{
    class AiLlmTestGenerationModelSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IProgressManager _progressManager;

        public AiLlmTestGenerationModelSetting(ISettingsProvider settingsProvider, IProgressManager progressManager)
        {
            _settingsProvider = settingsProvider;
            _progressManager = progressManager;

            Metadata.Name = "Test Generator Model";
            Metadata.Description = "Model to use for test case generation. This model requires support for tools (DeepSeek-R1 is not supported). If empty the default model will be used.";
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcon.AI;
            Metadata.Section.Name = "Models";
            Metadata.SearchText = "ai-models";
            Metadata.ShowDescription = true;
            Metadata.Type = FieldType.String;
            Metadata.AccessLevel = Identity.Models.AccessLevel.SuperAdmin;
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
                    // Report progress of downloading the model in background task
                    var task = Task.Run(async () =>
                    {
                        await using var progress = _progressManager.CreateProgressTask("Downloading " + settings.LlmTestGenerationModel);

                        var ollama = new OllamaApiClient(settings.AiProviderUrl, settings.LlmTestGenerationModel);
                        await foreach (var response in ollama.PullModelAsync(settings.LlmTestGenerationModel))
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
