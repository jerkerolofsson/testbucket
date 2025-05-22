using TestBucket.Traits.Core.UnitTests.Xunit.Fakes;
using TestBucket.Traits.Xunit;

namespace TestBucket.Traits.Core.UnitTests.Xunit;

/// <summary>
/// Unit tests for <see cref="EnrichedTestAttribute"/>, verifying that it attaches expected metadata and environment information to test results.
/// </summary>
[UnitTest]
[EnrichedTest]
public class EnrichedTestAttributeTests
{
    /// <summary>
    /// Verifies that the machine name is added as an attachment to the test result.
    /// </summary>
    [Fact]
    public void EnrichedTest_HasMachineNameAttachment()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute();
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.MachineName).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal(Environment.MachineName, attachment.Value.AsString());
    }

    /// <summary>
    /// Verifies that the automation class name is added as an attachment to the test result.
    /// </summary>
    [Fact]
    public void EnrichedTest_HasAutomationClassNameAttachment()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute();
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        // Note: The feature attribute is defined on the FakeXUnitTestMethod
        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == AutomationTraitNames.ClassName).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("TestBucket.Traits.Core.UnitTests.Xunit.Fakes.FakeXunitTestClass", attachment.Value.AsString());
    }

    /// <summary>
    /// Verifies that a quality characteristic attachment from the class is added to the test result.
    /// </summary>
    [Fact]
    public void EnrichedTest_HasAttachmentFromClass()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute();
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        // Note: The feature attribute is defined on the FakeXUnitTestMethod
        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.QualityCharacteristic).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("Functional Suitability", attachment.Value.AsString());
    }

    /// <summary>
    /// Verifies that a custom attribute from the method is added as an attachment to the test result.
    /// </summary>
    [Fact]
    public void EnrichedTest_HasAttachmentFromMethod()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute();
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        // Note: The feature attribute is defined on the FakeXUnitTestMethod
        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == "CustomAttribute").FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("TheValue", attachment.Value.AsString());
    }

    /// <summary>
    /// Verifies that the test description is added as an attachment to the test result.
    /// </summary>
    [Fact]
    public void EnrichedTest_HasTestDescription()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute();
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        // Note: The feature attribute is defined on the FakeXUnitTestMethod
        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.TestDescription).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("Hello World", attachment.Value.AsString());
    }

    /// <summary>
    /// Verifies that the operating system platform is set to "Windows" when running on Windows.
    /// </summary>
    [Fact]
    public void EnrichedTest_OnWindows_OperatingSystemPlatformIsWindows()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute(OperatingSystemCreator.CreateWindows());
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.OperatingSystemPlatform).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("Windows", attachment.Value.AsString());
    }

    /// <summary>
    /// Verifies that the operating system platform is set to "Linux" when running on Linux.
    /// </summary>
    [Fact]
    public void EnrichedTest_OnLinux_OperatingSystemPlatformIsLinux()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute(OperatingSystemCreator.CreateLinux());
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.OperatingSystemPlatform).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("Linux", attachment.Value.AsString());
    }

    /// <summary>
    /// Verifies that the operating system platform is set to "Android" when running on Android.
    /// </summary>
    [Fact]
    public void EnrichedTest_OnAndroid_OperatingSystemPlatformIsAndroid()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute(OperatingSystemCreator.CreateAndroid());
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.OperatingSystemPlatform).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("Android", attachment.Value.AsString());
    }

    /// <summary>
    /// Verifies that the operating system platform is set to "MacOS" when running on MacOS.
    /// </summary>
    [Fact]
    public void EnrichedTest_OnMacOS_OperatingSystemPlatformIsMacOS()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute(OperatingSystemCreator.CreateMacOS());
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.OperatingSystemPlatform).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("MacOS", attachment.Value.AsString());
    }

    /// <summary>
    /// Verifies that the operating system platform is set to "Browser" when running in a browser environment.
    /// </summary>
    [Fact]
    public void EnrichedTest_OnBrowser_OperatingSystemPlatformIsBrowser()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute(OperatingSystemCreator.CreateBrowser());
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.OperatingSystemPlatform).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("Browser", attachment.Value.AsString());
    }

    /// <summary>
    /// Verifies that the operating system platform is set to "iOS" when running on iOS.
    /// </summary>
    [Fact]
    public void EnrichedTest_OnIOS_OperatingSystemPlatformIsIOS()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute(OperatingSystemCreator.CreateIOS());
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.OperatingSystemPlatform).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("iOS", attachment.Value.AsString());
    }

    /// <summary>
    /// Verifies that the operating system platform is set to "MacCatalyst" when running on Mac Catalyst.
    /// </summary>
    [Fact]
    public void EnrichedTest_OnCatalyst_OperatingSystemPlatformIsCatalyst()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute(OperatingSystemCreator.CreateMacCatalyst());
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.OperatingSystemPlatform).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("MacCatalyst", attachment.Value.AsString());
    }
}