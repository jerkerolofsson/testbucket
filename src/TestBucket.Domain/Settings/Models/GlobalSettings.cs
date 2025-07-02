using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Settings.Models;
public class GlobalSettings
{
    public long Id { get; set; }

    /// <summary>
    /// The default tenant when the user logs in
    /// </summary>
    public string DefaultTenant { get; set; } = "default";

    /// <summary>
    /// Provider to use for LLM
    /// ollama, github-models, azure-ai
    /// </summary>
    public string AiProvider { get; set; } = "ollama";

    /// <summary>
    /// Symmetric key for signing
    /// </summary>
    public string? SymmetricJwtKey { get; set; }

    /// <summary>
    /// JWT issuer
    /// </summary>
    public string? JwtIssuer { get; set; }

    /// <summary>
    /// JWT audience
    /// </summary>
    public string? JwtAudience { get; set; }

    /// <summary>
    /// Default model to use for LLM
    /// </summary>
    public string LlmModel { get; set; } = "phi4-mini:3.8b";

    /// <summary>
    /// Model to use for test embeddings.
    /// </summary>
    public string? LlmEmbeddingModel { get; set; } = "all-minilm";

    /// <summary>
    /// Model to use for LLM classification. If null the default model will be used
    /// </summary>
    public string? LlmClassificationModel { get; set; }

    /// <summary>
    /// URL to ollama/azure etc..
    /// </summary>
    public string? AiProviderUrl { get; set; }

    /// <summary>
    /// For github provider
    /// </summary>
    public string? GithubModelsDeveloperKey { get; set; }

    /// <summary>
    /// Forazure-ai provider
    /// </summary>
    public string? AzureAiProductionKey { get; set; }

    /// <summary>
    /// Keep track of the changes
    /// </summary>
    public int Revision { get; set; }

    /// <summary>
    /// Base url where the Test Bucket server is publicly accessible from
    /// </summary>
    public string? PublicEndpointUrl { get; set; }
}
