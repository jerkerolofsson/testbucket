using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OneOf.Types;

using TestBucket.Domain.AI.Models;

namespace TestBucket.Domain.AI;
public class LlmModels
{
    /// <summary>
    /// List of LLM models that will be options in the UI
    /// </summary>
    public static IReadOnlyDictionary<string, LlmModel> Models = new Dictionary<string, LlmModel>
    {
        ["llama3.2:1b"] = new LlmModel
        {
            Icon = TbIcons.Brands.Meta,
            Vendor = "meta",
            Name = "Llama3.2 1b",
            OllamaName = "llama3.2:1b",
            Capabilities = ModelCapability.Classification
        },
        ["llama3.2:3b"] = new LlmModel
        {
            Icon = TbIcons.Brands.Meta,
            Vendor = "meta",
            Name = "Llama3.2 3b",
            OllamaName = "llama3.2:3b",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools
        },
        ["llama3.1:8b"] = new LlmModel
        {
            Icon = TbIcons.Brands.Meta,
            Vendor = "meta",
            Name = "Llama3.1 8b",
            OllamaName = "llama3.1:8b",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools
        },
        ["deepseek-r1:1.5b"] = new LlmModel
        {
            Icon = TbIcons.Brands.Deepseek,
            Vendor = "deepseek",
            Name = "Deepseek R1 1.5b",
            OllamaName = "deepseek-r1:1.5b",
            Capabilities = ModelCapability.Thinking | ModelCapability.Classification
        },
        ["deepseek-r1:8b"] = new LlmModel
        {
            Icon = TbIcons.Brands.Deepseek,
            Vendor = "deepseek",
            Name = "Deepseek R1 8b",
            OllamaName = "deepseek-r1:8b",
            Capabilities = ModelCapability.Tools | ModelCapability.Thinking | ModelCapability.Classification
        },
        ["nemotron-mini"] = new LlmModel
        {
            Vendor = "nvidia",
            Name = "nemotron-mini:4b",
            OllamaName = "nemotron-mini:4b",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools
        },

        ["phi3:3.8b"] = new LlmModel
        {
            Icon = TbIcons.Brands.Microsoft,
            Vendor = "microsoft",
            Name = "Phi3 3.8b",
            OllamaName = "phi3:3.8b",
            Capabilities = ModelCapability.Classification
        },
        ["phi4"] = new LlmModel
        {
            Vendor = "microsoft",
            Name = "phi4",
            OllamaName = "phi4",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools
        },
        ["phi4-mini"] = new LlmModel
        {
            Icon = TbIcons.Brands.Microsoft,
            Vendor = "microsoft",
            Name = "Phi4 mini",
            OllamaName = "phi4-mini:3.8b",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools
        },
        ["qwen3:0.6b"] = new LlmModel
        {
            Icon = TbIcons.Brands.AlibabaCloud,
            Vendor = "alibaba-cloud",
            Name = "Qwen3 0.6b",
            OllamaName = "qwen3:0.6b",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools | ModelCapability.Thinking
        },
        ["qwen3:8b"] = new LlmModel
        {
            Icon = TbIcons.Brands.AlibabaCloud,
            Vendor = "alibaba-cloud",
            Name = "Qwen3 8b",
            OllamaName = "qwen3:8b",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools | ModelCapability.Thinking
        },
        ["qwen3:1.7b"] = new LlmModel
        {
            Icon = TbIcons.Brands.AlibabaCloud,
            Vendor = "alibaba-cloud",
            Name = "Qwen3 1.7b",
            OllamaName = "qwen3:1.7b",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools | ModelCapability.Thinking
        },
        ["all-minilm"] = new LlmModel
        {
            Icon = TbIcons.Brands.HuggingFace,
            Vendor = "hugging-face",
            Name = "all-minilm",
            OllamaName = "all-minilm",
            Capabilities = ModelCapability.Embedding
        },
    };

    public static LlmModel? GetModelByName(string name)
    {
        return Models.Values.Where(x => x.Name == name).FirstOrDefault();
    }

    public static IEnumerable<LlmModel> GetModels(ModelCapability requiredCapability)
    {
        return Models.Values.Where(x => (x.Capabilities & requiredCapability) == requiredCapability);
    }
    internal static List<string> GetNames(ModelCapability requiredCapability)
    {
        return Models.Values.Where(x => (x.Capabilities & requiredCapability) == requiredCapability).Select(x => x.Name).ToList();
    }
}
