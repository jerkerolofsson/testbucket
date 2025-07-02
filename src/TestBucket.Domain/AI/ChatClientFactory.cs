using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OllamaSharp;

using TestBucket.Domain.AI.Models;

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

    public async Task<string?> GetModelNameAsync(ModelType modelType)
    {
        var settings = await _settingsProvider.LoadGlobalSettingsAsync();
        if (!string.IsNullOrEmpty(settings.AiProviderUrl))
        {
            return GetModelName(modelType, settings);

        }
        return null;
    } 
    public async Task<IChatClient?> CreateChatClientAsync(ModelType modelType)
    {
        return await CreateOllamaClientAsync(modelType);
    }

    private async Task<IChatClient?> CreateOllamaClientAsync(ModelType modelType)
    {
        var settings = await _settingsProvider.LoadGlobalSettingsAsync();
        if (!string.IsNullOrEmpty(settings.AiProviderUrl))
        {
            string model = GetModelName(modelType, settings);
            model = GetOllamaModelName(model);

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

    private static string GetOllamaModelName(string model)
    {
        return LlmModels.GetModelByName(model)?.OllamaName ?? model;
    }

    /// <summary>
    /// Returns a model from a use case name
    /// </summary>
    /// <param name="modelType"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    private static string GetModelName(ModelType modelType, GlobalSettings settings)
    {
        string model = settings.LlmModel ?? "phi4-mini:3.8b";
        if (modelType == ModelType.Classification && !string.IsNullOrEmpty(settings.LlmClassificationModel))
        {
            model = settings.LlmClassificationModel;
        }

        return model;
    }
}
