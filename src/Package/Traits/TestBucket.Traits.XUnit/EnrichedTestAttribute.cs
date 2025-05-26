using System.Reflection;

using TestBucket.Traits.Core.XmlDoc;

using Xunit;


namespace TestBucket.Traits.Xunit
{
    /// <summary>
    /// Adds metadata to a test, including test description from XML comments if the csproj defines
    /// &lt;GenerateDocumentationFile&gt;true&lt;/GenerateDocumentationFile&gt;
    /// </summary>
    public class EnrichedTestAttribute : BeforeAfterTestAttribute
    {
        private readonly OperatingSystemInformation _operatingSystem;

        /// <summary>
        /// Mock constructor for a specific operating system
        /// </summary>
        /// <param name="operatingSystem"></param>
        internal EnrichedTestAttribute(OperatingSystemInformation operatingSystem)
        {
            _operatingSystem = operatingSystem;
        }

        /// <summary>
        /// Adds metadata to a test, including test description from XML comments if the csproj defines
        /// &lt;GenerateDocumentationFile&gt;true&lt;/GenerateDocumentationFile&gt;
        /// </summary>
        public EnrichedTestAttribute()
        {
            _operatingSystem = new OperatingSystemInformation();
        }

        private void AddAttachmentIfNotExists(string key, string value)
        {
            if (TestContext.Current.Attachments?.Any(x => x.Key == key) == true)
            {
                return;
            }
            TestContext.Current.AddAttachment(key, value);
        }

        public override void After(MethodInfo methodUnderTest, IXunitTest test)
        {
            EnrichWithTraitAttachmentAttributes(test);
            EnrichWithOperatingSystem();

            // Add environment attachments
            AddAttachmentIfNotExists(TestTraitNames.MachineName, Environment.MachineName);

            // Add attachments related to the test running (class, method, and assembly)
            var className = test.TestMethod.TestClass.Class.Name;
            var namespaceName = test.TestMethod.TestClass.Class.Namespace;
            var methodName = test.TestMethod.MethodName;
            var assembly = test.TestMethod.TestClass.Class.Assembly;
            var assemblyName = assembly.GetName().Name ?? "";

            AddAttachmentIfNotExists(AutomationTraitNames.ClassName, $"{namespaceName}.{className}");
            AddAttachmentIfNotExists(AutomationTraitNames.MethodName, methodName);
            AddAttachmentIfNotExists(AutomationTraitNames.Assembly, assemblyName);

            try
            {
                AddTestDescriptionFromXmlDoc(className, namespaceName, methodName, assembly);
            }
            catch
            {
                // Could be IO errors, don't stop the test
            }
        }

        private void AddTestDescriptionFromXmlDoc(string className, string? namespaceName, string methodName, Assembly assembly)
        {
            // Lookup XML documentation, if present
            var assemblyPath = Path.GetDirectoryName(assembly.Location);
            if (assemblyPath is not null)
            {
                var assemblyPathName = Path.GetFileNameWithoutExtension(assembly.Location);
                var assemblyXmlDocPath = Path.Combine(assemblyPath, assemblyPathName + ".xml");
                if (File.Exists(assemblyXmlDocPath))
                {
                    // Note: There will be issues here if there are multiple methods with different parameters
                    var result = XmlDocSerializer.ParseFile(assemblyXmlDocPath);
                    var fullName = $"{namespaceName}.{className}.{methodName}";
                    var methodXmlDoc = result.Methods.Where(x => x.Name == fullName).FirstOrDefault();
                    if (methodXmlDoc is null)
                    {
                        // Theory will have arguments, we need to strip them
                        fullName = $"{namespaceName}.{className}.{StripSignature(methodName)}";
                        methodXmlDoc = result.Methods.Where(x => StripSignature(x.Name) == fullName).FirstOrDefault();
                    }
                    if (methodXmlDoc is not null)
                    {
                        var markdown = new XmlDocMarkdownBuilder().AddMethod(result, methodXmlDoc).Build();
                        AddAttachmentIfNotExists(TestTraitNames.TestDescription, markdown);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the parans and arguments
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private string StripSignature(string methodName)
        {
            var p = methodName.IndexOf('(');
            if(p > 0)
            {
                return methodName[0..p];
            }
            return methodName;
        }

        private void EnrichWithOperatingSystem()
        {
            if (TestContext.Current.Attachments?.Any(x => x.Key == TestTraitNames.OperatingSystemPlatform) == true)
            {
                return;
            }

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

        private void EnrichWithTraitAttachmentAttributes(IXunitTest test)
        {
            var testMethodDescriptionAttributes = test.TestMethod.TestClass.Class.GetCustomAttributes<TraitAttachmentPropertyAttribute>(true);
            var testClassDescriptionAttributes = test.TestMethod.Method.GetCustomAttributes<TraitAttachmentPropertyAttribute>(true).ToList();
            foreach (var attr in testClassDescriptionAttributes)
            {
                AddAttachmentIfNotExists(attr.Name, attr.Value);
            }
            foreach (var attr in testMethodDescriptionAttributes)
            {
                AddAttachmentIfNotExists(attr.Name, attr.Value);
            }
        }
    }
}
