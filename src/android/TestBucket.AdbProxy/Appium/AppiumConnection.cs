using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;

using TestBucket.AdbProxy.Models;

namespace TestBucket.AdbProxy.Appium;
public class AppiumConnection : IDisposable
{
    private readonly AndroidDriver _driver;

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

        //driverOptions.AddAdditionalAppiumOption("appPackage", "com.android.settings");
        //driverOptions.AddAdditionalAppiumOption("appActivity", ".Settings");
        // NoReset assumes the app com.google.android is preinstalled on the emulator
        //driverOptions.AddAdditionalAppiumOption("noReset", true);

        _driver = new AndroidDriver(serverUri, driverOptions, TimeSpan.FromSeconds(180));
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    public void StartApp(string appName)
    {
        _driver.ActivateApp(appName);
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
}
