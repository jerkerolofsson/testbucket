using System.Diagnostics;
using System.Threading;

using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;

using TestBucket.AdbProxy.Appium.PageSources;
using TestBucket.AdbProxy.DeviceHandling;
using TestBucket.AdbProxy.Models;
using TestBucket.Embeddings;
using TestBucket.ResourceServer.Contracts;

namespace TestBucket.AdbProxy.Appium;
public class AppiumConnection : IDisposable
{
    private readonly AndroidDriver _driver;
    private readonly TimeSpan _findWait = TimeSpan.FromSeconds(15);
    private readonly IAdbDeviceRepository _deviceRepository;
    private readonly IResourceRegistry _resourceRegistry;
    private readonly AdbDevice _device;

    public AppiumConnection(AdbDevice device, IAdbDeviceRepository deviceRepository, IResourceRegistry resourceRegistry)
    {
        ArgumentNullException.ThrowIfNull(device.AppiumUrl);
        _deviceRepository = deviceRepository;
        _resourceRegistry = resourceRegistry;
        _device = device;

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

    public void PressKeyCode(KeyEvent keyEvent)
    {
        _driver.PressKeyCode(keyEvent);
    }
    public void LongPressKeyCode(KeyEvent keyEvent)
    {
        _driver.LongPressKeyCode(keyEvent);
    }
    public bool IsKeyboardShown()
    {
        return _driver.IsKeyboardShown();
    }
    public void HideKeyboard()
    {
        _driver.HideKeyboard();
    }

    public async Task TerminateAppAsync(string appName, CancellationToken cancellationToken = default)
    {
        try
        {
            _driver.TerminateApp(appName);
        }
        catch(Exception)
        {
            var packageName = await ResolvePackageNameAsync(appName, cancellationToken);
            _driver.TerminateApp(packageName);
        }
    }

    /// <summary>
    /// Cached package names with embeddings
    /// </summary>
    private readonly List<PackageNameEmbedding> _packages = [];

    private async Task<string> ResolvePackageNameAsync(string appName, CancellationToken cancellationToken = default)
    {
        string? bestPackage = null;
        double? bestSimilarity = 0;

        var embedder = new LocalEmbedder();
        if (_packages.Count == 0)
        {
            var resource = await _resourceRegistry.GetResourceAsync(_device.DeviceId, cancellationToken);
            if (resource is AdbResource adbResource)
            {
                var packages = await adbResource.ListPackagesAsync(cancellationToken);

                var embeddings = await embedder.GenerateAsync(packages, null, cancellationToken);
                for(int i=0; i<Math.Min(embeddings.Count, packages.Length); i++)
                {
                    _packages.Add(new PackageNameEmbedding(embeddings[i].Vector, packages[i]));
                }
            }
        }

        var appNameEmbedding = (await embedder.GenerateAsync([appName], null, cancellationToken)).FirstOrDefault();
        if(appNameEmbedding is not null)
        {
            foreach(var package in _packages)
            {
                var similarity = CosineSimilarity.Calculate(appNameEmbedding.Vector, package.Embedding);
                if(bestPackage is null || bestSimilarity < similarity)
                {
                    bestPackage = package.PackageName;
                    bestSimilarity = similarity;
                }
            }
        }

        return bestPackage ?? appName;
    }

    public async Task StartAppAsync(string appName, CancellationToken cancellationToken = default)
    {
        try
        {
            _driver.ActivateApp(appName);
        }
        catch (Exception)
        {
            var packageName = await ResolvePackageNameAsync(appName, cancellationToken);
            _driver.ActivateApp(packageName);
        }
    }
    internal async Task Click(string elementQuery)
    {
        var start = Stopwatch.GetTimestamp();
        while (Stopwatch.GetElapsedTime(start) < _findWait)
        {
            var element = await FindFromPageSourceAsync(elementQuery, x => x.MatchThisOrAncestor(e=> e.Clickable == true));
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
            var element = await FindFromPageSourceAsync(elementQuery, _ => true);
            if (element is not null)
            {
                element.Clear();
                return;
            }
            await Task.Delay(500);
        }
        throw new Exception("Not found: " + elementQuery);
    }


    /// <summary>
    /// Returns true if the checkbox was changed, false if it remains
    /// </summary>
    /// <param name="elementQuery"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    internal async Task<bool> ToggleCheckbox(string elementQuery, bool state)
    {
        var start = Stopwatch.GetTimestamp();
        while (Stopwatch.GetElapsedTime(start) < _findWait)
        {
            var matchedNode = await FindNodeFromPageSourceAsync(elementQuery, x => x.MatchThisOrSibling(e => e.Checkable == true, 2));
            if (matchedNode is not null)
            {
                bool needToggle = matchedNode.Node.Checked != state;
                if (needToggle)
                {
                    var element = CreateXPathAndFindElement(matchedNode);
                    element.Click();
                    return true;
                }
                return false;
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
            var element = await FindFromPageSourceAsync(elementQuery, _ => true);
            if (element is not null)
            {
                element.SendKeys(text);
                return;
            }
            await Task.Delay(500);
        }
        throw new Exception("Not found: " + elementQuery);
    }
    internal async Task<AppiumElement?> Find(string elementQuery, Predicate<HierarchyNode> predicate)
    {
        var start = Stopwatch.GetTimestamp();
        while (Stopwatch.GetElapsedTime(start) < _findWait)
        {
            var element = await FindFromPageSourceAsync(elementQuery, predicate);
            if (element is not null)
            {
                return element;
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
    public async Task<AppiumElement?> FindFromPageSourceAsync(string textOrId, Predicate<HierarchyNode> predicate)
    {
        var source = _driver.PageSource;
        if (source.StartsWith('<'))
        {
            // Try to parse it as XML
            XmlPageSource pageSource = XmlPageSource.FromString(source);
            var elements = await pageSource.FindElementsAsync(textOrId, predicate);
            if (elements.Count > 0)
            {
                var bestElement = elements[0];
                return CreateXPathAndFindElement(bestElement);
            }
        }

        return null;
    }

    public async Task<MatchedHierarchyNode?> FindNodeFromPageSourceAsync(string textOrId, Predicate<HierarchyNode> predicate)
    {
        var source = _driver.PageSource;
        if (source.StartsWith('<'))
        {
            // Try to parse it as XML
            XmlPageSource pageSource = XmlPageSource.FromString(source);
            var elements = await pageSource.FindElementsAsync(textOrId, predicate);
            if (elements.Count > 0)
            {
                return elements[0];
            }
        }

        return null;
    }

    private AppiumElement CreateXPathAndFindElement(MatchedHierarchyNode bestElement)
    {
        // Create xpath query for the hierarchy
        List<string> xpathIndices = [];

        var element = bestElement.Node;
        while (element is not null)
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
