using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core.UnitTests.Xunit.Fakes;
using TestBucket.Traits.Xunit;

using Xunit.Sdk;
using Xunit.v3;

namespace TestBucket.Traits.Core.UnitTests.Xunit;

[UnitTest]
public class EnrichedTestAttributeTests
{
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


    [Fact]
    public void EnrichedTest_OnWindows_OperatingSystemPlatformIsWindows()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute(OperatingSystemCreator.CreateWindows());
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        // Note: The feature attribute is defined on the FakeXUnitTestMethod
        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.OperatingSystemPlatform).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("Windows", attachment.Value.AsString());
    }

    [Fact]
    public void EnrichedTest_OnLinux_OperatingSystemPlatformIsLinux()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute(OperatingSystemCreator.CreateLinux());
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        // Note: The feature attribute is defined on the FakeXUnitTestMethod
        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.OperatingSystemPlatform).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("Linux", attachment.Value.AsString());
    }


    [Fact]
    public void EnrichedTest_OnAndroid_OperatingSystemPlatformIsAndroid()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute(OperatingSystemCreator.CreateAndroid());
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        // Note: The feature attribute is defined on the FakeXUnitTestMethod
        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.OperatingSystemPlatform).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("Android", attachment.Value.AsString());
    }

    [Fact]
    public void EnrichedTest_OnMacOS_OperatingSystemPlatformIsMacOS()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute(OperatingSystemCreator.CreateMacOS());
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        // Note: The feature attribute is defined on the FakeXUnitTestMethod
        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.OperatingSystemPlatform).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("MacOS", attachment.Value.AsString());
    }

    [Fact]
    public void EnrichedTest_OnBrowser_OperatingSystemPlatformIsBrowser()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute(OperatingSystemCreator.CreateBrowser());
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        // Note: The feature attribute is defined on the FakeXUnitTestMethod
        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.OperatingSystemPlatform).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("Browser", attachment.Value.AsString());
    }

    [Fact]
    public void EnrichedTest_OnIOS_OperatingSystemPlatformIsIOS()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute(OperatingSystemCreator.CreateIOS());
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        // Note: The feature attribute is defined on the FakeXUnitTestMethod
        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.OperatingSystemPlatform).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("iOS", attachment.Value.AsString());
    }
    [Fact]
    public void EnrichedTest_OnCatalyst_OperatingSystemPlatformIsCatalyst()
    {
        var test = new Fakes.FakeXUnitTest();

        var attribute = new EnrichedTestAttribute(OperatingSystemCreator.CreateMacCatalyst());
        attribute.After(typeof(EnrichedTestAttribute).GetMethod(nameof(EnrichedTest_HasMachineNameAttachment))!, test);

        // Note: The feature attribute is defined on the FakeXUnitTestMethod
        var attachment = TestContext.Current.Attachments!.Where(x => x.Key == TestTraitNames.OperatingSystemPlatform).FirstOrDefault();
        Assert.NotNull(attachment.Key);
        Assert.Equal("MacCatalyst", attachment.Value.AsString());
    }
}
