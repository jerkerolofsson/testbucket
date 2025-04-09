using System;

using TestBucket.Formats.Shared;
using TestBucket.Traits.Core;

using static System.Net.Mime.MediaTypeNames;

namespace TestBucket.Formats.MicrosoftTrx;

internal class TrxSerializer : ITestResultSerializer
{
    private const string UnitTestTypeGuid = "13CDC9D9-DDB5-4fa4-A97D-D965CCFC6D4B";

    /// <summary>
    /// Attributes not serialized as properties as they are supported natively by .trx standard format
    /// </summary>
    private static readonly HashSet<TraitType> _nativeAttributes =
    [
        TraitType.TestId,
        TraitType.Name,
        TraitType.TestResult,
        TraitType.Line,
        TraitType.Duration,
        TraitType.ClassName,
        TraitType.Assembly,
        TraitType.Method,

        TraitType.SystemOut,
        TraitType.SystemErr,

        TraitType.FailureMessage,
        TraitType.CallStack,
        TraitType.InstanceId,
        TraitType.Computer,
        TraitType.InstanceName,
        TraitType.InstanceUserName,
    ];

    private XNamespace mstest => "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
    private XNamespace testbucket => "urn:github.com/jerkerolofsson/testbucket/TrxExtensions/1";

    public TestRunDto Deserialize(string text)
    {
        var testRun = new TestRunDto();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var doc = XDocument.Load(stream);
        XElement? testRunNode = ParseRunNode(testRun, doc);

        if (testRunNode is not null)
        {
            ParseTimesNode(testRun, testRunNode);
            List<TrxList> lists = ParseLists(testRunNode);
            List<TrxTestEntry> entries = ParseEntries(testRunNode);

            foreach (var list in lists)
            {
                var suite = new TestSuiteRunDto() { Name = list.Name, ExternalId = list.Id };
                testRun.Suites.Add(suite);

                foreach (var entry in entries.Where(x => x.TestListId == list.Id))
                {
                    foreach (TestCaseRunDto test in ParseTest(testRunNode, entry))
                    {
                        suite.Tests.Add(test);
                    }
                }
            }
        }

        return testRun;
    }

    private IEnumerable<TestCaseRunDto> ParseTest(XElement testRunNode, TrxTestEntry entry)
    {
        XElement? testDefinitions = testRunNode.Element(mstest + "TestDefinitions");
        XElement? results = testRunNode.Element(mstest + "Results");

        TestCaseRunDto test = new()
        {
            ExternalId = entry.TestId
        };

        if (testDefinitions is not null && results is not null)
        {
            // TestDefinitions/UnitTest
            foreach (var testDefinition in testDefinitions.Elements(mstest + "UnitTest"))
            {
                ParseTraits(test, testDefinition);

                if (testDefinition.Attribute("id")?.Value == entry.TestId)
                {
                    // Test Case name
                    test.Name = testDefinition.Attribute("name")?.Value;

                    /* <TestMethod 
                     *  codeBase="D:\local\code\testbucket\tests\TestBucket.Formats.UnitTests\bin\Debug\net9.0\TestBucket.Formats.UnitTests.dll" 
                     *  adapterTypeName="executor://xunit/VsTestRunner2/netcoreapp" 
                     *  className="TestBucket.Formats.UnitTests.JUnit.JUnitSerializerTests" 
                     *  name="Deserialize_WithTwoTestSuites_TwoRunsDeserializedWithCorrectNames" />
                     * */
                    var testMethodNode = testDefinition.Element(mstest + "TestMethod");
                    if (testMethodNode is not null)
                    {
                        var codeBase = testMethodNode.Attribute("codeBase")?.Value;
                        if (codeBase is not null)
                        {
                            test.Assembly = Path.GetFileName(codeBase);
                        }
                        //var adapterTypeName = testMethodNode.Attribute("adapterTypeName")?.Value;
                        test.ClassName = testMethodNode.Attribute("className")?.Value;
                        test.Method = testMethodNode.Attribute("name")?.Value;
                    }
                }
            }
            foreach (var resultNode in results.Elements(mstest + "UnitTestResult"))
            {
                if (resultNode.Attribute("executionId")?.Value == entry.ExecutionId)
                {
                    ParseAttachments(element: resultNode, test.Attachments);
                    ParseTraits(test, resultNode);
                    ParseUnitTestResultNode(test, resultNode);

                    var innerResultsNode = resultNode.Element(mstest + "InnerResults");
                    if (innerResultsNode is not null)
                    {
                        foreach (var innerResultNode in innerResultsNode.Elements(mstest + "UnitTestResult"))
                        {
                            var innerTest = new TestCaseRunDto();
                            innerTest.ClassName = test.ClassName;
                            innerTest.Method = test.Method;
                            innerTest.Assembly = test.Assembly;
                            innerTest.ExternalId = test.ExternalId;
                            innerTest.Name = test.Name;
                            ParseUnitTestResultNode(innerTest, innerResultsNode);
                            yield return innerTest;
                        }
                    }
                }
            }
        }
        yield return test;
    }

    private static void ParseUnitTestResultNode(TestCaseRunDto test, XElement resultNode)
    {
        test.InstanceName = resultNode.Attribute("testName")?.Value;
        test.Computer = resultNode.Attribute("computerName")?.Value;

        string? outcome = resultNode.Attribute("outcome")?.Value;
        test.Result = TrxResultMapper.Map(outcome);

        string? duration = resultNode.Attribute("duration")?.Value;
        string? startTime = resultNode.Attribute("startTime")?.Value;
        string? endTime = resultNode.Attribute("endTime")?.Value;

        test.StartedTime = resultNode.XAttributeDateTimeOffset("startTime");
        test.EndedTime = resultNode.XAttributeDateTimeOffset("endTime");
        test.Duration = resultNode.XAttributeTimeSpan("duration");
    }

    private List<TrxTestEntry> ParseEntries(XElement testRunNode)
    {
        var lists = new List<TrxTestEntry>();

        XElement? testListsNode = testRunNode.Element(mstest + "TestEntries");
        if (testListsNode is not null)
        {
            foreach (var testListNode in testListsNode.Elements(mstest + "TestEntry"))
            {
                var testId = testListNode.Attribute("testId")?.Value ?? Guid.NewGuid().ToString();
                var executionId = testListNode.Attribute("executionId")?.Value ?? Guid.NewGuid().ToString();
                var testListId = testListNode.Attribute("testListId")?.Value ?? Guid.NewGuid().ToString();
                lists.Add(new TrxTestEntry(testId, executionId, testListId));
            }
        }

        return lists;
    }
    private List<TrxList> ParseLists(XElement testRunNode)
    {
        var lists = new List<TrxList>();

        XElement? testListsNode = testRunNode.Element(mstest + "TestLists");
        if (testListsNode is not null)
        {
            foreach (var testListNode in testListsNode.Elements(mstest + "TestList"))
            {
                var id = testListNode.Attribute("id")?.Value ?? Guid.NewGuid().ToString();
                var name = testListNode.Attribute("name")?.Value;
                lists.Add(new TrxList(id, name));
            }
        }

        return lists;
    }

    private XElement? ParseRunNode(TestRunDto testRun, XDocument doc)
    {
        var testRunNode = doc.Element(mstest + "TestRun");
        if (testRunNode is not null)
        {
            testRun.ExternalId = testRunNode.Attribute("id")?.Value;
            testRun.Name = testRunNode.Attribute("name")?.Value;
            testRun.InstanceUserName = testRunNode.Attribute("runUser")?.Value;

            ParseTraits(testRun, testRunNode);
        }
        return testRunNode;
    }


    private void ParseTimesNode(TestRunDto testRun, XElement testRunNode)
    {
        XElement? timesNode = testRunNode.Element(mstest + "Times");
        if (timesNode is not null)
        {
            testRun.StartedTime = timesNode.XAttributeDateTimeOffset("creation");
            testRun.EndedTime = timesNode.XAttributeDateTimeOffset("finish");
        }
    }

    public string Serialize(TestRunDto testRun)
    {
        return BuildXml(testRun);
    }


    private string BuildXml(TestRunDto testRun)
    {
        // Assign mandatory guids..
        IdAssigner.AssignGuids(testRun);

        var doc = new XDocument();
        var testRunElement = new XElement(mstest + "TestRun",
                    new XAttribute("name", testRun.Name ?? ""),
                    new XAttribute("id", testRun.ExternalId ?? ""),
                    new XAttribute("runUser", testRun.InstanceUserName ?? ""));

        AddTraits(TraitExportType.Static, testRunElement, testRun);
        AddTraits(TraitExportType.Instance, testRunElement, testRun);

        doc.Add(testRunElement);

        var timesElement = new XElement(mstest + "Times");
        if (testRun.StartedTime is not null)
        {
            timesElement.Add(new XAttribute("creation", testRun.StartedTime));
        }
        if (testRun.EndedTime is not null)
        {
            timesElement.Add(new XAttribute("finish", testRun.EndedTime));
        }
        testRunElement.Add(timesElement);

        var testSettingsElement = new XElement(mstest + "TestSettings");
        testRunElement.Add(testSettingsElement);

        var resultsElement = new XElement(mstest + "Results");
        foreach (var suite in testRun.Suites)
        {
            foreach (var test in suite.Tests)
            {
                var unitTest = new XElement(mstest + "UnitTestResult",
                    new XAttribute("testName", test.InstanceName ?? test.Name ?? ""),
                    new XAttribute("computerName", test.Computer ?? ""),
                    new XAttribute("duration", test.Duration ?? TimeSpan.Zero),
                    new XAttribute("startTime", test.StartedTime ?? DateTimeOffset.MinValue),
                    new XAttribute("endTime", test.EndedTime ?? DateTimeOffset.MinValue),
                    new XAttribute("testType", UnitTestTypeGuid),
                    new XAttribute("outcome", TrxResultMapper.Map(test.Result)),
                    new XAttribute("testListId", suite.ExternalId ?? ""),

                    new XAttribute("executionId", test.InstanceId ?? throw new InvalidDataException("Expected InstanceId on TestCaseRunDto")),
                    new XAttribute("testId", test.ExternalId ?? throw new InvalidDataException("Expected ExternalId on TestCaseRunDto")));

                unitTest.Add(new XElement(mstest + "Execution", new XAttribute("id", test.InstanceId ?? "")));
                unitTest.Add(new XElement(mstest + "TestMethod",
                    new XAttribute("name", test.Method ?? ""),
                    new XAttribute("className", test.ClassName ?? ""),
                    new XAttribute("codeBase", test.Assembly ?? "")
                    ));

                AddTraits(TraitExportType.Instance, unitTest, test);
                AddAttachments(element: unitTest, test.Attachments);

                resultsElement.Add(unitTest);
            }
        }
        testRunElement.Add(resultsElement);

        AddTestDefinitions(testRun, testRunElement);

        AddTestEntries(testRun, testRunElement);

        AddTestLists(testRun, testRunElement);

        var resultSummaryElement = new XElement(mstest + "ResultSummary");
        resultSummaryElement.Add(new XAttribute("outcome", "Completed"));
        resultSummaryElement.Add(new XElement(mstest + "Counters"), 
            new XAttribute("total", testRun.Total),
            new XAttribute("executed", testRun.Executed),
            new XAttribute("passed", testRun.Passed),
            new XAttribute("failed", testRun.Failed),
            new XAttribute("error", testRun.Errors),
            new XAttribute("completed", testRun.Executed),
            new XAttribute("notExecuted", testRun.Skipped),
            new XAttribute("timeout", 0),
            new XAttribute("inconclusive", 0),
            new XAttribute("aborted", 0),
            new XAttribute("notRunnable", 0),
            new XAttribute("passedButRunAborted", 0),
            new XAttribute("warning", 0),
            new XAttribute("disconnected", 0),
            new XAttribute("inProgress", 0),
            new XAttribute("pending", 0)
            );
        testRunElement.Add(resultSummaryElement);

        return doc.ToString(SaveOptions.OmitDuplicateNamespaces);
    }

    private void AddTestEntries(TestRunDto testRun, XElement testRunElement)
    {
        var listElement = new XElement(mstest + "TestEntries");
        foreach (var suite in testRun.Suites)
        {
            foreach (var test in suite.Tests)
            {
                listElement.Add(new XElement(mstest + "TestEntry",
                    new XAttribute("name", test.Name ?? ""),
                    new XAttribute("testId", test.ExternalId ?? throw new InvalidDataException("Expected ExternalId on TestCaseRunDto")),
                    new XAttribute("executionId", test.InstanceId ?? throw new InvalidDataException("Expected InstanceId on TestCaseRunDto")),

                    new XAttribute("testListId", suite.ExternalId ?? throw new InvalidDataException("Expected ExternalId on TestSuiteDto"))
                    ));
            }
        }
        testRunElement.Add(listElement);
    }

    private void AddTestLists(TestRunDto testRun, XElement testRunElement)
    {
        var testListsElement = new XElement(mstest + "TestLists");
        foreach (var suite in testRun.Suites)
        {
            testListsElement.Add(new XElement(mstest + "TestList",
                new XAttribute("name", suite.Name ?? "suite"),
                new XAttribute("id", suite.ExternalId ?? throw new InvalidDataException("Expected ExternalId on TestSuiteDto"))));

            AddTraits(TraitExportType.Static, testListsElement, suite);
            AddTraits(TraitExportType.Instance, testListsElement, suite);
        }
        testRunElement.Add(testListsElement);
    }

    private void AddAttachments(XElement element, List<AttachmentDto> attachments)
    {
        if(attachments.Count > 0)
        {
            var attachmentsNode = new XElement(testbucket + "Attachments");
            foreach (var attachment in attachments)
            {
                attachmentsNode.Add(new XElement(testbucket + "Attachment",
                    new XAttribute("name", attachment.Name ?? ""),
                    new XAttribute("encoding", "base64"),
                    new XAttribute("mediaType", attachment.ContentType ?? "application/octet-stream"),
                    new XText(Convert.ToBase64String(attachment.Data ?? []))));
            }
            element.Add(attachmentsNode);
        }
    }

    private void ParseAttachments(XElement element, List<AttachmentDto> attachments)
    {
        var attachmentsNode = element.Element(testbucket + "Attachments");

        if (attachmentsNode is not null)
        {
            foreach (var attachmentNode in attachmentsNode.Elements(testbucket + "Attachment"))
            {
                var name = attachmentNode.Attribute("name")?.Value;
                var mediaType = attachmentNode.Attribute("mediaType")?.Value ?? "application/octet-stream";
                var encoding = attachmentNode.Attribute("encoding")?.Value ?? "base64";
                if (name is not null && encoding == "base64")
                {
                    attachments.Add(new AttachmentDto
                    {
                        ContentType = mediaType,
                        Data = Convert.FromBase64String(attachmentNode.Value),
                        Name = name
                    });
                }
            }
        }
    }

    private void ParseTraits(TestTraitCollection collection, XElement element)
    {
        var propertiesNode = element.Element(testbucket + "Properties");
        var traitsNode = element.Element(testbucket + "Traits");

        if (propertiesNode is not null)
        {
            foreach (var propertyNode in propertiesNode.Elements(testbucket + "Property"))
            {
                var name = propertyNode.Attribute("name")?.Value;
                var value = propertyNode.Attribute("value")?.Value;
                if (name is not null && value is not null)
                {
                    var attributeType = TestTraitHelper.GetTestTraitType(name);
                    collection.Traits.Add(new TestTrait(attributeType, name, value) { ExportType = TraitExportType.Instance });
                }
            }
        }
        if (traitsNode is not null)
        {
            foreach (var propertyNode in traitsNode.Elements(testbucket + "Trait"))
            {
                var name = propertyNode.Attribute("name")?.Value;
                var value = propertyNode.Attribute("value")?.Value;
                if (name is not null && value is not null)
                {
                    var attributeType = TestTraitHelper.GetTestTraitType(name);
                    collection.Traits.Add(new TestTrait(attributeType, name, value) { ExportType = TraitExportType.Static });
                }
            }
        }
    }

    private void AddTraits(TraitExportType exportType, XElement element, TestTraitCollection collection)
    {
        var items = collection.Traits.Where(x => x.ExportType == exportType && !_nativeAttributes.Contains(x.Type)).ToArray();
        if (items.Length > 0)
        {
            XElement traitsElement = exportType switch
            {
                TraitExportType.Static => new XElement(testbucket + "Traits"),
                _ => new XElement(testbucket + "Properties")
            };

            foreach(var trait in items)
            {
                XElement traitElement = exportType switch
                {
                    TraitExportType.Static => new XElement(testbucket + "Trait"),
                    _ => new XElement(testbucket + "Property")
                };

                var name = TestTraitHelper.GetTraitName(trait);
                if (name is not null)
                {
                    traitElement.Add(new XAttribute("name", name), new XAttribute("value", trait.Value));
                    traitsElement.Add(traitElement);
                }
            }

            element.Add(traitsElement);
        }
    }

    private void AddTestDefinitions(TestRunDto testRun, XElement testRunElement)
    {
        var testDefinitionsElement = new XElement(mstest + "TestDefinitions");
        foreach (var suite in testRun.Suites)
        {
            foreach (var test in suite.Tests)
            {
                var unitTest = new XElement(mstest + "UnitTest",
                    new XAttribute("name", test.Name ?? ""),
                    new XAttribute("id", test.ExternalId ?? throw new InvalidDataException("Expected ExternalId on TestCaseRunDto")));

                unitTest.Add(new XElement(mstest + "Execution", new XAttribute("id", test.InstanceId ?? "")));
                unitTest.Add(new XElement(mstest + "TestMethod",
                    new XAttribute("name", test.Method ?? ""),
                    new XAttribute("className", test.ClassName ?? ""),
                    new XAttribute("codeBase", test.Assembly ?? "")
                    ));

                AddTraits(TraitExportType.Static, unitTest, test);

                testDefinitionsElement.Add(unitTest);
            }
        }
        testRunElement.Add(testDefinitionsElement);
    }
}
