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
    /// Model to use for LLM
    /// </summary>
    public string LlmModel { get; set; } = "deepseek-r1:30b";

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
}
