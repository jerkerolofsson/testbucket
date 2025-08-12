using Mediator;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using OllamaSharp;

using TestBucket.Domain.AI.Settings.LLM;

namespace TestBucket.Domain.AI.Embeddings;
public record class GenerateEmbeddingRequest(ClaimsPrincipal Principal, long ProjectId, string Text) : IRequest<GenerateEmbeddingResponse>;
public record class GenerateEmbeddingResponse(ReadOnlyMemory<float>? EmbeddingVector);

public class GenerateEmbeddingHandler : IRequestHandler<GenerateEmbeddingRequest, GenerateEmbeddingResponse>
{
    private readonly ILogger<GenerateEmbeddingHandler> _logger;
    private readonly ISettingsProvider _settingsProvider;

    public GenerateEmbeddingHandler(ILogger<GenerateEmbeddingHandler> logger, ISettingsProvider settingsProvider)
    {
        _logger = logger;
        _settingsProvider = settingsProvider;
    }


    private async Task<IEmbeddingGenerator<string, Embedding<float>>?> CreateEmbeddingGeneratorAsync(ClaimsPrincipal principal)
    {
        var settings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(principal.GetTenantIdOrThrow(), null);
        settings ??= new();

        if (settings.EmbeddingAiProvider == "ollama" && settings.EmbeddingAiProviderUrl is not null)
        {
            string? model = settings.LlmEmbeddingModel;
            if(model is null)
            {
                return null;
            }
            var ollamaModelName = LlmModels.GetModelByName(model)?.ModelName;
            if(ollamaModelName is null)
            {
                return null;
            }

            try
            {
                var ollama = new OllamaApiClient(new OllamaApiClient.Configuration
                {
                    Model = model,
                    Uri = new Uri(settings.EmbeddingAiProviderUrl),
                });

                return new OllamaModelPullingEmbeddingGenerator(ollama, _logger);
            }
            catch (Exception) { }
        }
        else
        {
            // Use local embeddings
            return new TestBucket.Embeddings.LocalEmbedder();
        }
        return null;
    }

    public async ValueTask<GenerateEmbeddingResponse> Handle(GenerateEmbeddingRequest request, CancellationToken cancellationToken)
    {
        var client = await CreateEmbeddingGeneratorAsync(request.Principal);
        if(client is not null)
        {
            var embedding = await client.GenerateAsync(request.Text);
            return new GenerateEmbeddingResponse(embedding.Vector);
        }

        return new GenerateEmbeddingResponse(null);
    }
}
