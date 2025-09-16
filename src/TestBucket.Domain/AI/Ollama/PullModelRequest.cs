using Mediator;

using OllamaSharp;

using TestBucket.Domain.AI.Settings.LLM;
using TestBucket.Domain.Progress;


namespace TestBucket.Domain.AI.Ollama;
public record class PullModelRequest(ClaimsPrincipal Principal, string Name) : IRequest<bool>;

public class PullModelRequestHandler : IRequestHandler<PullModelRequest, bool>
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly IProgressManager _progressManager;

    public PullModelRequestHandler(ISettingsProvider settingsProvider, ProgressSystemManager manager)
    {
        _settingsProvider = settingsProvider;
        _progressManager = manager;
    }

    public async ValueTask<bool> Handle(PullModelRequest request, CancellationToken cancellationToken)
    {
        var model = LlmModels.GetModelByName(request.Name);
        if (model?.ModelName is null)
        {
            return false;
        }

        var settings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(request.Principal.GetTenantIdOrThrow(), null);
        settings ??= new();
        if (settings.AiProvider != "ollama" || string.IsNullOrEmpty(settings.AiProviderUrl))
        {
            return false;
        }

        // Report progress of downloading the model in background task
        var task = Task.Run(async () =>
        {
            await using var progress = _progressManager.CreateProgressTask("Downloading " + model.ModelName);

            var ollama = new OllamaApiClient(settings.AiProviderUrl, model.ModelName);
            try
            {
                await foreach (var response in ollama.PullModelAsync(model.ModelName))
                {
                    if (response is not null)
                    {
                        await progress.ReportStatusAsync($"{response.Status}", response.Percent);
                    }
                }
            }
            catch(Exception ex)
            {
                await progress.ReportStatusAsync($"{ex.Message}", 0);
                await Task.Delay(2000);
            }
        });
        return true;
    }
}
