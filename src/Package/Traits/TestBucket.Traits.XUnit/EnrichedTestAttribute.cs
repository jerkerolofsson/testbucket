using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;

using Xunit;


namespace TestBucket.Traits.Xunit
{
    public class EnrichedTestAttribute : BeforeAfterTestAttribute
    {
        private readonly OperatingSystemInformation _operatingSystem;

        internal EnrichedTestAttribute(OperatingSystemInformation operatingSystem)
        {
            _operatingSystem = operatingSystem;
        }

        public EnrichedTestAttribute()
        {
            _operatingSystem = new OperatingSystemInformation();
        }

        public override void After(MethodInfo methodUnderTest, IXunitTest test)
        {
            EnrichWithTraitAttachmentAttributes(test);
            EnrichWithOperatingSystem();

            TestContext.Current.AddAttachment(TestTraitNames.MachineName, Environment.MachineName);

            var className = test.TestMethod.TestClass.Class.Name;
            var namespaceName = test.TestMethod.TestClass.Class.Namespace;

            TestContext.Current.AddAttachment(AutomationTraitNames.ClassName, $"{namespaceName}.{className}");
            TestContext.Current.AddAttachment(AutomationTraitNames.MethodName, test.TestMethod.MethodName);

            var assemblyName = test.TestMethod.TestClass.Class.Assembly.GetName().Name ?? "";
            TestContext.Current.AddAttachment(AutomationTraitNames.Assembly, assemblyName);
        }

        private void EnrichWithOperatingSystem()
        {
            if (_operatingSystem.IsWindows)
            {
                TestContext.Current.AddAttachment(TestTraitNames.OperatingSystemPlatform, "Windows");
            }
            else if (_operatingSystem.IsMacOS)
            {
                TestContext.Current.AddAttachment(TestTraitNames.OperatingSystemPlatform, "MacOS");
            }
            else if (_operatingSystem.IsLinux)
            {
                TestContext.Current.AddAttachment(TestTraitNames.OperatingSystemPlatform, "Linux");
            }
            else if (_operatingSystem.IsIOS)
            {
                TestContext.Current.AddAttachment(TestTraitNames.OperatingSystemPlatform, "iOS");
            }
            else if (_operatingSystem.IsAndroid)
            {
                TestContext.Current.AddAttachment(TestTraitNames.OperatingSystemPlatform, "Android");
            }
            else if (_operatingSystem.IsBrowser)
            {
                TestContext.Current.AddAttachment(TestTraitNames.OperatingSystemPlatform, "Browser");
            }
            else if (_operatingSystem.IsMacCatalyst)
            {
                TestContext.Current.AddAttachment(TestTraitNames.OperatingSystemPlatform, "MacCatalyst");
            }
        }

        private static void EnrichWithTraitAttachmentAttributes(IXunitTest test)
        {
            var testMethodDescriptionAttributes = test.TestMethod.TestClass.Class.GetCustomAttributes<TraitAttachmentPropertyAttribute>(true);
            var testClassDescriptionAttributes = test.TestMethod.Method.GetCustomAttributes<TraitAttachmentPropertyAttribute>(true).ToList();
            foreach (var attr in testClassDescriptionAttributes)
            {
                TestContext.Current.AddAttachment(attr.Name, attr.Value);
            }
            foreach (var attr in testMethodDescriptionAttributes)
            {
                TestContext.Current.AddAttachment(attr.Name, attr.Value);
            }
        }
    }
}
