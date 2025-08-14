using System.ComponentModel;

using Microsoft.SemanticKernel;

using ModelContextProtocol.Server;

using OpenQA.Selenium.Appium.Android;

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


    [McpServerTool(Name = "swipe"), Description("Swipes in the specified direction")]
    public async Task<string> Swipe(
        [Description("The device")] string resourceId,
        [Description("Direction of swipe: down, up, left or right")] string direction)
    {
        try
        {
            await RunActionAsync(resourceId, async (appium) =>
            {
                await appium.SwipeAsync(direction, AppiumDefaults.SwipeDuration);
            });
            return $"OK. ";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Launches the specified app
    /// </summary>
    [McpServerTool(Name = "launch_app"), Description("Launches the specified app on the device specified by resourceId")]
    public async Task<string> LaunchApp(
        [Description("The device")] string resourceId,
        [Description("Name of the app")] string appName)
    {
        try
        {
            await RunActionAsync(resourceId, async (appium) =>
            {
                await appium.StartAppAsync(appName);
            });
            return $"OK. Started '{appName}'";
        }
        catch (Exception ex)
        {
            _appiumConnectionPool.Destroy(resourceId);

            return $"Error: {ex.Message}";
        }
    }


    /// <summary>
    /// Launches the specified app
    /// </summary>
    [McpServerTool(Name = "terminate_app"), Description("Terminates/kills the specified app")]
    public async Task<string> TerminateApp(
        [Description("The device")] string resourceId,
        [Description("Name of the app")] string appName)
    {
        try
        {
            await RunActionAsync(resourceId, async (appium) =>
            {
                await appium.TerminateAppAsync(appName);
            });
            return $"OK. Started '{appName}'";
        }
        catch (Exception ex)
        {
            _appiumConnectionPool.Destroy(resourceId);

            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Clicks on the specified text
    /// </summary>
    [McpServerTool(Name = "clear"), Description("Clears content for a UI element on the device specified by resourceId")]
    public async Task<string> Clear(
        [Description("The device")] string resourceId,
        [Description("Text, ID, content-description or other locator used to find the UI element")] string locator)
    {
        try
        {
            await RunActionAsync(resourceId, async (appium) =>
            {
                await appium.Clear(locator);
            });
            return $"OK. Cleared '{locator}'";
        }
        catch (Exception ex)
        {
            _appiumConnectionPool.Destroy(resourceId);

            return $"Error: {ex.Message}";
        }
    }

    [McpServerTool(Name = "press_back_key"), Description("Presses the back key")]
    public async Task<string> PressBackKey([Description("The device")] string resourceId)
    {
        try
        {
            await RunActionAsync(resourceId , (appium) =>
            {
                // https://developer.android.com/reference/android/view/KeyEvent#KEYCODE_BACK
                appium.PressKeyCode(new KeyEvent(4));
                return Task.CompletedTask;
            });
            return $"OK. ";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }


    [McpServerTool(Name = "swipe_to_find"), Description("Tries to find a UI element by swiping")]
    public async Task<string> SwipeToFind(
        [Description("The device")] string resourceId,
        [Description("Text, ID, content-description or other locator used to find the UI element")] string textOrId,
        [Description("Direction of swipe: down, up, left or right")] string direction)
    {
        try
        {
            await RunActionAsync(resourceId, async (appium) =>
            {
                await appium.SwipeToFindAsync(textOrId, direction);
            });
            return $"'{textOrId}' is visible";
        }
        catch
        {
            return $"'{textOrId}' is not visible";
        }
    }

    [McpServerTool(Name = "is_visible"), Description("Checks if a UI component is visible")]
    public async Task<string> IsVisible(
        [Description("The device")] string resourceId,
        [Description("Text, ID, content-description or other locator used to find the UI element")] string textOrId)
    {
        try
        {
            await RunActionAsync(resourceId, async (appium) =>
            {
                await appium.FindOrThrow(textOrId, _ => true, TimeSpan.FromSeconds(2));
            });
            return $"'{textOrId}' is visible";
        }
        catch
        {
            return $"'{textOrId}' is not visible";
        }
    }

    /// <summary>
    /// Clicks on the specified text
    /// </summary>
    [McpServerTool(Name = "toggle_checkbox_or_switch"), Description("Toggles a checkbox or switch UI component to the desired state")]
    public async Task<string> ToggleCheckboxOrSwitch(
        [Description("The device")] string resourceId,
        [Description("Text, ID, content-description or other locator used to find the UI element")] string locator,
        [Description("true: to check, false: to uncheck")] string state)
    {
        try
        {
            state ??= "false";
            bool boolState = state == "1" || state.ToLower() == "yes" || state.ToLower() == "true";   

            bool res = false;
            await RunActionAsync(resourceId, async (appium) =>
            {
                res = await appium.ToggleCheckbox(locator, boolState);
            });

            if (!res)
            {
                if (boolState)
                {
                    return $"'{locator}' was already checked";
                }
                else
                {
                    return $"'{locator}' was already unchecked";
                }
            }

            if (boolState)
            {
                return $"OK. '{locator}' is now checked";
            }
            else
            {
                return $"OK. '{locator}' is now unchecked";
            }
        }
        catch (Exception ex)
        {
            _appiumConnectionPool.Destroy(resourceId);

            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Clicks on the specified text
    /// </summary>
    [McpServerTool(Name = "click"), Description("Clicks/taps on a UI element on the device specified by resourceId")]
    public async Task<string> Click(
        [Description("The device")] string resourceId,
        [Description("Text, ID, content-description or other locator used to find the UI element")] string locator)
    {
        try
        {
            await RunActionAsync(resourceId, async (appium) =>
            {
                await appium.Click(locator);
            });
            return $"OK. Clicked on '{locator}'";
        }
        catch(Exception ex)
        {
            _appiumConnectionPool.Destroy(resourceId);

            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Clicks on the specified text
    /// </summary>
    [McpServerTool(Name = "send_text"), Description("Inputs text on the device specified by resourceId for the UI element indicated by the ")]
    public async Task<string> SendText(
        [Description("The device")] string resourceId,
        [Description("The text to send")] string text,
        [Description("Text, ID, content-description or other locator used to find the UI element")] string locator)
    {
        try
        {
            await RunActionAsync(resourceId, async (appium) =>
            {
                await appium.SendText(text, locator);
            });
            return $"OK. ";
        }
        catch (Exception ex)
        {
            _appiumConnectionPool.Destroy(resourceId);
            return $"Error: {ex.Message}";
        }
    }

    private async Task RunActionAsync(string resourceId, Func<AppiumConnection, Task> action)
    {
        try
        {
            var appium = _appiumConnectionPool.GetOrCreate(resourceId);
            await action(appium);
        }
        catch
        {
            _appiumConnectionPool.Destroy(resourceId);
            var appium = _appiumConnectionPool.GetOrCreate(resourceId);
            await action(appium);
        }
    }

}
