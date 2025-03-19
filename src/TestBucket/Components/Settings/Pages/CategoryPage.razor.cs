using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;

namespace TestBucket.Components.Settings.Pages;
public partial class CategoryPage
{
    [Parameter] public string TenantId { get; set; } = "";
    [Parameter] public string Category { get; set; } = "";

    private ISetting[] _settings = [];
    private SettingsSection[] _sections = [];

    private readonly Dictionary<long, FieldValue> _fieldMap = [];
    private readonly Dictionary<long, ISetting> _settingsMap = [];

    private async Task OnFieldChangedAsync(FieldValue fieldValue)
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var principal = authState.User;
        if (_settingsMap.TryGetValue(fieldValue.FieldDefinitionId, out var setting))
        {
            await setting.WriteAsync(principal, fieldValue);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var principal = authState.User;

        _settings = settingsManager.Settings.Where(x=>x.Metadata.Category.Name == Category).ToArray();
        _sections = _settings.Select(x => x.Metadata.Section).Distinct().ToArray();

        long id = 1;
        foreach(var setting in _settings)
        {
            var value = await setting.ReadAsync(principal);

            setting.Metadata.Id = id;
            value.FieldDefinitionId = id;
            value.FieldDefinition = setting.Metadata;

            _fieldMap[id] = value;
            _settingsMap[id] = setting;

            id++;
        }
    }
}