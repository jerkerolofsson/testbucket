﻿
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Files;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.States;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Services;
using TestBucket.Domain.Testing.Specifications.TestCases;
using TestBucket.Formats;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Testing.Services.Import;

internal class TextImporter : ITextTestResultsImporter
{
    private readonly IStateService _stateService;
    private readonly ITestCaseRepository _testCaseRepository;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly IFileRepository _fileRepository;
    private readonly ITestSuiteManager _testSuiteManager;
    private readonly ITestRunManager _testRunManager;
    private readonly ITestCaseManager _testCaseManager;
    private readonly IFieldManager _fieldManager;

    public TextImporter(
        IStateService stateService,
        ITestCaseRepository testCaseRepository,
        IFieldDefinitionManager fieldDefinitionManager,
        IFileRepository fileRepository,
        ITestSuiteManager testSuiteManager,
        ITestRunManager testRunManager,
        ITestCaseManager testCaseManager,
        IFieldManager fieldManager)
    {
        _stateService = stateService;
        _testCaseRepository = testCaseRepository;
        _fieldDefinitionManager = fieldDefinitionManager;
        _fileRepository = fileRepository;
        _testSuiteManager = testSuiteManager;
        _testRunManager = testRunManager;
        _testCaseManager = testCaseManager;
        _fieldManager = fieldManager;
    }

    /// <summary>
    /// Imports a text based test result document
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="teamId"></param>
    /// <param name="projectId"></param>
    /// <param name="format"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task ImportTextAsync(ClaimsPrincipal principal, long teamId, long projectId, TestResultFormat format, string text, ImportHandlingOptions options)
    {
        if(format == TestResultFormat.UnknownFormat)
        {
            format = TestResultDetector.Detect(Encoding.UTF8.GetBytes(text));
        }

        var serializer = TestResultSerializerFactory.Create(format);
        var run = serializer.Deserialize(text);
        await ImportRunAsync(principal, teamId, projectId, run, options);
    }

    public async Task ImportRunAsync(ClaimsPrincipal principal, long teamId, long projectId, TestRunDto run, ImportHandlingOptions options)
    {
        var tenantId = principal.GetTenantIdOrThrow();

        if (run.Suites is not null)
        {
            // Add test suite run
            TestRun testRun = await ResolveTestRunAsync(principal, teamId, projectId, run, options);

            // Copy some attributes from tests to the run
            AssignRunPropertiesFromTests(run);
            AssignRunPropertiesFromSuites(run);

            // Update test run
            testRun.SystemOut = run.SystemOut;
            //testRun.SystemErr = run.SystemErr;
            await _testRunManager.SaveTestRunAsync(principal, testRun);

            foreach (var runSuite in run.Suites)
            {
                // Create a new test suite if it doesn't exist
                var suiteName = runSuite.Name ?? run.Name;
                if (options.CreateTestSuiteFromAssemblyName && runSuite.Assembly is not null)
                {
                    suiteName = runSuite.Assembly;
                }

                if (runSuite.Tests is null || suiteName is null)
                {
                    continue;
                }

                // Get or create the test suite
                TestSuite? suite = null;
                if(options.TestSuiteId is not null)
                {
                    suite = await _testSuiteManager.GetTestSuiteByIdAsync(principal, options.TestSuiteId.Value);
                }
                if (suite is null)
                {
                    suite = await ResolveTestSuiteAsync(principal, teamId, projectId, options, suiteName);
                }

                // Get field definitions to imported entities
                var testRunFieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, projectId, FieldTarget.TestRun);
                var testCaseFieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, projectId, FieldTarget.TestCase);
                var testCaseRunFieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, projectId, FieldTarget.TestCaseRun);

                foreach (var test in runSuite.Tests)
                {
                    // Get the existing case or create a new one
                    if (test.ExternalId is null)
                    {
                        // Todo: Logwarn?
                    }
                    else
                    {
                        // Get or create the test case and add a test case run
                        TestCase? testCase = null;
                        if (options.TestCaseId is not null)
                        {
                            testCase = await _testCaseRepository.GetTestCaseByIdAsync(tenantId, options.TestCaseId.Value);
                        }
                        if (testCase is null)
                        {
                            testCase = await GetOrCreateTestCaseAsync(principal, projectId, suite, test, options);
                        }

                        // Add traits to the test case
                        foreach (var traitName in test.Traits.Select(x => x.Name))
                        {
                            // Get all traits for the specific name
                            var traits = test.Traits.Where(x => x.Name == traitName).ToArray();
                            await UpsertTestCaseTraitsAsync(principal, testCaseFieldDefinitions, testCase, traits);
                        }

                        TestCaseRun testCaseRun = await AddTestCaseRunAsync(principal, testRun, test, testCase, testCaseFieldDefinitions, testRunFieldDefinitions, testCaseRunFieldDefinitions);
                    }
                }
            }
        }
    }

    private static void AssignRunPropertiesFromSuites(TestRunDto run)
    {
        if (string.IsNullOrEmpty(run.SystemOut))
        {
            var stdout = new StringBuilder();

            foreach (var suite in run.Suites)
            {
                if (!string.IsNullOrEmpty(suite.SystemOut))
                {
                    stdout.AppendLine(suite.SystemOut);
                }
            }
            run.SystemOut = stdout.ToString();
        }

        if (string.IsNullOrEmpty(run.SystemErr))
        {
            var stderr = new StringBuilder();

            foreach (var suite in run.Suites)
            {
                if (!string.IsNullOrEmpty(suite.SystemErr))
                {
                    stderr.AppendLine(suite.SystemErr);
                }
            }
            run.SystemErr = stderr.ToString();
        }
    }
    private static void AssignRunPropertiesFromTests(TestRunDto run)
    {
        foreach (var suite in run.Suites)
        {
            foreach (var test in suite.Tests)
            {
                if (!string.IsNullOrEmpty(test.Assembly))
                {
                    suite.Assembly = test.Assembly;
                }
            }
        }
    }

    private async Task<TestRun> ResolveTestRunAsync(ClaimsPrincipal principal, long? teamId, long? projectId, TestRunDto run, ImportHandlingOptions options)
    {
        var tenantId = principal.GetTenantIdOrThrow();

        var now = DateTimeOffset.UtcNow;
        var dateString = now.ToLocalTime().ToString("yyyy-MM-dd HHmmss");

        TestRun testRun;
        if (options.TestRunId is null)
        {
            // Create a new test run
            testRun = new TestRun()
            {
                Created = now,
                Name = (run.Name ?? "Test Results") + " - " + dateString,
                TeamId = teamId,
                TenantId = tenantId,
                TestProjectId = projectId,
                ExternalId = run.ExternalId,
                Description = "Imported",
                SystemOut = run.SystemOut,
            };
            await _testRunManager.AddTestRunAsync(principal, testRun);
        }
        else
        {
            var existingRun = await _testRunManager.GetTestRunByIdAsync(principal, options.TestRunId.Value);
            if (existingRun is null)
            {
                throw new InvalidOperationException("Test run not found!");
            }

            testRun = existingRun;
        }

        return testRun;
    }

    private async Task<TestSuite> ResolveTestSuiteAsync(ClaimsPrincipal principal, long? teamId, long? projectId, ImportHandlingOptions options, string suiteName)
    {
        TestSuite? suite = null;
        if (options.TestCaseId is not null)
        {
            var testCase = await _testCaseRepository.GetTestCaseByIdAsync(principal.GetTenantIdOrThrow(), options.TestCaseId.Value);
            if (testCase is not null)
            {
                suite = await _testSuiteManager.GetTestSuiteByIdAsync(principal, testCase.TestSuiteId);
            }
        }
        if (suite is null)
        {
            // Get or create a tesat suite
            suite = await _testSuiteManager.GetTestSuiteByNameAsync(principal, teamId, projectId, suiteName);
            suite ??= await _testSuiteManager.AddTestSuiteAsync(principal, teamId, projectId, suiteName);
        }

        return suite;
    }

    /// <summary>
    /// Updates test case fields from result file
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testCaseFieldDefinitions"></param>
    /// <param name="testCase"></param>
    /// <param name="traits"></param>
    /// <returns></returns>
    private async Task UpsertTestCaseTraitsAsync(ClaimsPrincipal principal, IReadOnlyList<FieldDefinition> testCaseFieldDefinitions, TestCase testCase, TestTrait[] traits)
    {
        string tenantId = principal.GetTenantIdOrThrow();

        if (traits.Length == 0)
        {
            return;
        }
        var trait = traits[0];

        var fieldDefinition = testCaseFieldDefinitions.Where(x => x.Trait == trait.Name || x.Name == trait.Name).FirstOrDefault();
        if (fieldDefinition is not null)
        {
            // Create a new field
            var field = new TestCaseField
            {
                TenantId = tenantId,
                TestCaseId = testCase.Id,
                FieldDefinitionId = fieldDefinition.Id,
            };
            
            // In the test case, a trait can have multiple values, but we may save them as an array if that's the
            // target field
            var values = traits.Select(x => x.Value).ToArray();
            if (FieldValueConverter.TryAssignValue(fieldDefinition, field, values))
            {
                await _fieldDefinitionManager.UpsertTestCaseFieldsAsync(principal, field);
            }
        }
    }

    private async Task<TestCaseRun> AddTestCaseRunAsync(
        ClaimsPrincipal principal, 
        TestRun testRun, 
        TestCaseRunDto test, 
        TestCase testCase, 
        IReadOnlyList<FieldDefinition> testCaseFieldDefinitions, 
        IReadOnlyList<FieldDefinition> testRunFieldDefinitions, 
        IReadOnlyList<FieldDefinition> testCaseRunFieldDefinitions)
    {
        if (testRun.TestProjectId is null)
        {
            throw new InvalidOperationException("Test runs must have a project");
        }
        var tenantId = principal.GetTenantIdOrThrow();

        var testName = testCase.Name ?? "-";
        var completedState = await _stateService.GetProjectFinalStateAsync(principal, testRun.TestProjectId.Value);
        var testCaseRun = new TestCaseRun()
        {
            Name = testName,
            TenantId = tenantId,
            TestRunId = testRun.Id,
            TestProjectId = testRun.TestProjectId.Value,
            Result = test.Result ?? TestResult.NoRun,
            TestCaseId = testCase.Id,
            Duration = (int)(test.Duration?.TotalMicroseconds ?? 0),
            CallStack = test.CallStack,
            SystemOut = test.SystemOut,
            SystemErr = test.SystemErr,
            Message = test.Message,
            State = completedState.Name
        };

        await _testCaseRepository.AddTestCaseRunAsync(testCaseRun);

        // Add traits from the test case (could be manually assigned)
        List<TestCaseRunField> fields = new();

        TestCaseRunFieldHelper.BuildInheritedFields(testRun, testCase, testCaseFieldDefinitions, testRunFieldDefinitions, testCaseRun, fields);

        await _fieldManager.SaveTestCaseRunFieldsAsync(principal, fields);

        await AddAttachmentsAsync(testCaseRun, test.Attachments);

        return testCaseRun;
    }


    private async Task AddAttachmentsAsync(TestCaseRun testCaseRun, List<AttachmentDto>? attachments)
    {
        Debug.Assert(testCaseRun.TenantId != null);

        if(attachments is not null)
        {
            foreach(var attachment in attachments)
            {
                var data = attachment.Data ?? [];
                var dbo = new FileResource()
                {
                    Created = DateTimeOffset.UtcNow,
                    ContentType = attachment.ContentType ?? "application/octet-stream",
                    Data = data,
                    TenantId = testCaseRun.TenantId!,
                    TestCaseRunId = testCaseRun.Id,
                    Name = attachment.Name ?? "Attachment",
                    Category = ResourceCategory.Attachment,
                    Length = data.Length
                };

                await _fileRepository.AddResourceAsync(dbo);
            }
        }
    }

    private async Task<TestCase> GetOrCreateTestCaseAsync(ClaimsPrincipal principal, long projectId, TestSuite suite, TestCaseRunDto test, ImportHandlingOptions options)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        // We try to map the test case by the external ID first.
        // This allows tests to define [TestId] traits for simplicity

        if (test.ExternalId is not null)
        {
            FilterSpecification<TestCase>[] filters = [
                new FilterByTenant<TestCase>(tenantId),
                //new FilterTestCasesByTestSuite(suite.Id),
                new FilterByProject<TestCase>(projectId),
                new FilterTestCasesByExternalId(test.ExternalId)
            ];
            var testCase = (await _testCaseRepository.SearchTestCasesAsync(0, 1, filters)).Items.FirstOrDefault();
            if (testCase is not null)
            {
                return testCase;
            }
        }

        //TestCase? testCase = await _testCaseRepository.GetTestCaseByExternalIdAsync(tenantId, test.ExternalId);

        var className = test.ClassName;
        var method = test.Method;
        var module = test.Module;
        var assemblyName = test.Assembly;
        if (className is not null || method is not null || assemblyName is not null || module is not null)
        {
            FilterSpecification<TestCase>[] filters = [
                new FilterByTenant<TestCase>(tenantId),
                new FilterTestCasesByTestSuite(suite.Id),
                new FilterByProject<TestCase>(projectId),
                new FilterTestCasesByAutomationImplementation(className, method, module, assemblyName)
            ];
            var testCase = (await _testCaseRepository.SearchTestCasesAsync(0, 1, filters)).Items.FirstOrDefault();
            if (testCase is not null)
            {
                return testCase;
            }
        }

        // Do we need to create a new test case?
        var newTestCase = await CreateNewTestCaseAsync(principal, projectId, suite, test, options);
        return newTestCase;
    }

    private async Task<TestCase> CreateNewTestCaseAsync(ClaimsPrincipal principal, long? projectId, TestSuite suite, TestCaseRunDto test, ImportHandlingOptions options)
    {
        var tenantId = principal.GetTenantIdOrThrow();

        // Create folder for the test case
        long? folderId = null;
        if (options.CreateFoldersFromClassNamespace)
        {
            if (test.ClassName is not null)
            {
                test.Folders = test.ClassName.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                folderId = await GetOrCreateTestCaseFolderPathAsync(tenantId, projectId, suite.Id, test.Folders);
            }
        }

        if (folderId is null)
        {
            if (test.Folders.Length == 0 && test.ClassName is not null)
            {
                test.Folders = [test.ClassName];
            }
            folderId = await GetOrCreateTestCaseFolderPathAsync(tenantId, projectId, suite.Id, test.Folders);
        }

        var testName = test.Name ?? "";
        if (options.RemoveClassNameFromTestName && test.ClassName is not null)
        {
            if (testName.StartsWith(test.ClassName))
            {
                testName = testName[test.ClassName.Length..].TrimStart('.', '/');
            }
        }

        var testCase = new TestCase
        {
            Name = testName,
            ExternalId = test.TestId ?? test.ExternalId,
            AutomationAssembly = test.Assembly,
            ClassName = test.ClassName,
            Method = test.Method,
            Module = test.Module,
            TenantId = tenantId,
            TeamId = suite.TeamId,
            TestProjectId = projectId,
            TestSuiteId = suite.Id,
            TestSuiteFolderId = folderId,
            Description = test.Description,
            ExecutionType = TestExecutionType.Automated
        };
        await _testCaseManager.AddTestCaseAsync(principal, testCase);
        return testCase;
    }

    private async Task<long?> GetOrCreateTestCaseFolderPathAsync(string tenantId, long? projectId, long testSuiteId, string[] folderNames)
    {
        long? id = null;

        foreach (var folderName in folderNames)
        {
            TestSuiteFolder folder = await _testCaseRepository.AddTestSuiteFolderAsync(tenantId, projectId, testSuiteId, parentFolderId: id, folderName);
            id = folder.Id;
        }

        return id;
    }
}
