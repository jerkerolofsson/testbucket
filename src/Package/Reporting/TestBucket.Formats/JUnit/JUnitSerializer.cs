using System.ComponentModel;
using System.Text.RegularExpressions;

using TestBucket.Contracts.Abstractions;
using TestBucket.Formats.Shared;
using TestBucket.Traits.Core;

namespace TestBucket.Formats.JUnit
{
    public class JUnitSerializer : ITestResultSerializer
    {
        /// <summary>
        /// Attributes not serialized as properties as they are supported natively by junit
        /// </summary>
        private static readonly HashSet<TraitType> _nativeAttributes =
        [
            TraitType.TestId,
            TraitType.Name,
            TraitType.TestResult,
            TraitType.Line,
            TraitType.Duration,
            TraitType.ClassName,

            TraitType.SystemOut,
            TraitType.SystemErr,

            TraitType.FailureMessage,
            TraitType.CallStack,
            TraitType.FailureType
        ];

        public string Serialize(TestRunDto testRun)
        {
            return BuildXml(testRun);
        }

        public TestRunDto Deserialize(string xml)
        {
            var options = new JUnitSerializerOptions();
            return Deserialize(xml, options);
        }

        public TestRunDto Deserialize(string xml, JUnitSerializerOptions options)
        {
            var testRun = new TestRunDto();
            testRun.Suites = new();

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var doc = XDocument.Load(stream);

            var testSuitesNode = doc.Element("testsuites");
            if(testSuitesNode is null)
            {
                var testSuiteNode = doc.Element("testsuite");
                if (testSuiteNode is not null)
                {
                    testRun.ExternalId = testSuiteNode.Attribute("id")?.Value;
                    testRun.Name = testSuiteNode.Attribute("name")?.Value;
                    ReadProperties(null, testRun, testSuiteNode);
                    testRun.Assembly ??= testRun.Name;
                    testRun.Module ??= testRun.Name;

                    ParseTestSuiteNode(options, testRun, testSuiteNode);
                }
            }
            else
            {
                testRun.ExternalId = testSuitesNode.Attribute("id")?.Value;
                testRun.Name = testSuitesNode.Attribute("name")?.Value;
                ReadProperties(null, testRun, testSuitesNode);
                testRun.Assembly ??= testRun.Name;
                testRun.Module ??= testRun.Name;

                foreach (var testSuiteNode in testSuitesNode.Elements("testsuite"))
                {
                    ParseTestSuiteNode(options, testRun, testSuiteNode);
                }
            }


            return testRun;
        }

        private void ParseTestSuiteNode(JUnitSerializerOptions options, TestRunDto testRun, XElement testSuiteNode)
        {
            var testSuite = new TestSuiteRunDto();
            testRun.Suites.Add(testSuite);

            ReadProperties(null, testSuite, testSuiteNode);
            testSuite.Name = testSuiteNode.Attribute("name")?.Value;
            if (options.ProcessXunitCollectionName)
            {
                HandleXunitNameFormat(testSuite);
            }
            testSuite.ExternalId ??= testSuiteNode.Attribute("id")?.Value;

            testSuite.Assembly ??= testSuite.Name;
            testSuite.Module ??= testSuite.Name;
            testSuite.SystemOut = testSuiteNode.Element("system-out")?.Value;
            testSuite.SystemErr = testSuiteNode.Element("system-err")?.Value;

            // Copy some fields to the parent
            testRun.Name ??= testSuite.Name;

            var folders = new Stack<string>();
            ReadTestsFromSuite(testRun, testSuiteNode, testSuite, testSuite.Name ?? "", folders, options);
        }

        private void HandleXunitNameFormat(TestSuiteRunDto testSuite)
        {
            if(string.IsNullOrEmpty(testSuite.Name))
            {
                return;
            }

            // Test collection for TestBucket.Formats.UnitTests.XUnit.XUnitSerializerTests (id: da8cd3d88ee6e9d4988dcd095d84dc5287c1fa9c88564f2d34192c011f8ae07e)
            Regex regex = new Regex("Test collection for (.*) \\(id: (.*)\\)");
            var match = regex.Match(testSuite.Name);
            if(match.Success && match.Groups.Count == 3)
            {
                testSuite.Name = match.Groups[1].Value;
                testSuite.ExternalId ??= match.Groups[2].Value;
            }
        }

        private static void ReadTestsFromSuite(TestRunDto testRun, XElement testSuiteNode, TestSuiteRunDto testSuite, string testSuiteElementName, Stack<string> folders, JUnitSerializerOptions options)
        {
            foreach (var resultNode in testSuiteNode.Elements("testcase"))
            {
                var testCaseRun = new TestCaseRunDto();
                ReadProperties(testCaseRun, testCaseRun, resultNode);
                testCaseRun.Name = resultNode.Attribute("name")?.Value;
                testCaseRun.ClassName = resultNode.Attribute("classname")?.Value;
                testCaseRun.Result = TestResult.Passed;
                testCaseRun.ExternalId ??= resultNode.Attribute("id")?.Value;
                testCaseRun.ExternalId ??= ImportDefaults.GetExternalId(testRun, testSuite, testCaseRun);

                testCaseRun.Method ??= testCaseRun.Name;

                // There can be an assembly trait on a test case, which is more accurate than what is defined in the 
                // junst, if there is, use that
                if (testCaseRun.Assembly is not null)
                {
                    testRun.Assembly = testSuite.Assembly = testCaseRun.Assembly;
                }
                else
                {
                    // inherit
                    testCaseRun.Assembly ??= testSuite.Assembly ?? testRun.Assembly;
                }
                testCaseRun.Folders = folders.ToArray();

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
                var passed = resultNode.Element("passed");

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

                var failureElement = failure ?? blocked ?? skipped ?? error ?? passed ?? crashed;
                if (failureElement is not null)
                {
                    testCaseRun.FailureType = failureElement.Attribute("type")?.Value;
                    testCaseRun.Message = failureElement.Attribute("message")?.Value;
                    testCaseRun.CallStack = failureElement.Value;
                }
            }

            foreach (var innerTestSuiteNode in testSuiteNode.Elements("testsuite"))
            {
                // Nested testsuite may have a name that matches the namespace, like:
                // Tests.Registration
                //  Tests.Registration.Email

                // If that's the case, we'll crop the name so that the folder name is "Email"
                var name = innerTestSuiteNode.Attribute("name")?.Value;
                if (name != null)
                {
                    var croppedName = name;
                    if(testSuiteElementName is not null && name is not null)
                    {
                        if(name.Length-1 > testSuiteElementName.Length && name.StartsWith(testSuiteElementName))
                        {
                            croppedName = name.Substring(testSuiteElementName.Length+1);
                        }
                    }
                    folders.Push(croppedName);
                }

                ReadTestsFromSuite(testRun, innerTestSuiteNode, testSuite, name??"", folders, options);

                if (name != null)
                {
                    folders.Pop();
                }
            }
        }

        private string BuildXml(TestRunDto testRun)
        {
            var doc = new XDocument();
            var testSuitesElement = new XElement("testsuites");
            doc.Add(testSuitesElement);

            double totalRunSeconds = 0;

            WriteProperties(testRun, testSuitesElement);

            // Standard attributes
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
                    WriteAttachments(result, resultElement);
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
                        case TestResult.Passed:
                            failureElement = new XElement("passed");
                            break;
                    }

                    if (failureElement is not null)
                    {
                        resultElement.Add(failureElement);
                        if (result.Message is not null)
                        {
                            failureElement.Add(new XAttribute("message", result.Message));
                        }
                        if (result.FailureType is not null && result.FailureType is not null)
                        {
                            failureElement.Add(new XAttribute("type", result.FailureType));
                        }
                        if (result.CallStack is not null)
                        {
                            failureElement.Add(new XText(result.CallStack));
                        }
                    }
                    else if(!string.IsNullOrEmpty(result.Message))
                    {

                    }
                }

                testSuiteElement.Add(new XAttribute("time", totalTestSuiteSeconds));
            }
            testSuitesElement.Add(new XAttribute("time", totalRunSeconds));
            return doc.ToString(SaveOptions.OmitDuplicateNamespaces);
        }

        private static void ReadProperties(ITestAttachmentSource? attachmentSource, TestTraitCollection attributes, XElement node)
        {
            foreach (var propertiesNode in node.Elements("properties"))
            {
                if (attachmentSource is not null)
                {
                    ReadAttachmentsFromPropertiesNode(attachmentSource, propertiesNode);
                }

                foreach (var propertyNode in propertiesNode.Elements("property"))
                {
                    ReadTraitsAndAttachmentsFromPropertyNode(attachmentSource, attributes, propertyNode);
                }
            }
        }

        private static void ReadTraitsAndAttachmentsFromPropertyNode(ITestAttachmentSource? attachmentSource, TestTraitCollection attributes, XElement propertyNode)
        {
            var traitPrefix = "trait:";
            var attachmentPrefix = "attachment:";

            var name = propertyNode.Attribute("name")?.Value;
            var value = propertyNode.Attribute("value")?.Value ?? propertyNode.Value;
            if(value is null)
            {
                value = propertyNode.Descendants().Where(x => x.NodeType == XmlNodeType.Text).Select(x => x.Value).FirstOrDefault();
            }

            // <property name="attachment:file.png">data:image/png;base64,iVB..
            if (name is not null && name.StartsWith(attachmentPrefix) && value is not null && value.StartsWith("data:"))
            {
                if (name.StartsWith(attachmentPrefix))
                {
                    name = name[attachmentPrefix.Length..];

                    // get value from inner text
                    if (attachmentSource is not null)
                    {
                        var attachment = DataUriParser.ParseDataUri(value);
                        attachment.Name = name;
                        attachmentSource.Attachments.Add(attachment);
                    }
                }
            }
            else if (name is not null && value is not null)
            {
                // Trait, even with attachment prefix
                if (name.StartsWith(attachmentPrefix))
                {
                    name = name[attachmentPrefix.Length..];
                }
                if (name.StartsWith(traitPrefix))
                {
                    name = name[traitPrefix.Length..];
                }
                var attributeType = TestTraitHelper.GetTestTraitType(name);
                if (attributeType == TraitType.Custom)
                {
                    attributes.Traits.Add(new TestTrait(attributeType, name, value));
                }
                else
                {
                    attributes.Traits.Add(new TestTrait(attributeType, value));
                }
            }
        }

        private static void ReadAttachmentsFromPropertiesNode(ITestAttachmentSource? attachmentSource, XElement propertiesNode)
        {
            foreach (var propertyNode in propertiesNode.Elements("attachment"))
            {
                var name = propertyNode.Attribute("name")?.Value;
                var mimeType = propertyNode.Attribute("media-type")?.Value ??
                    propertyNode.Attribute("content-type")?.Value ??
                    propertyNode.Attribute("mime-type")?.Value ??
                    "application/octet-stream";
                var value = propertyNode.Value ?? propertyNode.Attribute("value")?.Value;
                if (name is not null)
                {
                    if (attachmentSource is not null)
                    {
                        byte[] data = [];
                        if (value is not null)
                        {
                            data = Convert.FromBase64String(value);
                        }

                        var attachment = new AttachmentDto() { Name = name, ContentType = mimeType, Data = data };
                        attachmentSource.Attachments.Add(attachment);
                    }
                }
            }
        }

        private static void WriteAttachments(TestCaseRunDto test, XElement testSuitesElement)
        {
            var properties = new XElement("properties");
            testSuitesElement.Add(properties);
            WritePropertiesToPropertiesElement(test, properties);
            foreach (var attachment in test.Attachments)
            {
                if(attachment.Data is null)
                {
                    continue;
                }
                var name = "attachment:" + attachment.Name;
                var innerText = $"data:{attachment.ContentType};base64,{Convert.ToBase64String(attachment.Data)}";

                var property = new XElement("property",new XAttribute("name", name), new XText(innerText));
                properties.Add(property);
            }
        }
        private static void WriteProperties(TestTraitCollection attributeCollection, XElement testSuitesElement)
        {
            var properties = new XElement("properties");
            testSuitesElement.Add(properties);
            WritePropertiesToPropertiesElement(attributeCollection, properties);
        }

        private static void WritePropertiesToPropertiesElement(TestTraitCollection attributeCollection, XElement properties)
        {
            foreach (var attribute in attributeCollection.Traits)
            {
                if (!_nativeAttributes.Contains(attribute.Type))
                {
                    var property = new XElement("property",
                        new XAttribute("name", GetTraitName(attribute)),
                        new XAttribute("value", attribute.Value));
                    properties.Add(property);
                }
            }
        }

        private static string GetTraitName(TestTrait attribute)
        {
            // Well known traits
            if (TraitTypeConverter.TryConvert(attribute.Type, out var name))
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
