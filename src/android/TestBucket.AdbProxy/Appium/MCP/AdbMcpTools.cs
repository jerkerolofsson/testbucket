using System.ComponentModel;
using System.Text;

using ModelContextProtocol.Server;

using TestBucket.AdbProxy.DeviceHandling;
using TestBucket.ResourceServer.Contracts;

namespace TestBucket.AdbProxy.Appium.MCP;

[McpServerToolType, Description("Tools to interact with adb (android phones etc)")]
public class AdbMcpTools
{
    private readonly IAdbDeviceRepository _service;
    private readonly IResourceRegistry _resourceRegistry;

    public AdbMcpTools(IAdbDeviceRepository service, IResourceRegistry resourceRegistry)
    {
        _service = service;
        _resourceRegistry = resourceRegistry;
    }

    /// <summary>
    /// Gets the list of connected devices.
    /// </summary>
    /// <returns>A list of connected devices.</returns>
    [McpServerTool(Name = "get_connected_devices"), Description("Gets the ID for connected devices")]
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
    public async Task<string> OpenSettingsAsync([Description("The device")] string resourceId)
    {
        CancellationToken cancellationToken = default;

        var resource = await _resourceRegistry.GetResourceAsync(resourceId, cancellationToken);
        if (resource is null)
        {
            return $"Failed: No resource found with ID '{resourceId}'";
        }

        if (resource is AdbResource adbResource)
        {
            var text = await adbResource.ExecShellGetStringAsync("shell:am start com.android.settings", cancellationToken);

            return "OK. I have opened the settings app.";
        }
        return "This is not an ADB resource. Cannot use this tool for this device.";
    }

    /// <summary>
    /// Lists all packages
    /// </summary>
    /// <param name="resourceId"></param>
    /// <returns></returns>
    [McpServerTool(Name = "list_packages"), Description("Lists all packages / apps")]
    public async Task<string> ListPackagesAsync(string resourceId)
    {
        CancellationToken cancellationToken = default;

        var resource = await _resourceRegistry.GetResourceAsync(resourceId, cancellationToken);
        if (resource is null)
        {
            return $"Failed: No resource found with ID '{resourceId}'";
        }

        if (resource is AdbResource adbResource)
        {
            var packages = await adbResource.ListPackagesAsync(cancellationToken);

            var sb = new StringBuilder();
            sb.AppendLine("Here is a list of all installed packages:");
            foreach(var package in packages)
            {
                sb.AppendLine(package);
            }

            return sb.ToString();
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
            var text = await adbResource.ExecShellGetStringAsync("shell:input keyevent KEYCODE_HOME", cancellationToken);

            return "OK. Going to home screen.";
        }
        return "This is not an ADB resource. Cannot use this tool for this device.";
    }
}
