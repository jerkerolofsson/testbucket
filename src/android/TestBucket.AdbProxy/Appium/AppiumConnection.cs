using System.Diagnostics;

using Docker.DotNet.Models;

using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;

using TestBucket.AdbProxy.Appium.PageSources;
using TestBucket.AdbProxy.Models;

using static System.Net.Mime.MediaTypeNames;

namespace TestBucket.AdbProxy.Appium;
public class AppiumConnection : IDisposable
{
    private readonly AndroidDriver _driver;
    private readonly TimeSpan _findWait = TimeSpan.FromSeconds(15);
    public AppiumConnection(AdbDevice device)
    {
        ArgumentNullException.ThrowIfNull(device.AppiumUrl);

        var serverUri = new Uri(device.AppiumUrl);
        var driverOptions = new AppiumOptions()
        {
            AutomationName = AutomationName.AndroidUIAutomator2,
            PlatformName = "Android",
            DeviceName = device.DeviceId,
        };

        var hostnameFromContainer = Environment.GetEnvironmentVariable("TB_APPIUM_HOSTNAME_FOR_ADBPROXY") ?? "host.docker.internal";
        driverOptions.AddAdditionalAppiumOption("udid", device.DeviceId ?? "");
        driverOptions.AddAdditionalAppiumOption("remoteAdbHost", hostnameFromContainer);

        driverOptions.AddAdditionalAppiumOption("appPackage", "com.android.settings");
        driverOptions.AddAdditionalAppiumOption("appActivity", ".Settings");
        driverOptions.AddAdditionalAppiumOption("appium:newCommandTimeout", "300");

        // NoReset assumes the app com.google.android is preinstalled on the emulator
        driverOptions.AddAdditionalAppiumOption("noReset", true);

        _driver = new AndroidDriver(serverUri, driverOptions, TimeSpan.FromSeconds(240));
        var timeouts = _driver.Manage().Timeouts();
        timeouts.ImplicitWait = TimeSpan.FromSeconds(15);
    }

    public bool IsKeyboardShown()
    {
        return _driver.IsKeyboardShown();
    }
    public void HideKeyboard()
    {
        _driver.HideKeyboard();
    }

    public void TerminateApp(string appName)
    {
        _driver.TerminateApp(appName);
    }

    public void StartApp(string appName)
    {
        _driver.ActivateApp(appName);
    }
    internal async Task Click(string elementQuery)
    {
        var start = Stopwatch.GetTimestamp();
        while (Stopwatch.GetElapsedTime(start) < _findWait)
        {
            var element = FindFromPageSource(elementQuery);
            if (element is not null)
            {
                element.Click();
                return;
            }
            await Task.Delay(500);
        }
        throw new Exception("Not found: " + elementQuery);
    }
    internal async Task Clear(string elementQuery)
    {
        var start = Stopwatch.GetTimestamp();
        while (Stopwatch.GetElapsedTime(start) < _findWait)
        {
            var element = FindFromPageSource(elementQuery);
            if (element is not null)
            {
                element.Clear();
                return;
            }
            await Task.Delay(500);
        }
        throw new Exception("Not found: " + elementQuery);
    }
    internal async Task SendText(string text, string elementQuery)
    {
        var start = Stopwatch.GetTimestamp();
        while (Stopwatch.GetElapsedTime(start) < _findWait)
        {
            var element = FindFromPageSource(elementQuery);
            if (element is not null)
            {
                element.SendKeys(text);
                return;
            }
            await Task.Delay(500);
        }
        throw new Exception("Not found: " + elementQuery);
    }
    internal async Task Find(string elementQuery)
    {
        var start = Stopwatch.GetTimestamp();
        while (Stopwatch.GetElapsedTime(start) < _findWait)
        {
            var element = FindFromPageSource(elementQuery);
            if (element is not null)
            {
                return;
            }
            await Task.Delay(500);
        }
        throw new Exception("Not found: " + elementQuery);
    }

    public void FindById(string elementId)
    {
        _driver.FindElement(By.Id(elementId));
    }
    public void FindByText(string text)
    {
        _driver.FindElement(By.XPath($"//*[@text='{text}']"));
    }
    public void ClickId(string elementId)
    {
        _driver.FindElement(By.Id(elementId)).Click();
    }
    public void ClickText(string text)
    {
        _driver.FindElement(By.XPath($"//*[@text='{text}']")).Click();
    }

    public void Dispose()
    {
        _driver.Dispose();
    }

    /// <summary>
    /// Finds an element from the page source by text,ID, content description, by finding exact or partial matches and ranking them.
    /// Constructing a positional indexing xpath based on the result and returning that AppiumElement
    /// </summary>
    /// <param name="textOrId"></param>
    /// <returns></returns>
    public AppiumElement? FindFromPageSource(string textOrId)
    {
        var source = _driver.PageSource;
        if (source.StartsWith('<'))
        {
            // Try to parse it as XML
            XmlPageSource pageSource = XmlPageSource.FromString(source);
            var elements = pageSource.FindElements(textOrId);
            if (elements.Count > 0)
            {
                var bestElement = elements[0];

                // Create xpath query for the hierarchy
                List<string> xpathIndices = [];

                var element = bestElement.Node;
                while(element is not null)
                {
                    //var xpathIndex = element.Inde; // xpath uses 1-based indices
                    xpathIndices.Add($"*[@index={element.Index}]");

                    element = element.Parent;
                }

                xpathIndices.Reverse();
                string xpath = "/*/" + string.Join("/", xpathIndices);
                return _driver.FindElement(By.XPath(xpath));
            }
        }

        return null;
    }

}
