using TestBucket.AdbProxy.Models;

namespace TestBucket.Servers.AdbProxy.Components.Pages;

public partial class Index
{
    private InformSettings? _settings;

    protected override async Task OnInitializedAsync()
    {
        _settings = await informer.LoadInformSettingsAsync();
    }

    private async Task OnInformUrlChangedAsync(string url)
    {
        if (_settings is not null)
        {
            _settings.Url = url;
            await informer.SaveInformSettingsAsync(_settings);
        }
    }

    private async Task OnAuthHeaderChangedAsync(string authHeader)
    {
        if (_settings is not null)
        {
            _settings.AuthHeader = authHeader;
            await informer.SaveInformSettingsAsync(_settings);
        }
    }
}
