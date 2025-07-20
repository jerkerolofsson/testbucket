using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using OllamaSharp;

namespace TestBucket.Domain.AI.Embeddings;
internal class OllamaModelPullingEmbeddingGenerator(OllamaApiClient Ollama, ILogger Logger) : IEmbeddingGenerator<string, Embedding<float>>
{
    private bool _disposedValue;

    public async Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(IEnumerable<string> values, EmbeddingGenerationOptions? options = null, CancellationToken cancellationToken = default)
    {
        IEmbeddingGenerator<string, Embedding<float>> ollamaEmbedding = Ollama;
        try
        {
            return await ollamaEmbedding.GenerateAsync(values, options, cancellationToken);  
        }
        catch(HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Logger.LogWarning("Ollama GenerateAsync responded with 404 NotFound. This may happen if the model failed to download, or this may be the first time - let's try to download it..");
            await foreach(var message in Ollama.PullModelAsync(Ollama.Config.Model, cancellationToken))
            {
                Logger.LogDebug("Pulling model {ModelName}: {Message}", Ollama.Config.Model, message);
            }
            return await ollamaEmbedding.GenerateAsync(values, options, cancellationToken);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Ollama.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        IEmbeddingGenerator<string, Embedding<float>> ollamaEmbedding = Ollama;
        return ollamaEmbedding.GetService(serviceType, serviceKey);  
    }
}
