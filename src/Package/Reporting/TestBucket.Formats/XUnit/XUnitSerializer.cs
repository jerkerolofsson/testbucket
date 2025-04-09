using System;

using TestBucket.Formats.Shared;
using TestBucket.Traits.Core;

namespace TestBucket.Formats.XUnit
{
    public class XUnitSerializer : ITestResultSerializer
    {
        private static readonly CultureInfo _culture = new CultureInfo("en-US");

        /// <summary>
        /// Traits that will not be serialized as they are handled by xunit itself (as XML attributes or elements)
        /// </summary>
        private static readonly HashSet<TraitType> _nativeAttributes =
        [
            TraitType.TestId,
            TraitType.Name,
            TraitType.TestResult,
            TraitType.ClassName,
            TraitType.Method,
            TraitType.Module,
            TraitType.FailureMessage,
            TraitType.Line,
            TraitType.Duration,
            TraitType.SystemOut,
            TraitType.FailureType
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
                    var assemblyPath = assemblyNode.Attribute("name")?.Value;
                    string? assemblyName = null;
                    if(assemblyPath is not null)
                    {
                        assemblyName = assemblyPath.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries).Last();
                    }

                    var testSuite = new TestSuiteRunDto();
                    testSuite.Tests ??= new();
                    testSuite.Module = assemblyName ?? assemblyPath;
                    testRun.Name ??= testSuite.Module;
                    testRun.Suites ??= new();
                    testRun.Suites.Add(testSuite);

                    testSuite.ExternalId = testSuite.Module;
                    testSuite.Name = testSuite.Module;

                    foreach (var collectionNode in assemblyNode.Elements("collection"))
                    {
                        var collectionName = collectionNode.Attribute("name")?.Value;
                        ReadTraits(testSuite, collectionNode);

                        foreach (var resultNode in collectionNode.Elements("test"))
                        {
                            var testCaseRun = new TestCaseRunDto();
                            ReadTraits(testCaseRun, resultNode);
                            ReadAttachments(testCaseRun, resultNode);

                            CopyTestTraitsToParent(testRun, testSuite, testCaseRun);

                            if (collectionName is not null)
                            {
                                testCaseRun.Traits.Add(new TestTrait { Type = TraitType.CollectionName, Name = "Collection", Value = collectionName });
                            }
                            testCaseRun.Name = resultNode.Attribute("name")?.Value;
                            testCaseRun.ClassName = resultNode.Attribute("type")?.Value;
                            testCaseRun.Assembly = assemblyName;
                            testCaseRun.Method ??= testCaseRun.Name;
                            testCaseRun.Method = resultNode.Attribute("method")?.Value;
                            testCaseRun.Result = TestResult.Passed;
                            testCaseRun.ExternalId ??= ImportDefaults.GetExternalId(testRun, testSuite, testCaseRun);

                            var outputNode = resultNode.Element("output");
                            testCaseRun.SystemOut = outputNode?.Value;

                            var resultString = resultNode.Attribute("result")?.Value;
                            if (resultString is not null)
                            {
                                testCaseRun.Result = XUnitResultMapper.Map(resultString);
                            }

                            var failure = resultNode.Element("failure");

                            if (failure is not null)
                            {
                                testCaseRun.Message = failure.Element("message")?.Value;
                                testCaseRun.CallStack = failure.Element("stack-trace")?.Value;
                                testCaseRun.FailureType = failure.Element("exception-type")?.Value;
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
            WriteAttachments(testRun, assemblies, []);
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
                WriteAttachments(testSuite, collection, []);
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
                    WriteAttachments(test, testElement, test.Attachments);
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
                    if(!string.IsNullOrEmpty(test.SystemOut))
                    {
                        var outputElement = new XElement("output");
                        outputElement.Value = test.SystemOut;
                        testElement.Add(outputElement);
                    }

                    testElement.Add(new XAttribute("result", XUnitResultMapper.Map(test.Result)));

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
                        if (test.FailureType is not null)
                        {
                            failureElement.Add(new XAttribute("failure-type", test.FailureType));
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
                        var attributeType = TestTraitHelper.GetTestTraitType(name);
                        if (attributeType == TraitType.Custom)
                        {
                            attributes.Traits.Add(new TestTrait(attributeType, name, value) { ExportType = TraitExportType.Static });
                        }
                        else
                        {
                            attributes.Traits.Add(new TestTrait(attributeType, value) { ExportType = TraitExportType.Static });
                        }
                    }
                }
            }
        }

        private static void ReadAttachments(TestCaseRunDto test, XElement node)
        {
            foreach (var traits in node.Elements("attachments"))
            {
                foreach (var propertyNode in traits.Elements("attachment"))
                {
                    var value = propertyNode.Value;
                    var name = propertyNode.Attribute("name")?.Value;
                    var mediaType = propertyNode.Attribute("media-type")?.Value;
                    if (name is not null)
                    {
                        if (mediaType is not null)
                        {
                            try
                            {
                                var bytes = Convert.FromBase64String(value);
                                test.Attachments.Add(new AttachmentDto
                                {
                                    Name = name,
                                    ContentType = mediaType,
                                    Data = bytes.ToArray()
                                });
                            }
                            catch { }
                        }
                        else
                        {
                            var attributeType = TestTraitHelper.GetTestTraitType(name);
                            if (attributeType == TraitType.Custom)
                            {
                                test.Traits.Add(new TestTrait(attributeType, name, value) { ExportType = TraitExportType.Instance });
                            }
                            else
                            {
                                test.Traits.Add(new TestTrait(attributeType, value) { ExportType = TraitExportType.Instance });
                            }
                        }
                    }
                }
            }
        }

        private static void WriteTraits(TestTraitCollection attributeCollection, XElement element)
        {
            var traits = attributeCollection.Traits.Where(x => x.ExportType == TraitExportType.Static && !_nativeAttributes.Contains(x.Type)).ToArray();
            var attachments = attributeCollection.Traits.Where(x => x.ExportType == TraitExportType.Instance && !_nativeAttributes.Contains(x.Type)).ToArray();

            if (traits.Length > 0)
            {
                var traitsElement = new XElement("traits");
                element.Add(traitsElement);

                foreach (var attribute in traits)
                {
                    AddTrait(traitsElement, attribute);
                }
            }
        }


        private static void WriteAttachments(TestTraitCollection attributeCollection, XElement element, IReadOnlyCollection<AttachmentDto> fileAttachments)
        {
            var traits = attributeCollection.Traits.Where(x => x.ExportType == TraitExportType.Instance && !_nativeAttributes.Contains(x.Type)).ToArray();

            if (traits.Length > 0 || fileAttachments.Count > 0)
            {
                var attachmentsElement = new XElement("attachments");
                element.Add(attachmentsElement);

                foreach (var attribute in traits)
                {
                    var attachmentElement = new XElement("attachment", new XAttribute("name", TestTraitHelper.GetTraitName(attribute)), new XCData(attribute.Value));
                    attachmentsElement.Add(attachmentElement);
                }

                // Files are attachmetns with a media-type, encoded as base64
                foreach(var attachment in fileAttachments)
                {
                    var data = Convert.ToBase64String(attachment.Data ?? []);
                    var mediaType = attachment.ContentType ?? "application/octet-stream";
                    var attachmentElement = new XElement("attachment", 
                        new XAttribute("name", attachment.Name ?? "attachment"), 
                        new XAttribute("media-type", mediaType), 
                        new XText(data));
                    attachmentsElement.Add(attachmentElement);
                }
            }
        }


        private static void AddTrait(XElement traitsElement, TestTrait attribute)
        {
            var traitElement = new XElement("trait",
                new XAttribute("name", TestTraitHelper.GetTraitName(attribute)),
                new XAttribute("value", attribute.Value));
            traitsElement.Add(traitElement);
        }
    }
}
