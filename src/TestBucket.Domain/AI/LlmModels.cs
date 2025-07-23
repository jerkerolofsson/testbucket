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
    public const string DefaultEmbeddingModel = "all-minilm";

    /// <summary>
    /// List of LLM models that will be options in the UI
    /// </summary>
    public static IReadOnlyDictionary<string, LlmModel> Models = new Dictionary<string, LlmModel>
    {
        ["orieg/gemma3-tools:12b"] = new LlmModel 
        {
            Icon = TbIcons.Brands.Gemma,
            Vendor = "google",
            Name = "Gemma 3 12b",
            ModelName = "orieg/gemma3-tools:12b",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools
        },

        ["llama3.2:1b"] = new LlmModel
        {
            Icon = TbIcons.Brands.Meta,
            Vendor = "meta",
            Name = "Llama3.2 1b",
            ModelName = "llama3.2:1b",
            Capabilities = ModelCapability.Classification
        },
        ["llama3.2:3b"] = new LlmModel
        {
            Icon = TbIcons.Brands.Meta,
            Vendor = "meta",
            Name = "Llama3.2 3b",
            ModelName = "llama3.2:3b",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools
        },
        ["llama3.1:8b"] = new LlmModel
        {
            Icon = TbIcons.Brands.Meta,
            Vendor = "meta",
            Name = "Llama3.1 8b",
            ModelName = "llama3.1:8b",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools
        },
        ["deepseek-r1:1.5b"] = new LlmModel
        {
            Icon = TbIcons.Brands.Deepseek,
            Vendor = "deepseek",
            Name = "Deepseek R1 1.5b",
            ModelName = "deepseek-r1:1.5b",
            Capabilities = ModelCapability.Thinking | ModelCapability.Classification
        },
        ["deepseek-r1:8b"] = new LlmModel
        {
            Icon = TbIcons.Brands.Deepseek,
            Vendor = "deepseek",
            Name = "Deepseek R1 8b",
            ModelName = "deepseek-r1:8b",
            Capabilities = ModelCapability.Tools | ModelCapability.Thinking | ModelCapability.Classification
        },
        ["mistral:7b"] = new LlmModel 
        { 
            Name = "Mistral 7b",    
            ModelName = "mistral:7b",
            Vendor = "mistral",
            Capabilities = ModelCapability.Tools,
            Icon = TbIcons.Brands.Mistral
        },
        ["nemotron-mini"] = new LlmModel
        {
            Vendor = "nvidia",
            Name = "nemotron-mini:4b",
            ModelName = "nemotron-mini:4b",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools
        },
        ["phi3:3.8b"] = new LlmModel
        {
            Icon = TbIcons.Brands.Microsoft,
            Vendor = "microsoft",
            Name = "Phi3 3.8b",
            ModelName = "phi3:3.8b",
            Capabilities = ModelCapability.Classification
        },
        ["phi4"] = new LlmModel
        {
            Vendor = "microsoft",
            Name = "phi4",
            ModelName = "phi4",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools
        },
        ["phi4-mini"] = new LlmModel
        {
            Icon = TbIcons.Brands.Microsoft,
            Vendor = "microsoft",
            Name = "Phi4 mini",
            ModelName = "phi4-mini:3.8b",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools
        },
        ["qwen3:0.6b"] = new LlmModel
        {
            Icon = TbIcons.Brands.AlibabaCloud,
            Vendor = "alibaba-cloud",
            Name = "Qwen3 0.6b",
            ModelName = "qwen3:0.6b",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools | ModelCapability.Thinking
        },
        ["qwen3:8b"] = new LlmModel
        {
            Icon = TbIcons.Brands.AlibabaCloud,
            Vendor = "alibaba-cloud",
            Name = "Qwen3 8b",
            ModelName = "qwen3:8b",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools | ModelCapability.Thinking
        },
        ["qwen3:1.7b"] = new LlmModel
        {
            Icon = TbIcons.Brands.AlibabaCloud,
            Vendor = "alibaba-cloud",
            Name = "Qwen3 1.7b",
            ModelName = "qwen3:1.7b",
            Capabilities = ModelCapability.Classification | ModelCapability.Tools | ModelCapability.Thinking
        },
        ["all-minilm"] = new LlmModel
        {
            Icon = TbIcons.Brands.HuggingFace,
            Vendor = "hugging-face",
            Name = "all-minilm",
            ModelName = "all-minilm",
            Capabilities = ModelCapability.Embedding
        },
    };

    /// <summary>
    /// List of LLM models that will be options in the UI
    /// </summary>
    public static IReadOnlyDictionary<string, LlmModel> OpenAiModels = new Dictionary<string, LlmModel>
    {
        ["gpt-4.1"] = new LlmModel
        {
            UsdPerMillionTokens = 2.0,
            Icon = TbIcons.Brands.OpenAI,
            Vendor = "open-ai",
            Name = "GPT 4.1",
            ModelName = "gpt-4.1",
            Capabilities = ModelCapability.Tools
        },
        ["gpt-4.1-mini"] = new LlmModel
        {
            UsdPerMillionTokens = 0.4,
            Icon = TbIcons.Brands.OpenAI,
            Vendor = "open-ai",
            Name = "GPT 4.1 Mini",
            ModelName = "gpt-4.1-mini",
            Capabilities = ModelCapability.Tools
        },
        ["gpt-4o-mini"] = new LlmModel
        {
            UsdPerMillionTokens = 0.15,
            Icon = TbIcons.Brands.OpenAI,
            Vendor = "open-ai",
            Name = "GPT 4o Mini",
            ModelName = "gpt-4o-mini",
            Capabilities = ModelCapability.Tools
        },
        ["o4-mini"] = new LlmModel
        {
            UsdPerMillionTokens = 1.1,
            Icon = TbIcons.Brands.OpenAI,
            Vendor = "open-ai",
            Name = "o4 Mini",
            ModelName = "o4-mini",
            Capabilities = ModelCapability.Tools
        }
    };
    /// <summary>
    /// List of LLM models that will be options in the UI
    /// </summary>
    public static IReadOnlyDictionary<string, LlmModel> AnthropicModels = new Dictionary<string, LlmModel>
    {
        ["claude-sonnet-4-20250514"] = new LlmModel
        {
            Icon = TbIcons.Brands.Claude,
            Vendor = "anthropic",
            Name = "Claude Sonnet 4",
            ModelName = "claude-sonnet-4-20250514",
            Capabilities = ModelCapability.Tools | ModelCapability.Thinking
        },
        ["claude-opus-4-20250514"] = new LlmModel
        {
            Icon = TbIcons.Brands.Claude,
            Vendor = "anthropic",
            Name = "Claude Opus 4",
            ModelName = "claude-opus-4-20250514",
            Capabilities = ModelCapability.Tools | ModelCapability.Thinking
        },
    };

    public static LlmModel? GetModelByName(string name)
    {
        var model = Models.Values.Where(x => x.Name == name).FirstOrDefault();
        model ??= OpenAiModels.Values.Where(x => x.Name == name).FirstOrDefault();
        model ??= AnthropicModels.Values.Where(x => x.Name == name).FirstOrDefault();
        return model;
    }

    public static IEnumerable<LlmModel> GetModels(string? aiProvider, ModelCapability requiredCapability)
    {
        if(aiProvider == "anthropic")
        {
            return AnthropicModels.Values.Where(x => (x.Capabilities & requiredCapability) == requiredCapability);
        }
        if (aiProvider == "open-ai")
        {
            return OpenAiModels.Values.Where(x => (x.Capabilities & requiredCapability) == requiredCapability);
        }
        if (aiProvider == "ollama")
        {
            return Models.Values.Where(x => (x.Capabilities & requiredCapability) == requiredCapability);
        }
        return [];
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
