using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brightest.Testing.Domain.Formats.Xml.XUnit;

using TestBucket.Domain.Testing.Formats;
using TestBucket.Domain.Testing.Formats.Dtos;
using TestBucket.Domain.Testing.Formats.JUnit;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing;

internal class TextImporter : ITextTestResultsImporter
{
    private readonly ITestCaseRepository _testCaseRepository;

    public TextImporter(ITestCaseRepository testCaseRepository)
    {
        _testCaseRepository = testCaseRepository;
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
            if(test.ClassName is not null)
            {
                long? parentId = null;
                string folderName = test.ClassName;
                TestSuiteFolder? folder = await _testCaseRepository.GetTestSuiteFolderByNameAsync(tenantId, suite.Id, parentId, folderName);
                if(folder is null)
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
            };
            await _testCaseRepository.AddTestCaseAsync(testCase);
        }

        return testCase;
    }
}
