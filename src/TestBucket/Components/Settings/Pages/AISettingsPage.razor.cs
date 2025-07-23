using System;

using TestBucket.Domain;
using TestBucket.Domain.AI;
using TestBucket.Domain.AI.Models;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;
using TestBucket.Domain.Shared;

namespace TestBucket.Components.Settings.Pages;
public partial class AISettingsPage
{
    [Parameter] public string TenantId { get; set; } = "";

    /// <summary>
    /// Currently selected project
    /// </summary>
    [CascadingParameter] public TestProject? Project { get; set; }

    private string McpUrl => $"{TenantId}/Settings/Categories/MCP";

    private ISetting[] _settings = [];
    private SettingContext? _context;
    private string? _category;

    private readonly Dictionary<long, FieldValue> _fieldMap = [];
    private readonly Dictionary<long, ISetting> _settingsMap = [];

    private string? _defaultModel;
    private string? _embeddingModel;
    private string? _aiProvider;
    private string? _aiProviderUrl;
    private string? _embeddingAiProvider;
    private string? _embeddingAiProviderUrl;

    private string? GetModelIcon(string? name)
    {
        if(name is null)
        {
            return null;
        }
        var model = LlmModels.GetModelByName(name);
        if(model is not null)
        {
            return GetModelIcon(model);
        }
        return null;
    }
    private string? GetModelIcon(LlmModel model)
    {
        if(model.Icon is not null)
        {
            return model.Icon;
        }
        if (model.Vendor == "meta")
        {
            return TbIcons.Brands.Meta;
        }
        if (model.Vendor == "qwen")
        {
            return TbIcons.Brands.Qwen;
        }
        if (model.Vendor == "deepseek")
        {
            return TbIcons.Brands.Deepseek;
        }
        if (model.Vendor == "anthropic")
        {
            return TbIcons.Brands.Claude;
        }
        if (model.Vendor == "microsoft")
        {
            return Icons.Custom.Brands.Microsoft;
        }
        if (model.Vendor == "nvidia")
        {
            return TbIcons.Brands.Nvidia;
        }
        if (model.Vendor == "mistral")
        {
            return TbIcons.Brands.Mistral;
        }
        if (model.Vendor == "alibaba-cloud")
        {
            return TbIcons.Brands.AlibabaCloud;
        }
        return null;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadSettingsAsync();
    }

    private async Task OnFieldChangedAsync(FieldValue fieldValue)
    {
        if (_settingsMap.TryGetValue(fieldValue.FieldDefinitionId, out var setting))
        {
            _context ??= await CreateSettingContextAsync();
            await setting.WriteAsync(_context, fieldValue);
        }
    }

    private async Task<SettingContext> CreateSettingContextAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var principal = authState.User;

        return new SettingContext { Principal = principal, ProjectId = Project?.Id, TenantId = principal.GetTenantIdOrThrow() };
    }
    private async Task OnEmbeddingAiProviderUrlChanged(string url)
    {
        _embeddingAiProviderUrl = url;

        if (_context is null)
        {
            return;
        }

        var setting = settingsManager.GetSettingByName(_context, "embedding-ai-provider-url");
        if (setting is not null)
        {
            await setting.WriteAsync(_context, new FieldValue { StringValue = url, FieldDefinitionId = 0 });
        }
    }
    private async Task OnAiProviderUrlChanged(string url)
    {
        _aiProviderUrl = url;

        if (_context is null)
        {
            return;
        }

        var setting = settingsManager.GetSettingByName(_context, "ai-provider-url");
        if (setting is not null)
        {
            await setting.WriteAsync(_context, new FieldValue { StringValue = url, FieldDefinitionId = 0 });
        }
    }
    private async Task OnAiProviderChanged(string provider)
    {

        if (_context is null)
        {
            return;
        }

        if (provider != _aiProvider)
        {
            _aiProvider = provider;
            var setting = settingsManager.GetSettingByName(_context, "ai-provider");
            if (setting is not null)
            {
                await setting.WriteAsync(_context, new FieldValue { StringValue = provider, FieldDefinitionId = 0 });
            }

            if(provider == "open-ai")
            {
                await OnAiProviderUrlChanged("https://api.openai.com/v1");
            }

            if (provider == "azure-ai" || provider == "github-models")
            {
                await OnAiProviderUrlChanged("https://models.inference.ai.azure.com");
            }

            if (provider == "anthropic")
            {
                await OnAiProviderUrlChanged("https://api.anthropic.com/v1");
            }

            if (provider == "ollama")
            {
                await OnAiProviderUrlChanged(Environment.GetEnvironmentVariable("TB_OLLAMA_BASE_URL") ?? "http://localhost:11434");
            }
        }
    }
    private async Task OnEmbeddingAiProviderChanged(string provider)
    {
        if (_context is null)
        {
            return;
        }

        if (provider != _embeddingAiProvider)
        {
            _embeddingAiProvider = provider;
            var setting = settingsManager.GetSettingByName(_context, "embedding-ai-provider");
            if (setting is not null)
            {
                await setting.WriteAsync(_context, new FieldValue { StringValue = provider, FieldDefinitionId = 0 });
            }
           
            if (provider == "ollama")
            {
                await OnEmbeddingAiProviderUrlChanged(Environment.GetEnvironmentVariable("TB_OLLAMA_BASE_URL") ?? "http://localhost:11434");
            }
        }
    }

    private async Task OnEmbeddingModelChanged(string model)
    {
        _embeddingModel = model;

        if (_context is null)
        {
            return;
        }

        var setting = settingsManager.GetSettingByName(_context, "ai-embedding-model");
        if (setting is not null)
        {
            await setting.WriteAsync(_context, new FieldValue { StringValue = model, FieldDefinitionId = 0 });
        }
    }
    private async Task OnDefaultModelChanged(string model)
    {
        _defaultModel = model;

        if (_context is null)
        {
            return;
        }

        var setting = settingsManager.GetSettingByName(_context, "ai-default-model");
        if (setting is not null)
        {
            await setting.WriteAsync(_context, new FieldValue { StringValue = model, FieldDefinitionId = 0 });

            // Reload settings as changing the model may change billing info
            await LoadSettingsAsync();
        }
    }

    public string? GetProviderIcon(string? name)
    {
        if(name is null)
        {
            return null;
        }
        return name switch
        {
            "ollama" => TbIcons.Brands.Ollama,
            "open-ai" => TbIcons.Brands.OpenAI,
            "azure-ai" => TbIcons.Brands.AzureAI,
            "github-models" => Icons.Custom.Brands.GitHub,
            "anthropic" => TbIcons.Brands.Claude,
            _ => null
        };
    }

    private async Task LoadSettingsAsync()
    {
        _context ??= await CreateSettingContextAsync();

        var defaultModel = settingsManager.GetSettingByName(_context, "ai-default-model");
        var generatorModel = settingsManager.GetSettingByName(_context, "ai-test-generator-model");
        var embeddingModel = settingsManager.GetSettingByName(_context, "ai-embedding-model");
        var provider = settingsManager.GetSettingByName(_context, "ai-provider");
        var providerUrl = settingsManager.GetSettingByName(_context, "ai-provider-url");
        var embeddingProvider = settingsManager.GetSettingByName(_context, "embedding-ai-provider");
        var embeddingProviderUrl = settingsManager.GetSettingByName(_context, "embedding-ai-provider-url");

        if (provider is not null)
        {
            _aiProvider = (await provider.ReadAsync(_context)).StringValue;
        }
        if (providerUrl is not null)
        {
            _aiProviderUrl = (await providerUrl.ReadAsync(_context)).StringValue;
        }
        if (defaultModel is not null)
        {
            _defaultModel = (await defaultModel.ReadAsync(_context)).StringValue;
        }
        if (embeddingModel is not null)
        {
            _embeddingModel = (await embeddingModel.ReadAsync(_context)).StringValue;
        }
        if (embeddingProvider is not null)
        {
            _embeddingAiProvider = (await embeddingProvider.ReadAsync(_context)).StringValue;
        }
        if (embeddingProviderUrl is not null)
        {
            _embeddingAiProviderUrl = (await embeddingProviderUrl.ReadAsync(_context)).StringValue;
        }


        _settings = settingsManager.GetSettings(_context).Where(x => x.Metadata.Category.Name == _category).ToArray();
        //_sections = _settings.Select(x => x.Metadata.Section).Distinct().ToArray();

        long id = 1;
        foreach (var setting in _settings)
        {
            var value = await setting.ReadAsync(_context);

            setting.Metadata.Id = id;
            value.FieldDefinitionId = id;
            value.FieldDefinition = setting.Metadata;

            _fieldMap[id] = value;
            _settingsMap[id] = setting;

            id++;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_category != "AI")
        {
            _category = "AI";

            await LoadSettingsAsync();

            this.StateHasChanged();
        }
    }
}