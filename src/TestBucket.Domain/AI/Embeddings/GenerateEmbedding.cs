using Mediator;

using Microsoft.Extensions.AI;

using OllamaSharp;

namespace TestBucket.Domain.AI.Embeddings;
public record class GenerateEmbeddingRequest(long ProjectId, string Text) : IRequest<GenerateEmbeddingResponse>;
public record class GenerateEmbeddingResponse(ReadOnlyMemory<float>? EmbeddingVector);

public class GenerateEmbeddingHandler : IRequestHandler<GenerateEmbeddingRequest, GenerateEmbeddingResponse>
{
    private readonly ISettingsProvider _settingsProvider;

    public GenerateEmbeddingHandler(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }


    private async Task<IEmbeddingGenerator<string, Embedding<float>>?> CreateEmbeddingGeneratorAsync()
    {
        var settings = await _settingsProvider.LoadGlobalSettingsAsync();
        if (settings.AiProvider == "ollama" && settings.AiProviderUrl is not null)
        {
            string? model = settings.LlmEmbeddingModel;
            if(model is null)
            {
                return null;
            }
            var ollamaModelName = LlmModels.GetModelByName(model)?.OllamaName;
            if(ollamaModelName is null)
            {
                return null;
            }

            try
            {
                var ollama = new OllamaApiClient(new OllamaApiClient.Configuration
                {
                    Model = model,
                    Uri = new Uri(settings.AiProviderUrl),
                });
                return ollama;
            }
            catch (Exception) { }
        }
        return null;
    }

    public async ValueTask<GenerateEmbeddingResponse> Handle(GenerateEmbeddingRequest request, CancellationToken cancellationToken)
    {
        var client = await CreateEmbeddingGeneratorAsync();
        if(client is not null)
        {
            var embedding = await client.GenerateAsync(request.Text);
            return new GenerateEmbeddingResponse(embedding.Vector);
        }

        return new GenerateEmbeddingResponse(null);
    }
}
