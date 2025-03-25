
using System.Diagnostics;
using System.Security.Claims;

using TestBucket.Domain.Fields;
using TestBucket.Domain.Fields.Models;
using TestBucket.Domain.Files;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.States;
using TestBucket.Domain.Teams.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Formats;
using TestBucket.Formats.Ctrf;
using TestBucket.Formats.Dtos;
using TestBucket.Formats.JUnit;
using TestBucket.Formats.XUnit;

namespace TestBucket.Domain.Testing;

internal class TextImporter : ITextTestResultsImporter
{
    private readonly IStateService _stateService;
    private readonly ITestCaseRepository _testCaseRepository;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly IFileRepository _fileRepository;
    private readonly ITestSuiteManager _testSuiteManager;
    private readonly ITestRunManager _testRunManager;

    public TextImporter(
        IStateService stateService,
        ITestCaseRepository testCaseRepository, IFieldDefinitionManager fieldDefinitionManager, IFileRepository fileRepository, ITestSuiteManager testSuiteManager, ITestRunManager testRunManager)
    {
        _stateService = stateService;
        _testCaseRepository = testCaseRepository;
        _fieldDefinitionManager = fieldDefinitionManager;
        _fileRepository = fileRepository;
        _testSuiteManager = testSuiteManager;
        _testRunManager = testRunManager;
    }

    /// <summary>
    /// Imports a text based test result document
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="teamId"></param>
    /// <param name="projectId"></param>
    /// <param name="format"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task ImportTextAsync(ClaimsPrincipal principal, long? teamId, long? projectId, TestResultFormat format, string text, ImportHandlingOptions options)
    {
        ITestResultSerializer serializer = format switch
        {
            TestResultFormat.JUnitXml => new JUnitSerializer(),
            TestResultFormat.xUnitXml => new XUnitSerializer(),
            TestResultFormat.CommonTestReportFormat => new CtrfXunitSerializer(),
            _ => throw new ArgumentException("Unknown format", nameof(format))
        };    

        var run = serializer.Deserialize(text);
        await ImportRunAsync(principal, teamId, projectId, run, options);
    }

    private async Task ImportRunAsync(ClaimsPrincipal principal, long? teamId, long? projectId, TestRunDto run, ImportHandlingOptions options)
    {
        var tenantId = principal.GetTentantIdOrThrow();

        if (run.Suites is not null)
        {
            var now = DateTimeOffset.UtcNow;
            var dateString = now.ToLocalTime().ToString("yyyy-MM-dd HHmmss");

            // Add test suite run
            TestRun testRun = new TestRun()
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

            foreach (var runSuite in run.Suites)
            {
                // Create a new test suite if it doesn't exist
                var suiteName = runSuite.Name ?? run.Name;
                if(options.CreateTestSuiteFromAssemblyName && runSuite.Assembly is not null)
                {
                    suiteName = runSuite.Assembly;
                }

                if (runSuite.Tests is null || suiteName is null)
                {
                    continue;
                }
                TestSuite? suite = await _testSuiteManager.GetTestSuiteByNameAsync(principal, teamId, projectId, suiteName);
                suite ??= await _testSuiteManager.AddTestSuiteAsync(principal, teamId, projectId, suiteName);

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
                        TestCase testCase = await GetOrCreateTestCaseAsync(tenantId, projectId, suite, test, options);
                        TestCaseRun testCaseRun = await AddTestCaseRunAsync(principal, testRun, test, testCase);

                        // Add traits to the test case
                        foreach(var traitName in test.Traits.Select(x=>x.Name))
                        {
                            // Get all traits for the specific name
                            var traits = test.Traits.Where(x => x.Name == traitName).ToArray();
                            await UpsertTestCaseTraitsAsync(principal, testCaseFieldDefinitions, testCase, traits);
                        }
                    }
                }
            }
        }
    }

    private async Task UpsertTestCaseTraitsAsync(ClaimsPrincipal principal, IReadOnlyList<FieldDefinition> testCaseFieldDefinitions, TestCase testCase, TestTrait[] traits)
    {
        string tenantId = principal.GetTentantIdOrThrow();

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
        else
        {
            // Update?
        }
    }

    private async Task<TestCaseRun> AddTestCaseRunAsync(ClaimsPrincipal principal, TestRun testRun, TestCaseRunDto test, TestCase testCase)
    {
        if(testRun.TestProjectId is null)
        {
            throw new InvalidOperationException("Test runs must have a project");
        }
        var tenantId = principal.GetTentantIdOrThrow();

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
            Message = test.Message,
            State = completedState.Name
        };

        await _testCaseRepository.AddTestCaseRunAsync(testCaseRun);

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
                var dbo = new FileResource()
                {
                    ContentType = attachment.ContentType ?? "application/octet-stream",
                    Data = attachment.Data ?? [],
                    TenantId = testCaseRun.TenantId!,
                    TestCaseRunId = testCaseRun.Id,
                    Name = attachment.Name ?? "Attachment",
                    Category = ResourceCategory.Attachment,
                };

                await _fileRepository.AddResourceAsync(dbo);
            }
        }
    }

    private async Task<TestCase> GetOrCreateTestCaseAsync(string tenantId, long? projectId, TestSuite suite, TestCaseRunDto test, ImportHandlingOptions options)
    {
        // We try to map the test case by the external ID first.
        // This allows tests to define [TestId] traits for simplicity
        TestCase? testCase = await _testCaseRepository.GetTestCaseByExternalIdAsync(tenantId, test.ExternalId);

        if(testCase is null)
        {
            var className = test.ClassName;
            var method = test.Method;
            var module = test.Module;
            var assemblyName = test.Assembly;
            if (className is not null || method is not null || assemblyName is not null || module is not null)
            {
                testCase = await _testCaseRepository.GetTestCaseByAutomationImplementationAttributesAsync(tenantId, assemblyName, module, className, method);
            }
        }

        // Do we need to create a new test case?
        if (testCase is null)
        {
            testCase = await CreateNewTestCaseAsync(tenantId, projectId, suite, test, options);
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

    private async Task<TestCase> CreateNewTestCaseAsync(string tenantId, long? projectId, TestSuite suite, TestCaseRunDto test, ImportHandlingOptions options)
    {
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
            TestProjectId = projectId,
            TestSuiteId = suite.Id,
            TestSuiteFolderId = folderId,
            Description = test.Description,
        };
        await _testCaseRepository.AddTestCaseAsync(testCase);
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
