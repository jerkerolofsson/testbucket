using System;

using TestBucket.Domain;
using TestBucket.Domain.AI;
using TestBucket.Domain.AI.Models;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;

namespace TestBucket.Components.Settings.Pages;
public partial class AISettingsPage
{
    [Parameter] public string TenantId { get; set; } = "";

    /// <summary>
    /// Currently selected project
    /// </summary>
    [CascadingParameter] public TestProject? Project { get; set; }

    private ISetting[] _settings = [];
    //private SettingsSection[] _sections = [];
    private SettingContext? _context;
    private string? _category;

    private readonly Dictionary<long, FieldValue> _fieldMap = [];
    private readonly Dictionary<long, ISetting> _settingsMap = [];

    private string? _defaultModel;
    private string? _classificationModel;
    private string? _generatorModel;
    private string? _aiProvider;
    private string? _aiProviderUrl;

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
        if (model.Vendor == "microsoft")
        {
            return Icons.Custom.Brands.Microsoft;
        }
        if (model.Vendor == "nvidia")
        {
            return TbIcons.Brands.Nvidia;
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

        return new SettingContext { Principal = principal, ProjectId = Project?.Id };
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

            if (provider == "azure-ai" || provider == "github-models")
            {
                await OnAiProviderUrlChanged("https://models.inference.ai.azure.com");
            }

            if (provider == "ollama")
            {
                await OnAiProviderUrlChanged(Environment.GetEnvironmentVariable("TB_OLLAMA_BASE_URL") ?? "http://localhost:11434");
            }
        }
    }
 
    private async Task OnClassificationModelChanged(string model)
    {
        _classificationModel = model;

        if (_context is null)
        {
            return;
        }

        var setting = settingsManager.GetSettingByName(_context, "ai-classification-model");
        if(setting is not null)
        {
            await setting.WriteAsync(_context, new FieldValue { StringValue = model, FieldDefinitionId = 0 });
        }
    }

    private async Task OnGeneratorModelChanged(string model)
    {
        _generatorModel = model;

        if (_context is null)
        {
            return;
        }

        var setting = settingsManager.GetSettingByName(_context, "ai-test-generator-model");
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
            "azure-ai" => TbIcons.Brands.AzureAI,
            "github-models" => Icons.Custom.Brands.GitHub,
            _ => null
        };
    }

    private async Task LoadSettingsAsync()
    {
        _context ??= await CreateSettingContextAsync();

        var defaultModel = settingsManager.GetSettingByName(_context, "ai-default-model");
        var generatorModel = settingsManager.GetSettingByName(_context, "ai-test-generator-model");
        var classificationModel = settingsManager.GetSettingByName(_context, "ai-classification-model");
        var provider = settingsManager.GetSettingByName(_context, "ai-provider");
        var providerUrl = settingsManager.GetSettingByName(_context, "ai-provider-url");

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
        if (classificationModel is not null)
        {
            _classificationModel = (await classificationModel.ReadAsync(_context)).StringValue;
        }
        if (generatorModel is not null)
        {
            _generatorModel = (await generatorModel.ReadAsync(_context)).StringValue;
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