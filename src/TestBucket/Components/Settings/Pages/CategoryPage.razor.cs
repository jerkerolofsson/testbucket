using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;
using TestBucket.Domain.Shared;

namespace TestBucket.Components.Settings.Pages;
public partial class CategoryPage
{
    [Parameter] public string TenantId { get; set; } = "";
    [Parameter] public string Category { get; set; } = "";

    /// <summary>
    /// Currently selected project
    /// </summary>
    [CascadingParameter] public TestProject? Project { get; set; }

    private ISetting[] _settings = [];
    private SettingsSection[] _sections = [];

    private readonly Dictionary<long, FieldValue> _fieldMap = [];
    private readonly Dictionary<long, ISetting> _settingsMap = [];

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

    private SettingContext? _context;
    private string? _category;

    protected override async Task OnParametersSetAsync()
    {
        _context ??= await CreateSettingContextAsync();

        if (_category != Category)
        {
            _category = Category;

            _settings = settingsManager.GetSettings(_context).Where(x => x.Metadata.Category.Name == Category).ToArray();
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

            this.StateHasChanged();
        }
    }
}