using Mediator;
using OllamaSharp;
using TestBucket.Domain.Progress;


namespace TestBucket.Domain.AI.Ollama;
public record class PullModelRequest(string Name) : IRequest<bool>;

public class PullModelRequestHandler : IRequestHandler<PullModelRequest, bool>
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly IProgressManager _progressManager;

    public PullModelRequestHandler(ISettingsProvider settingsProvider, IProgressManager manager)
    {
        _settingsProvider = settingsProvider;
        _progressManager = manager;
    }

    public async ValueTask<bool> Handle(PullModelRequest request, CancellationToken cancellationToken)
    {
        var model = LlmModels.GetModelByName(request.Name);
        if (model?.OllamaName is null)
        {
            return false;
        }

        var settings = await _settingsProvider.LoadGlobalSettingsAsync();
        if(settings.AiProvider != "ollama" || string.IsNullOrEmpty(settings.AiProviderUrl))
        {
            return false;
        }

        // Report progress of downloading the model in background task
        var task = Task.Run(async () =>
        {
            await using var progress = _progressManager.CreateProgressTask("Downloading " + model.OllamaName);

            var ollama = new OllamaApiClient(settings.AiProviderUrl, model.OllamaName);
            await foreach (var response in ollama.PullModelAsync(model.OllamaName))
            {
                if (response is not null)
                {
                    await progress.ReportStatusAsync($"{response.Status}", response.Percent);
                }
            }
        });
        return true;
    }
}
