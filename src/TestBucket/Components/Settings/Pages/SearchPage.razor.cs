using Microsoft.CodeAnalysis;

using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;

namespace TestBucket.Components.Settings.Pages;
public partial class SearchPage
{
    [Parameter] public string TenantId { get; set; } = "";
    [SupplyParameterFromQuery(Name = "q")] public string SearchPhrase { get; set; } = "";

    private ISetting[] _settings = [];
    private SettingsSection[] _sections = [];

    private readonly Dictionary<long, FieldValue> _fieldMap = [];
    private readonly Dictionary<long, ISetting> _settingsMap = [];

    /// <summary>
    /// Currently selected project
    /// </summary>
    [CascadingParameter] public TestProject? Project { get; set; }

    /// <summary>
    /// Creates a context that holds information about the current user and project
    /// </summary>
    /// <returns></returns>
    private async Task<SettingContext> CreateSettingContextAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var principal = authState.User;

        return new SettingContext { Principal = principal, ProjectId = Project?.Id };
    }

    /// <summary>
    /// Field changed, update the setting
    /// </summary>
    /// <param name="fieldValue"></param>
    /// <returns></returns>
    private async Task OnFieldChangedAsync(FieldValue fieldValue)
    {
        if (_settingsMap.TryGetValue(fieldValue.FieldDefinitionId, out var setting))
        {
            var context = await CreateSettingContextAsync();
            await setting.WriteAsync(context, fieldValue);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        var context = await CreateSettingContextAsync();

        _settings = settingsManager.Search(context,SearchPhrase);
        _sections = _settings.Select(x => x.Metadata.Section).Distinct().ToArray();

        long id = 1;
        foreach(var setting in _settings)
        {
            var value = await setting.ReadAsync(context);

            setting.Metadata.Id = id;
            value.FieldDefinitionId = id;
            value.FieldDefinition = setting.Metadata;

            _fieldMap[id] = value;
            _settingsMap[id] = setting;

            id++;
        }
    }
}