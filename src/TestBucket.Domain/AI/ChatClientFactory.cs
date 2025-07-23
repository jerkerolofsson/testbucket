using System.ClientModel;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using OllamaSharp;

using OpenAI;

using TestBucket.Domain.AI.Models;
using TestBucket.Domain.AI.Settings.LLM;

namespace TestBucket.Domain.AI;
internal class ChatClientFactory : IChatClientFactory
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly ILogger<ChatClientFactory> _logger;

    public ChatClientFactory(ISettingsProvider settingsProvider, ILogger<ChatClientFactory> logger)
    {
        _settingsProvider = settingsProvider;
        _logger = logger;
    }

    public async Task<string?> GetModelNameAsync(ClaimsPrincipal principal, ModelType modelType)
    {
        var settings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(principal.GetTenantIdOrThrow(), null);
        settings ??= new();
        if (!string.IsNullOrEmpty(settings.AiProviderUrl))
        {
            return GetModelName(modelType, settings);

        }
        return null;
    } 
    public async Task<IChatClient?> CreateChatClientAsync(ClaimsPrincipal principal, ModelType modelType)
    {
        var settings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(principal.GetTenantIdOrThrow(), null);
        settings ??= new();

        if (settings.AiProvider == "ollama")
        {
            return await CreateOllamaClientAsync(principal, modelType);
        }
        if (settings.AiProvider == "anthropic")
        {
            return CreateOpenAIChatClient(modelType, settings.AiProviderUrl, settings.AnthropicApiKey, settings.LlmModel);
        }
        if (settings.AiProvider == "open-ai")
        {
            return CreateOpenAIChatClient(modelType, settings.AiProviderUrl, settings.OpenAiApiKey, settings.LlmModel);
        }

        return null;
    }

    private IChatClient? CreateOpenAIChatClient(ModelType modelType, string? aiProviderUrl, string? anthropicApiKey, string model)
    {
        if(anthropicApiKey is null || aiProviderUrl is null)
        {
            return null;
        }
        var client = new OpenAI.OpenAIClient(new ApiKeyCredential(anthropicApiKey), new OpenAIClientOptions
        {
            Endpoint = new Uri(aiProviderUrl)
        });

        model = GetApiModelName(model);

        var chatClient = client.GetChatClient(model).AsIChatClient();
        return new ChatClientBuilder(chatClient).UseFunctionInvocation().Build();
    }

    private async Task<IChatClient?> CreateOllamaClientAsync(ClaimsPrincipal principal, ModelType modelType)
    {
        var settings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(principal.GetTenantIdOrThrow(), null);
        settings ??= new();

        if (!string.IsNullOrEmpty(settings.AiProviderUrl))
        {
            string model = GetModelName(modelType, settings);
            model = GetApiModelName(model);

            try
            {
                var ollama = new OllamaSharp.OllamaApiClient(new OllamaApiClient.Configuration
                {
                    Model = model,
                    Uri = new Uri(settings.AiProviderUrl),
                });
                return new ChatClientBuilder(ollama).UseFunctionInvocation().Build();
            }
            catch (Exception) { }
        }
        return null;
    }

    private static string GetApiModelName(string model)
    {
        return LlmModels.GetModelByName(model)?.ModelName ?? model;
    }

    /// <summary>
    /// Returns a model from a use case name
    /// </summary>
    /// <param name="modelType"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static string GetModelName(ModelType modelType, LlmSettings settings)
    {
        string model = settings.LlmModel ?? "phi4-mini:3.8b";
        return model;
    }
}
