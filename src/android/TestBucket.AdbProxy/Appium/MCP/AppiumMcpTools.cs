using System.ComponentModel;

using ModelContextProtocol.Server;

using TestBucket.AdbProxy.DeviceHandling;
using TestBucket.ResourceServer.Contracts;

namespace TestBucket.AdbProxy.Appium.MCP;

[McpServerToolType, Description("Tools to interact with appium (android phones etc)")]
public class AppiumMcpTools
{
    private readonly IAdbDeviceRepository _service;
    private readonly IResourceRegistry _resourceRegistry;
    private readonly AppiumConnectionPool _appiumConnectionPool;

    public AppiumMcpTools(IAdbDeviceRepository service, IResourceRegistry resourceRegistry, AppiumConnectionPool appiumConnectionPool)
    {
        _service = service;
        _resourceRegistry = resourceRegistry;
        _appiumConnectionPool = appiumConnectionPool;
    }

    /// <summary>
    /// Clicks on the specified text
    /// </summary>
    [McpServerTool(Name = "click_text"), Description("Clicks/taps on the text for the device specified by resourceId")]
    public string ClickText(
        [Description("The device")] string resourceId, 
        [Description("The text to click on ")] string text)
    {
        var appium = _appiumConnectionPool.GetOrCreate(resourceId);
        try
        {
            appium.ClickText(text);
            return $"OK. Clicked on '{text}'";
        }
        catch(Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Gets the list of connected devices.
    /// </summary>
    /// <returns>A list of connected devices.</returns>
    [McpServerTool(Name = "get_connected_appium_devices"), Description("Gets the ID for connected devices")]
    public Task<string[]> GetConnectedDevicesAsync()
    {
        return Task.FromResult(_service.Devices.Select(x=>x.DeviceId).ToArray());
    }

    /// <summary>
    /// Opens settings
    /// </summary>
    /// <param name="resourceId"></param>
    /// <returns></returns>
    [McpServerTool(Name = "open_settings"), Description("Opens the settings app")]
    public async Task<string> OpenSettingsAsync(string resourceId)
    {
        CancellationToken cancellationToken = default;

        var resource = await _resourceRegistry.GetResourceAsync(resourceId, cancellationToken);
        if (resource is null)
        {
            return $"Failed: No resource found with ID '{resourceId}'";
        }

        if (resource is AdbResource adbResource)
        {
            var text = await adbResource.ExecShellGetStringAsync(resourceId, "shell:am start com.android.settings", cancellationToken);

            return "OK. I have opened the settings app.";
        }
        return "This is not an ADB resource. Cannot use this tool for this device.";
    }

    /// <summary>
    /// Presses the home key
    /// </summary>
    /// <param name="resourceId"></param>
    /// <returns></returns>
    [McpServerTool(Name = "go_home"), Description("Goes to home")]
    public async Task<string> GoHomeAsync(string resourceId)
    {
        CancellationToken cancellationToken = default;

        var resource = await _resourceRegistry.GetResourceAsync(resourceId, cancellationToken);
        if(resource is null)
        {
            return $"Failed: No resource found with ID '{resourceId}'";
        }

        if(resource is AdbResource adbResource)
        {
            var text = await adbResource.ExecShellGetStringAsync(resourceId, "shell:input keyevent KEYCODE_HOME", cancellationToken);

            return "OK. Going to home screen.";
        }
        return "This is not an ADB resource. Cannot use this tool for this device.";
    }
}
