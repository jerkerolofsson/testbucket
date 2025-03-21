
using OllamaSharp;

using TestBucket.Domain.Progress;

namespace TestBucket.Domain.AI.Settings
{
    class AiModelSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IProgressManager _progressManager;

        public AiModelSetting(ISettingsProvider settingsProvider, IProgressManager progressManager)
        {
            _settingsProvider = settingsProvider;
            _progressManager = progressManager;

            Metadata.Name = "LLM Model";
            Metadata.Description = "Model to use for inference";
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcon.AI;
            Metadata.Section.Name = "Provider";
            Metadata.ShowDescription = true;
            Metadata.Type = FieldType.String;
            Metadata.AccessLevel = Identity.Models.AccessLevel.SuperAdmin;

            // Azure
            Metadata.Options = ["llama3.1"];
        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();
            return new FieldValue { StringValue = settings.LlmModel, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
           
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();

            if (settings.LlmModel != value.StringValue)
            {
                settings.LlmModel = value.StringValue ?? "llama3.1";
                await _settingsProvider.SaveGlobalSettingsAsync(settings);

                if (settings.AiProvider == "ollama" && settings.AiProviderUrl != null)
                {
                    // Todo: IMediator notification when setting changed?

                    // Report progress of downloading the model in background task
                    var task = Task.Run(async () =>
                    {
                        await using var progress = _progressManager.CreateProgressTask("Downloading " + settings.LlmModel);

                        var ollama = new OllamaApiClient(settings.AiProviderUrl, settings.LlmModel);
                        await progress.ReportStatusAsync($"Downloading {settings.LlmModel}", 0);
                        await foreach (var response in ollama.PullModelAsync(settings.LlmModel))
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
