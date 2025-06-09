using TestBucket.Domain;
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

    private GlobalSettings? _globalSettings; 
    private ISetting[] _settings = [];
    private SettingsSection[] _sections = [];


    private readonly Dictionary<long, FieldValue> _fieldMap = [];
    private readonly Dictionary<long, ISetting> _settingsMap = [];

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
        if (model.Vendor == "alibaba-cloud")
        {
            return TbIcons.Brands.AlibabaCloud;
        }
        return null;
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

    private SettingContext? _context;
    private string? _category;

    protected override async Task OnInitializedAsync()
    {
        _globalSettings = await settingsProvider.LoadGlobalSettingsAsync();
    }

    private async Task OnAiProviderUrlChanged(string url)
    {
        if (_globalSettings is null)
        {
            return;
        }
        _globalSettings.AiProviderUrl = url;
        await settingsProvider.SaveGlobalSettingsAsync(_globalSettings);
        await LoadSettingsAsync();
    }
    private async Task OnAiProviderChanged(string provider)
    {
        if (_globalSettings is null)
        {
            return;
        }
        if (provider != _globalSettings.AiProvider)
        {
            _globalSettings.AiProvider = provider;

            if (provider == "azure-ai" || provider == "github-models")
            {
                _globalSettings.AiProviderUrl = "https://models.inference.ai.azure.com";
            }

            if (provider == "ollama")
            {
                _globalSettings.AiProviderUrl = Environment.GetEnvironmentVariable("TB_OLLAMA_BASE_URL") ?? "http://localhost:11434";
            }
            await settingsProvider.SaveGlobalSettingsAsync(_globalSettings);
            await LoadSettingsAsync();
        }
    }
    private async Task OnDefaultModelChanged(string model)
    {
        if (_globalSettings is null)
        {
            return;
        }
        _globalSettings.LlmModel = model;
        await settingsProvider.SaveGlobalSettingsAsync(_globalSettings);
        await LoadSettingsAsync();
    }
    private async Task OnClassificationModelChanged(string model)
    {
        if (_globalSettings is null)
        {
            return;
        }
        _globalSettings.LlmClassificationModel = model;
        await settingsProvider.SaveGlobalSettingsAsync(_globalSettings);
        await LoadSettingsAsync();
    }
    private async Task OnGeneratorModelChanged(string model)
    {
        if (_globalSettings is null)
        {
            return;
        }
        _globalSettings.LlmTestGenerationModel = model;
        await settingsProvider.SaveGlobalSettingsAsync(_globalSettings);
        await LoadSettingsAsync();
    }

    private async Task LoadSettingsAsync()
    {
        _context ??= await CreateSettingContextAsync();

        _settings = settingsManager.GetSettings(_context).Where(x => x.Metadata.Category.Name == _category).ToArray();
        _sections = _settings.Select(x => x.Metadata.Section).Distinct().ToArray();

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