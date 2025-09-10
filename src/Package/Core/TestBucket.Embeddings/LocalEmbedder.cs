// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Onnx;
using Microsoft.SemanticKernel.Embeddings;

namespace TestBucket.Embeddings;

public sealed partial class LocalEmbedder : IDisposable, IEmbeddingGenerator<string, Embedding<float>>
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

    public LocalEmbedder(string modelName = "default", bool caseSensitive = false, int maximumTokens = 512)
        : this(modelName, new BertOnnxOptions { CaseSensitive = caseSensitive, MaximumTokens = maximumTokens })
    {
    }

    public LocalEmbedder(BertOnnxOptions options)
        : this("default", options)
    {
    }

    public LocalEmbedder(string modelName, BertOnnxOptions options)
    {
        var modelFilePath = GetFullPathToModelFile(modelName, "model.onnx");
        var vocabFilePath = GetFullPathToModelFile(modelName, "vocab.txt");

        var services = new ServiceCollection();
        services.AddBertOnnxEmbeddingGenerator(modelFilePath, vocabFilePath, options);
        var serviceProvider = services.BuildServiceProvider();
        _embeddingGenerator = serviceProvider.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

        //_embeddingGenerator = BertOnnxTextEmbeddingGenerationService.Create(
        //    GetFullPathToModelFile(modelName, "model.onnx"),
        //    vocabPath: GetFullPathToModelFile(modelName, "vocab.txt"),
        //    options);
    }

    private static string GetFullPathToModelFile(string modelName, string fileName)
    {
        var baseDir = AppContext.BaseDirectory;
        var fullPath = Path.Combine(baseDir, "LocalEmbeddingsModel", modelName, fileName);
        if (!File.Exists(fullPath))
        {
            throw new InvalidOperationException($"Required file {fullPath} does not exist");
        }

        return fullPath;
    }

    public void Dispose()
        => _embeddingGenerator.Dispose();

    public async ValueTask<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IList<string> data, CancellationToken cancellationToken = default)
    {
        var embeddingVectors = await _embeddingGenerator.GenerateAsync(data, null, cancellationToken);
        return embeddingVectors.Select(x => x.Vector).ToList();
    }


    public async Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(IEnumerable<string> values, EmbeddingGenerationOptions? options = null, CancellationToken cancellationToken = default)
    {
        //var embeddingVectors = await _embeddingGenerator.GenerateEmbeddingsAsync(values.ToList(), null, cancellationToken);
        var embeddingVectors = await _embeddingGenerator.GenerateAsync(values.ToList(), null, cancellationToken);
        return embeddingVectors;
        //var embeddings = embeddingVectors.Select(x => new Embedding<float>(x.ToArray()));
        //return new GeneratedEmbeddings<Embedding<float>>(embeddings);
    }

    public object? GetService(Type serviceType, object? serviceKey = null) => null;
}
