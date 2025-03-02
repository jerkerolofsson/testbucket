using System.Globalization;
using System.Text;
using System.Xml.Linq;

using TestBucket.Domain.Testing.Formats.Dtos;
using TestBucket.Domain.Testing.Formats;
using TestBucket.Domain.Testing.Models;

namespace Brightest.Testing.Domain.Formats.Xml.XUnit
{
    public class XUnitSerializer : ITestResultSerializer
    {
        private static readonly CultureInfo _culture = new CultureInfo("en-US");

        private static readonly Dictionary<TestTraitType, string> _propertyNames = new Dictionary<TestTraitType, string>
        {
            [TestTraitType.Version] = "version",
            [TestTraitType.Ci] = "ci",
            [TestTraitType.Commit] = "commit",
            [TestTraitType.TestCategory] = "category",
            [TestTraitType.Tag] = "tag",
            [TestTraitType.Area] = "area",
            [TestTraitType.Project] = "project",
            [TestTraitType.TestedSoftwareVersion] = "sw",
            [TestTraitType.TestedHardwareVersion] = "hw",
        };

        /// <summary>
        /// Attributes not serialized as properties
        /// </summary>
        private static readonly HashSet<TestTraitType> _nativeAttributes =
        [
            TestTraitType.ExternalId,
            TestTraitType.Name,
            TestTraitType.TestResult,
            TestTraitType.ClassName,
            TestTraitType.Method,
            TestTraitType.Module,
            TestTraitType.Message,
            TestTraitType.Line,
            TestTraitType.Duration,
        ];

        public string Serialize(TestRunDto testRun)
        {
            return BuildXml(testRun);
        }

        public TestRunDto Deserialize(string xml)
        {
            var testRun = new TestRunDto();

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var doc = XDocument.Load(stream);

            var assemblies = doc.Element("assemblies");
            if (assemblies is not null)
            {
                ReadTraits(testRun, assemblies);
                var timestamp = assemblies.Attribute("timestamp")?.Value;
                if (DateTimeOffset.TryParse(timestamp, _culture, out var created))
                {
                    testRun.CreatedTime = created;
                }

                testRun.ExternalId = assemblies.Attribute("id")?.Value;
                testRun.Name = assemblies.Attribute("name")?.Value;

                foreach (var assemblyNode in assemblies.Elements("assembly"))
                {
                    var testSuite = new TestSuiteRunDto();
                    testSuite.Tests ??= new();
                    testSuite.Module = assemblyNode.Attribute("name")?.Value;
                    testRun.Suites ??= new();
                    testRun.Suites.Add(testSuite);

                    foreach (var collectionNode in assemblyNode.Elements("collection"))
                    {
                        testSuite.ExternalId = collectionNode.Attribute("id")?.Value;
                        testSuite.Name = collectionNode.Attribute("name")?.Value;
                        ReadTraits(testSuite, collectionNode);

                        foreach (var resultNode in collectionNode.Elements("test"))
                        {
                            var testCaseRun = new TestCaseRunDto();
                            ReadTraits(testCaseRun, resultNode);

                            CopyTestTraitsToParent(testRun, testSuite, testCaseRun);

                            testCaseRun.ExternalId = resultNode.Attribute("id")?.Value;
                            testCaseRun.Name = resultNode.Attribute("name")?.Value;
                            testCaseRun.ClassName = resultNode.Attribute("type")?.Value;
                            testCaseRun.Method = resultNode.Attribute("method")?.Value;
                            testCaseRun.Result = TestResult.Passed;
                            testCaseRun.ExternalId = ImportDefaults.GetExternalId(testRun, testSuite, testCaseRun);

                            var resultString = resultNode.Attribute("result")?.Value;
                            if (resultString is not null)
                            {
                                testCaseRun.Result = resultString switch
                                {
                                    "Pass" => TestResult.Passed,
                                    "Fail" => TestResult.Failed,
                                    "Skip" => TestResult.Skipped,
                                    _ => TestResult.Skipped,
                                };
                            }

                            var failure = resultNode.Element("failure");

                            if (failure is not null)
                            {
                                testCaseRun.Message = failure.Element("message")?.Value;
                                testCaseRun.CallStack = failure.Element("stack-trace")?.Value;
                            }

                            testSuite.Tests.Add(testCaseRun);

                            string? timeString = resultNode.Attribute("time")?.Value;
                            if (timeString is not null && double.TryParse(timeString, CultureInfo.InvariantCulture, out var time))
                            {
                                testCaseRun.Duration = TimeSpan.FromSeconds(time);
                            }
                        }
                    }
                }
            }

            return testRun;
        }

        private static void CopyTestTraitsToParent(TestRunDto testRun, TestSuiteRunDto testSuite, TestCaseRunDto testCaseRun)
        {
            if (testCaseRun.Project is not null)
            {
                if (testSuite.Project is null)
                {
                    testSuite.Project = testCaseRun.Project;
                }
                if (testRun.Project is null)
                {
                    testRun.Project = testCaseRun.Project;
                }
            }
            if (testCaseRun.TestCategory is not null)
            {
                if (testSuite.TestCategory is null)
                {
                    testSuite.TestCategory = testCaseRun.TestCategory;
                }
                if (testRun.TestCategory is null)
                {
                    testRun.TestCategory = testCaseRun.TestCategory;
                }
            }
        }

        private string BuildXml(TestRunDto testRun)
        {
            var doc = new XDocument();
            var assemblies = new XElement("assemblies");
            WriteTraits(testRun, assemblies);
            doc.Add(assemblies);

            if (testRun.CreatedTime is not null)
            {
                assemblies.Add(new XAttribute("timestamp", testRun.CreatedTime.Value.ToString(_culture)));
            }

            if (testRun.ExternalId is not null)
            {
                assemblies.Add(new XAttribute("id", testRun.ExternalId));
            }
            if (testRun.Name is not null)
            {
                assemblies.Add(new XAttribute("name", testRun.Name));
            }

            testRun.Suites ??= new();
            foreach (var testSuite in testRun.Suites)
            {
                var assembly = new XElement("assembly");
                if (testSuite.Module is not null)
                {
                    assembly.Add(new XAttribute("name", testSuite.Module));
                }
                assemblies.Add(assembly);

                double totalTestSuiteSeconds = 0;
                var collection = new XElement("collection");
                assembly.Add(collection);
                WriteTraits(testSuite, collection);
                collection.Add(new XAttribute("total", testSuite.Total.ToString()));
                collection.Add(new XAttribute("passed", testSuite.Passed.ToString()));
                collection.Add(new XAttribute("failed", testSuite.Failed.ToString()));
                if (testSuite.Name is not null)
                {
                    collection.Add(new XAttribute("name", testSuite.Name));
                }
                if (testSuite.ExternalId is not null)
                {
                    collection.Add(new XAttribute("id", testSuite.ExternalId));
                }
                testSuite.Tests ??= new();
                foreach (var test in testSuite.Tests)
                {
                    var testElement = new XElement("test");
                    WriteTraits(test, testElement);
                    collection.Add(testElement);

                    if (test.Name is not null)
                    {
                        testElement.Add(new XAttribute("name", test.Name));
                    }
                    if (test.ExternalId is not null)
                    {
                        testElement.Add(new XAttribute("id", test.ExternalId));
                    }
                    if (test.Method is not null)
                    {
                        testElement.Add(new XAttribute("method", test.Method));
                    }
                    if (test.ClassName is not null)
                    {
                        testElement.Add(new XAttribute("type", test.ClassName));
                    }
                    if (test.Duration is not null)
                    {
                        var seconds = test.Duration.Value.TotalSeconds;
                        testElement.Add(new XAttribute("time", seconds));
                        totalTestSuiteSeconds += seconds;
                    }

                    var resultName = test.Result switch
                    {
                        TestResult.Passed => "Pass",
                        TestResult.Failed => "Fail",
                        TestResult.Skipped => "Skip",
                        _ => "Fail"
                    };
                    testElement.Add(new XAttribute("result", resultName));

                    if (test.Result != TestResult.Passed &&
                        test.Result != TestResult.NoRun &&
                        test.Result != TestResult.Skipped &&
                        test.Result != TestResult.Blocked)
                    {
                        var failureElement = new XElement("failure");
                        if (test.Message is not null)
                        {
                            failureElement.Add(new XElement("message", test.Message));
                        }
                        if (test.CallStack is not null)
                        {
                            failureElement.Add(new XElement("stack-trace", test.CallStack));
                        }
                        testElement.Add(failureElement);
                    }
                }

                collection.Add(new XAttribute("time", totalTestSuiteSeconds));
            }
            return doc.ToString(SaveOptions.OmitDuplicateNamespaces);
        }

        private static void ReadTraits(TestTraitCollection attributes, XElement node)
        {
            foreach (var traits in node.Elements("traits"))
            {
                foreach (var propertyNode in traits.Elements("trait"))
                {
                    var name = propertyNode.Attribute("name")?.Value;
                    var value = propertyNode.Attribute("value")?.Value;
                    if (name is not null && value is not null)
                    {
                        var attributeType = GetTestTraitType(name);
                        if (attributeType == TestTraitType.Custom)
                        {
                            attributes.Attributes.Add(new TestTrait(attributeType, name, value));
                        }
                        else
                        {
                            attributes.Attributes.Add(new TestTrait(attributeType, value));
                        }
                    }
                }
            }
        }


        private static void WriteTraits(TestTraitCollection attributeCollection, XElement element)
        {
            var traits = new XElement("traits");
            element.Add(traits);

            foreach (var attribute in attributeCollection.Attributes)
            {
                if (!_nativeAttributes.Contains(attribute.Type))
                {
                    var property = new XElement("trait",
                        new XAttribute("name", GetAttributeName(attribute)),
                        new XAttribute("value", attribute.Value));
                    traits.Add(property);
                }
            }
        }

        private static TestTraitType GetTestTraitType(string name)
        {
            var types = _propertyNames.Where(x => x.Value == name).Select(x => x.Key);
            foreach (var type in types)
            {
                return type;
            }

            if (Enum.TryParse(typeof(TestTraitType), name, true, out object? enumType))
            {
                return (TestTraitType)enumType;
            }
            return TestTraitType.Custom;
        }

        private static string GetAttributeName(TestTrait attribute)
        {
            if (_propertyNames.TryGetValue(attribute.Type, out var name))
            {
                return name;
            }
            if (attribute.Name is not null)
            {
                return attribute.Name;
            }
            return attribute.Type.ToString();
        }

        private string FormatTime(TimeSpan duration)
        {
            var numberFormat = new NumberFormatInfo();
            numberFormat.NumberDecimalSeparator = ".";
            numberFormat.NumberGroupSeparator = "";

            var seconds = duration.TotalSeconds;
            return Convert.ToString(seconds, numberFormat);
        }

    }
}
