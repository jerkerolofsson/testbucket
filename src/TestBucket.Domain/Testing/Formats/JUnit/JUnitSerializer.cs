using System.Globalization;
using System.Text;
using System.Xml.Linq;

using TestBucket.Domain.Testing.Formats.Dtos;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Formats.JUnit
{
    public class JUnitSerializer : ITestResultSerializer
    {
        private static readonly Dictionary<TestTraitType, string> _propertyNames = new Dictionary<TestTraitType, string>
        {
            [TestTraitType.Version] = "version",
            [TestTraitType.Ci] = "ci",
            [TestTraitType.Commit] = "commit",
            [TestTraitType.TestCategory] = "category",
            [TestTraitType.Tag] = "tag",
        };

        /// <summary>
        /// Attributes not serialized as properties
        /// </summary>
        private static readonly HashSet<TestTraitType> _nativeAttributes =
        [
            TestTraitType.ExternalId,
            TestTraitType.Name,
            TestTraitType.TestResult,
            TestTraitType.SystemOut,
            TestTraitType.SystemErr,
            TestTraitType.Line,
            TestTraitType.Duration,
            TestTraitType.ClassName,

            TestTraitType.SystemOut,
            TestTraitType.SystemErr,

            TestTraitType.Message,
            TestTraitType.CallStack,
            TestTraitType.FailureType
        ];

        public string Serialize(TestRunDto testRun)
        {
            return BuildXml(testRun);
        }

        public TestRunDto Deserialize(string xml)
        {
            var testRun = new TestRunDto();
            testRun.Suites = new();

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var doc = XDocument.Load(stream);

            var testSuitesNode = doc.Element("testsuites");
            if (testSuitesNode is not null)
            {
                testRun.ExternalId = testSuitesNode.Attribute("id")?.Value;
                testRun.Name = testSuitesNode.Attribute("name")?.Value;
                ReadProperties(testRun, testSuitesNode);

                foreach (var testSuiteNode in testSuitesNode.Elements("testsuite"))
                {
                    var testSuite = new TestSuiteRunDto();
                    ReadProperties(testSuite, testSuiteNode);
                    testSuite.ExternalId = testSuiteNode.Attribute("id")?.Value;
                    testSuite.Name = testSuiteNode.Attribute("name")?.Value;
                    testRun.Name ??= testSuite.Name;
                    testRun.Suites.Add(testSuite);

                    testSuite.SystemOut = testSuiteNode.Element("system-out")?.Value;
                    testSuite.SystemErr = testSuiteNode.Element("system-err")?.Value;

                    foreach (var resultNode in testSuiteNode.Elements("testcase"))
                    {
                        var testCaseRun = new TestCaseRunDto();
                        ReadProperties(testCaseRun, resultNode);
                        testCaseRun.ExternalId = resultNode.Attribute("id")?.Value;
                        testCaseRun.Name = resultNode.Attribute("name")?.Value;
                        testCaseRun.ClassName = resultNode.Attribute("classname")?.Value;
                        testCaseRun.Result = TestResult.Passed;
                        testCaseRun.ExternalId = ImportDefaults.GetExternalId(testRun, testSuite, testCaseRun);

                        testSuite.Tests ??= new();
                        testSuite.Tests.Add(testCaseRun);

                        string? timeString = resultNode.Attribute("time")?.Value;
                        if (timeString is not null && double.TryParse(timeString, CultureInfo.InvariantCulture, out var time))
                        {
                            testCaseRun.Duration = TimeSpan.FromSeconds(time);
                        }

                        var failure = resultNode.Element("failure");
                        var blocked = resultNode.Element("blocked");
                        var skipped = resultNode.Element("skipped");
                        var error = resultNode.Element("error");
                        var crashed = resultNode.Element("crashed");
                        var assert = resultNode.Element("assert");

                        if (skipped is not null)
                        {
                            testCaseRun.Result = TestResult.Skipped;
                        }
                        if (blocked is not null)
                        {
                            testCaseRun.Result = TestResult.Blocked;
                        }
                        if (failure is not null)
                        {
                            testCaseRun.Result = TestResult.Failed;
                        }
                        if (error is not null)
                        {
                            testCaseRun.Result = TestResult.Error;
                        }
                        if (crashed is not null)
                        {
                            testCaseRun.Result = TestResult.Crashed;
                        }
                        if (assert is not null)
                        {
                            testCaseRun.Result = TestResult.Assert;
                        }

                        var failureElement = failure ?? blocked ?? skipped ?? error;
                        if (failureElement is not null)
                        {
                            testCaseRun.FailureType = failureElement.Attribute("type")?.Value;
                            testCaseRun.Message = failureElement.Attribute("message")?.Value;

                            testCaseRun.CallStack = failureElement.Value;
                        }
                    }
                }
            }


            return testRun;
        }

        private string BuildXml(TestRunDto testRun)
        {
            var doc = new XDocument();
            var testSuitesElement = new XElement("testsuites");
            doc.Add(testSuitesElement);

            double totalRunSeconds = 0;

            WriteProperties(testRun, testSuitesElement);

            // Standard attrfibutes
            testSuitesElement.Add(new XAttribute("tests", testRun.Total.ToString()));
            testSuitesElement.Add(new XAttribute("passed", testRun.Passed.ToString()));
            testSuitesElement.Add(new XAttribute("failures", testRun.Failed.ToString()));
            if (testRun.Name is not null)
            {
                testSuitesElement.Add(new XAttribute("name", testRun.Name));
            }
            if (testRun.ExternalId is not null)
            {
                testSuitesElement.Add(new XAttribute("id", testRun.ExternalId));
            }

            testRun.Suites ??= new();
            foreach (var testSuite in testRun.Suites)
            {
                double totalTestSuiteSeconds = 0;
                var testSuiteElement = new XElement("testsuite");
                WriteProperties(testSuite, testSuiteElement);
                testSuitesElement.Add(testSuiteElement);

                if (testSuite.SystemOut is not null)
                {
                    testSuiteElement.Add(new XElement("system-out", testSuite.SystemOut));
                }
                if (testSuite.SystemErr is not null)
                {
                    testSuiteElement.Add(new XElement("system-err", testSuite.SystemErr));
                }

                testSuite.Tests ??= new();

                testSuiteElement.Add(new XAttribute("tests", testSuite.Tests.Count.ToString()));
                testSuiteElement.Add(new XAttribute("passed", testSuite.Passed.ToString()));
                testSuiteElement.Add(new XAttribute("failures", testSuite.Failed.ToString()));
                testSuiteElement.Add(new XAttribute("assertions", testSuite.Asserted.ToString()));
                testSuiteElement.Add(new XAttribute("errors", testSuite.Errors.ToString()));
                if (testSuite.Name is not null)
                {
                    testSuiteElement.Add(new XAttribute("name", testSuite.Name));
                }
                if (testSuite.ExternalId is not null)
                {
                    testSuiteElement.Add(new XAttribute("id", testSuite.ExternalId));
                }

                foreach (var result in testSuite.Tests)
                {
                    var resultElement = new XElement("testcase");
                    WriteProperties(result, resultElement);
                    testSuiteElement.Add(resultElement);

                    if (result.Name is not null)
                    {
                        resultElement.Add(new XAttribute("name", result.Name));
                    }
                    if (result.ExternalId is not null)
                    {
                        resultElement.Add(new XAttribute("id", result.ExternalId));
                    }
                    if (result.ClassName is not null)
                    {
                        resultElement.Add(new XAttribute("classname", result.ClassName));
                    }
                    if (result.Duration is not null)
                    {
                        var seconds = result.Duration.Value.TotalSeconds;
                        resultElement.Add(new XAttribute("time", seconds));
                        totalRunSeconds += seconds;
                        totalTestSuiteSeconds += seconds;
                    }

                    XElement? failureElement = null;
                    switch (result.Result)
                    {
                        case TestResult.Skipped:
                            failureElement = new XElement("skipped");
                            break;
                        case TestResult.Error:
                            failureElement = new XElement("error");
                            break;
                        case TestResult.Failed:
                            failureElement = new XElement("failure");
                            break;
                        case TestResult.Blocked:
                            failureElement = new XElement("blocked");
                            break;
                        case TestResult.Assert:
                            failureElement = new XElement("assert");
                            break;
                        case TestResult.Crashed:
                            failureElement = new XElement("crashed");
                            break;
                    }

                    if (failureElement is not null)
                    {
                        resultElement.Add(failureElement);
                        if (result.Message is not null)
                        {
                            failureElement.Add(new XAttribute("message", result.Message));
                        }
                        if (result.FailureType is not null)
                        {
                            failureElement.Add(new XAttribute("type", result.FailureType));
                        }
                        if (result.CallStack is not null)
                        {
                            failureElement.Add(new XText(result.CallStack));
                        }

                    }
                }

                testSuiteElement.Add(new XAttribute("time", totalTestSuiteSeconds));
            }
            testSuitesElement.Add(new XAttribute("time", totalRunSeconds));
            return doc.ToString(SaveOptions.OmitDuplicateNamespaces);
        }

        private static void ReadProperties(TestTraitCollection attributes, XElement node)
        {
            foreach (var propertiesNode in node.Elements("properties"))
            {
                foreach (var propertyNode in propertiesNode.Elements("property"))
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


        private static void WriteProperties(TestTraitCollection attributeCollection, XElement testSuitesElement)
        {
            var properties = new XElement("properties");
            testSuitesElement.Add(properties);
            foreach (var attribute in attributeCollection.Attributes)
            {
                if (!_nativeAttributes.Contains(attribute.Type))
                {
                    var property = new XElement("property",
                        new XAttribute("name", GetAttributeName(attribute)),
                        new XAttribute("value", attribute.Value));
                    properties.Add(property);
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
