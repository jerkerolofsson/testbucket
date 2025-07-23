using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.AI.Settings.LLM;
public class LlmSettings
{
    /// <summary>
    /// Provider to use for LLM
    /// </summary>
    public string AiProvider { get; set; } = "ollama";

    /// <summary>
    /// Provider to use for embeddings
    /// </summary>
    public string EmbeddingAiProvider { get; set; } = "ollama";

    /// <summary>
    /// Default model to use for LLM
    /// </summary>
    public string LlmModel { get; set; } = "qwen3:8b";

    /// <summary>
    /// Model to use for test embeddings.
    /// </summary>
    public string? LlmEmbeddingModel { get; set; } = "all-minilm";

    /// <summary>
    /// Price for 1 000 000 tokens in USD with the current model
    /// </summary>
    public double LlmModelUsdPerMillionTokens { get; set; }

    /// <summary>
    /// URL to ollama/azure etc..
    /// </summary>
    public string? AiProviderUrl { get; set; }

    /// <summary>
    /// URL to ollama/azure etc..
    /// </summary>
    public string? EmbeddingAiProviderUrl { get; set; }

    /// <summary>
    /// For github provider
    /// </summary>
    public string? GithubModelsDeveloperKey { get; set; }

    /// <summary>
    /// For azure-ai provider
    /// </summary>
    public string? AzureAiProductionKey { get; set; }

    /// <summary>
    /// For anthropic provider
    /// </summary>
    public string? AnthropicApiKey { get; set; }

    /// <summary>
    /// Open AI API key
    /// </summary>
    public string? OpenAiApiKey { get; set; }
}
