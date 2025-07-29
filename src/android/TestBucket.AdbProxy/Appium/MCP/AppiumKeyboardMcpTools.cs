using System.ComponentModel;

using ModelContextProtocol.Server;

using TestBucket.AdbProxy.DeviceHandling;
using TestBucket.ResourceServer.Contracts;

namespace TestBucket.AdbProxy.Appium.MCP;

[McpServerToolType, Description("Tools to interact with appium (android phones etc)")]
public class AppiumKeyboardMcpTools
{
    private readonly IAdbDeviceRepository _service;
    private readonly IResourceRegistry _resourceRegistry;
    private readonly AppiumConnectionPool _appiumConnectionPool;

    public AppiumKeyboardMcpTools(IAdbDeviceRepository service, IResourceRegistry resourceRegistry, AppiumConnectionPool appiumConnectionPool)
    {
        _service = service;
        _resourceRegistry = resourceRegistry;
        _appiumConnectionPool = appiumConnectionPool;
    }

    /// <summary>
    /// Launches the specified app
    /// </summary>
    [McpServerTool(Name = "hide_keyboard"), Description("Hides the keyboard")]
    public string HideKeyboard([Description("The device")] string resourceId)
    {
        try
        {
            RunAction(resourceId, (appium) =>
            {
                appium.HideKeyboard();
            });
            return $"OK. The keyboard is hidden.";
        }
        catch (Exception ex)
        {
            _appiumConnectionPool.Destroy(resourceId);

            return $"Error: {ex.Message}";
        }
    }
    
    private void RunAction(string resourceId, Action<AppiumConnection> action)
    {
        try
        {
            var appium = _appiumConnectionPool.GetOrCreate(resourceId);
            action(appium);
        }
        catch
        {
            _appiumConnectionPool.Destroy(resourceId);
            var appium = _appiumConnectionPool.GetOrCreate(resourceId);
            action(appium);
        }
    }
}
