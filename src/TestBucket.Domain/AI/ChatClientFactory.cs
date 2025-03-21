using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Build.Framework;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

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

    public async Task<IChatClient?> CreateChatClientAsync()
    {
        return await CreateOllamaClientAsync();
    }

    private async Task<IChatClient?> CreateOllamaClientAsync()
    {
        var settings = await _settingsProvider.LoadGlobalSettingsAsync();
        if (!string.IsNullOrEmpty(settings.AiProviderUrl))
        {
            //string model = "deepseek-r1:7b";
            string model = settings.LlmModel ?? "deepseek-r1:30b";

            if(model == "DeepSeek-R1")
            {
                model = "deepseek-r1:7b";
            }

            // deepseek-r1:7b
            //var ollama = new OllamaApiClient(ollamaBaseUrl, "deepseek-r1:7b");
            try
            {
                var ollama = new OllamaChatClient(settings.AiProviderUrl, model)
                    .AsBuilder()
                    .UseFunctionInvocation()
                    .Build();

                //await foreach (var response in ollama.PullModelAsync(model))
                //{
                //    if (response is not null)
                //    {
                //        _logger.LogInformation($"{response.Status}: {response.Completed}/{response.Total} ({response.Percent})");
                //    }
                //}

                return ollama;
            }
            catch (Exception) { }
        }
        return null;
    }
}
