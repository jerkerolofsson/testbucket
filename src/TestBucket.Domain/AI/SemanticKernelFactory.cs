using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

using TestBucket.Domain.AI.Settings.LLM;

namespace TestBucket.Domain.AI;
internal class SemanticKernelFactory
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly ILogger<SemanticKernelFactory> _logger;

    public SemanticKernelFactory(ISettingsProvider settingsProvider, ILogger<SemanticKernelFactory> logger)
    {
        _settingsProvider = settingsProvider;
        _logger = logger;
    }

    public async Task<Kernel> CreateKernelAsync(ClaimsPrincipal principal)
    {
        var settings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(principal.GetTenantIdOrThrow(), null);
        settings ??= new();

        var builder = Kernel.CreateBuilder();

        if(settings.AiProvider is null || string.IsNullOrEmpty(settings.AiProviderUrl) || string.IsNullOrEmpty(settings.LlmModel))
        {
            throw new Exception("AI Provider and Model is not configured");
        }

        var llmModel = LlmModels.GetModelByName(settings.LlmModel);
        var modelId = llmModel?.ModelName ?? settings.LlmModel;

        switch (settings.AiProvider)
        {
            case "ollama":
                builder.AddOllamaChatCompletion(modelId, new Uri(settings.AiProviderUrl));
                break;

            case "anthropic":
                builder.AddOpenAIChatCompletion(modelId, new Uri(settings.AiProviderUrl), settings.AnthropicApiKey);
                break;

            case "open-ai":
                builder.AddOpenAIChatCompletion(modelId, new Uri(settings.AiProviderUrl), settings.OpenAiApiKey);
                break;

            default:
                throw new NotImplementedException();
        }

        var kernel = builder.Build();
        return kernel;
    }

}
