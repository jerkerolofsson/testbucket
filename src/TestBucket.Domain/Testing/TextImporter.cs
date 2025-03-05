using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Fields;
using TestBucket.Domain.Testing.Models;
using TestBucket.Formats;
using TestBucket.Formats.Dtos;
using TestBucket.Formats.JUnit;
using TestBucket.Formats.XUnit;

namespace TestBucket.Domain.Testing;

internal class TextImporter : ITextTestResultsImporter
{
    private readonly ITestCaseRepository _testCaseRepository;
    private readonly IFieldRepository _fieldRepository;

    public TextImporter(ITestCaseRepository testCaseRepository, IFieldRepository fieldRepository)
    {
        _testCaseRepository = testCaseRepository;
        _fieldRepository = fieldRepository;
    }

    public async Task ImportTextAsync(string tenantId, long? teamId, long? projectId, TestResultFormat format, string text)
    {
        ITestResultSerializer serializer = format switch
        {
            TestResultFormat.JUnitXml => new JUnitSerializer(),
            TestResultFormat.xUnitXml => new XUnitSerializer(),
            _ => throw new ArgumentException()
        };

        var run = serializer.Deserialize(text);

        await ImportRunAsync(tenantId, teamId, projectId, run);
    }

    private async Task ImportRunAsync(string tenantId, long? teamId, long? projectId, TestRunDto run)
    {
        await ImportTestCasesAsync(tenantId, teamId, projectId, run);
    }

    private async Task ImportTestCasesAsync(string tenantId, long? teamId, long? projectId, TestRunDto run)
    {
        if (run.Suites is not null)
        {
            foreach (var runSuite in run.Suites)
            {
                var suiteName = runSuite.Name ?? run.Name;
                if (runSuite.Tests is null || suiteName is null)
                {
                    continue;
                }
                TestSuite? suite = await _testCaseRepository.GetTestSuiteByNameAsync(tenantId, teamId, projectId, suiteName);
                if (suite is null)
                {
                    suite = await _testCaseRepository.AddTestSuiteAsync(tenantId, teamId, projectId, suiteName);
                }

                // Add test suite run
                TestRun testRun = new TestRun()
                {
                    Name = suiteName,
                    TenantId = tenantId,
                    TestProjectId = projectId,
                    ExternalId = run.ExternalId,
                    Description = "Imported",
                    SystemOut = run.SystemOut,
                };
                await _testCaseRepository.AddTestRunAsync(testRun);

                // Get field definitions to map traits to them
                var fieldDefinitions = await _fieldRepository.SearchAsync(tenantId, new SearchQuery { ProjectId = projectId });

                foreach (var test in runSuite.Tests)
                {
                    // Get the existing case or create a new one
                    if (test.ExternalId is null)
                    {
                        // Logwarn
                    }
                    else
                    {
                        TestCase testCase = await GetOrCreateTestCaseAsync(tenantId, projectId, suite, test);
                        await AddTestCaseRunAsync(tenantId, testRun, test, testCase);

                        // Add traits
                        foreach(var trait in test.Traits)
                        {
                            var fieldDefinition = fieldDefinitions.Where(x => x.Trait == trait.Name || x.Name == trait.Name).FirstOrDefault();  
                            if(fieldDefinition is not null)
                            {
                                var field = new TestCaseField
                                {
                                    TenantId = tenantId,
                                    TestCaseId = testCase.Id,
                                    FieldDefinitionId = fieldDefinition.Id,
                                    StringValue = trait.Value
                                };

                                // Todo: Extract conversion method
                                if(fieldDefinition.Type == Fields.Models.FieldType.Boolean)
                                {
                                    field.BooleanValue = trait.Value == "true";
                                }

                                await _fieldRepository.UpsertTestCaseFieldsAsync(field);
                            }
                        }
                    }
                }
            }
        }
    }

    private async Task AddTestCaseRunAsync(string tenantId, TestRun testRun, TestCaseRunDto test, TestCase testCase)
    {
        var testCaseRun = new TestCaseRun()
        {
            Name = test.Name ?? "-",
            TenantId = tenantId,
            TestRunId = testRun.Id,
            Result = test.Result ?? TestResult.NoRun,
            TestCaseId = testCase.Id,
            Duration = (int)(test.Duration?.TotalMicroseconds ?? 0),
            CallStack = test.CallStack,
            Message = test.Message,
        };

        await _testCaseRepository.AddTestCaseRunAsync(testCaseRun);
    }

    private async Task<TestCase> GetOrCreateTestCaseAsync(string tenantId, long? projectId, TestSuite suite, TestCaseRunDto test)
    {
        TestCase? testCase = await _testCaseRepository.GetTestCaseByExternalIdAsync(tenantId, suite.Id, test.ExternalId);
        if (testCase is null)
        {
            long? folderId = null;
            if (test.ClassName is not null)
            {
                long? parentId = null;
                string folderName = test.ClassName;
                TestSuiteFolder? folder = await _testCaseRepository.GetTestSuiteFolderByNameAsync(tenantId, suite.Id, parentId, folderName);
                if (folder is null)
                {
                    folder = await _testCaseRepository.AddTestSuiteFolderAsync(tenantId, projectId, suite.Id, parentId, folderName);
                }
                folderId = folder?.Id;
            }
            testCase = new TestCase
            {
                Name = test.Name ?? "",
                ExternalId = test.ExternalId,
                ClassName = test.ClassName,
                Method = test.Method,
                Module = test.Module,
                TenantId = tenantId,
                TestProjectId = projectId,
                TestSuiteId = suite.Id,
                TestSuiteFolderId = folderId,
                Description = test.Description,
            };
            await _testCaseRepository.AddTestCaseAsync(testCase);
        }
        else
        {
            bool changed = false;
            if (string.IsNullOrEmpty(testCase.Description) && !string.IsNullOrEmpty(test.Description))
            {
                testCase.Description = test.Description;
                changed = true;
            }
            if (changed)
            {
                await _testCaseRepository.UpdateTestCaseAsync(testCase);
            }
        }

        return testCase;
    }
}
