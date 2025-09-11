using System.Diagnostics;
using System.Security.Principal;
using System.Text;

using Mediator;

using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Files;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Metrics;
using TestBucket.Domain.Metrics.Models;
using TestBucket.Domain.Progress;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.States;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Tenants.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCases;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Formats;
using TestBucket.Formats.Dtos;
using TestBucket.Traits.Core;
using TestBucket.Traits.Core.Metrics;

namespace TestBucket.Domain.Testing.Services.Import;

public record class ImportRunRequest(ClaimsPrincipal Principal, TestRunDto Run, ImportHandlingOptions Options) : IRequest<TestRun>;

public class ImportRunHandler : IRequestHandler<ImportRunRequest, TestRun>
{
    private readonly IStateService _stateService;
    private readonly ITestCaseRepository _testCaseRepository;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly IFileRepository _fileRepository;
    private readonly ITestSuiteManager _testSuiteManager;
    private readonly ITestRunManager _testRunManager;
    private readonly ITestCaseManager _testCaseManager;
    private readonly IFieldManager _fieldManager;
    private readonly IProjectManager _projectManager;
    private readonly ITeamManager _teamManager;
    private readonly IRequirementManager _requirementManager;
    private readonly IProgressManager _progressManager;
    private readonly IMetricsManager _metricsManager;
    private readonly ILogger<ImportRunHandler> _logger;

    public ImportRunHandler(
        IStateService stateService,
        ITestCaseRepository testCaseRepository,
        IFieldDefinitionManager fieldDefinitionManager,
        IFileRepository fileRepository,
        ITestSuiteManager testSuiteManager,
        ITestRunManager testRunManager,
        ITestCaseManager testCaseManager,
        IFieldManager fieldManager,
        IProjectManager projectManager,
        ITeamManager teamManager,
        IRequirementManager requirementManager,
        IProgressManager progressManager,
        ILogger<ImportRunHandler> logger,
        IMetricsManager metricsManager)
    {
        _stateService = stateService;
        _testCaseRepository = testCaseRepository;
        _fieldDefinitionManager = fieldDefinitionManager;
        _fileRepository = fileRepository;
        _testSuiteManager = testSuiteManager;
        _testRunManager = testRunManager;
        _testCaseManager = testCaseManager;
        _fieldManager = fieldManager;
        _projectManager = projectManager;
        _teamManager = teamManager;
        _requirementManager = requirementManager;
        _progressManager = progressManager;
        _logger = logger;
        _metricsManager = metricsManager;
    }
    public async ValueTask<TestRun> Handle(ImportRunRequest request, CancellationToken cancellationToken)
    {
        var run = request.Run;
        ArgumentException.ThrowIfNullOrEmpty(run.Team);
        ArgumentException.ThrowIfNullOrEmpty(run.Project);

        var principal = request.Principal;
        var team = await _teamManager.GetTeamBySlugAsync(principal, run.Team);
        var project = await _projectManager.GetTestProjectBySlugAsync(principal, run.Project);

        if (project is null)
        {
            throw new ArgumentException("project not found");
        }
        if (team is null)
        {
            throw new ArgumentException("Team not found");
        }

        return await ImportRunAsync(principal, team.Id, project.Id, run, request.Options);
    }


    public async Task<TestRun> ImportRunAsync(
        ClaimsPrincipal principal, 
        long teamId, 
        long projectId, 
        TestRunDto run, 
        ImportHandlingOptions options)
    {
        await using var progress = _progressManager.CreateProgressTask("Import Test Results");

        await progress.ReportStatusAsync("Loading field definitions", 0);
        var tenantId = principal.GetTenantIdOrThrow();
        var testRunFieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, projectId, FieldTarget.TestRun);
        var testCaseFieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, projectId, FieldTarget.TestCase);
        var testCaseRunFieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, projectId, FieldTarget.TestCaseRun);

        // Add test suite run
        await progress.ReportStatusAsync("Creating run", 0);
        TestRun testRun = await ResolveTestRunAsync(principal, teamId, projectId, run, options);

        if (run.Suites is not null)
        {
            // Copy some attributes from tests to the run
            AssignRunPropertiesFromTests(run);
            AssignRunPropertiesFromSuites(run);
            await UpdateTestRunFieldsAsync(principal, testRun, run, testRunFieldDefinitions);

            // Update test run
            testRun.SystemOut = run.SystemOut;
            await _testRunManager.SaveTestRunAsync(principal, testRun);

            foreach (var runSuite in run.Suites)
            {
                // Create a new test suite if it doesn't exist
                var suiteName = runSuite.Name ?? run.Name;
                if (options.CreateTestSuiteFromAssemblyName && runSuite.Assembly is not null)
                {
                    suiteName = runSuite.Assembly;
                    if(suiteName.EndsWith(".dll"))
                    {
                        suiteName = suiteName[0..^4];
                    }
                }

                if (runSuite.Tests is null || suiteName is null)
                {
                    continue;
                }

                // Get or create the test suite
                TestSuite? suite = null;
                if (options.TestSuiteId is not null)
                {
                    suite = await _testSuiteManager.GetTestSuiteByIdAsync(principal, options.TestSuiteId.Value);
                }
                if (suite is null)
                {
                    await progress.ReportStatusAsync("Creating test suite", 0);
                    suite = await ResolveTestSuiteAsync(principal, teamId, projectId, options, suiteName);
                }

                foreach (var testIndex in runSuite.Tests.Index())
                {
                    var test = testIndex.Item;
                    var startTimestamp = Stopwatch.GetTimestamp();
                    _logger.LogInformation("Importing result for {TestName}", test.Name);
                    await progress.ReportStatusAsync($"Importing {test.Name}", (testIndex.Index*100.0/(double)runSuite.Tests.Count));

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
                        _logger.LogInformation("Updating test case traits for {TestName}, {ElapsedMillis}ms", test.Name, (int)Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds);
                        foreach (var traitName in test.Traits.Select(x => x.Name))
                        {
                            // Get all traits for the specific name
                            var traits = test.Traits.Where(x => x.Name == traitName).ToArray();
                            await UpsertTestCaseTraitsAsync(principal, testCaseFieldDefinitions, testCase, traits);
                        }

                        _logger.LogInformation("Creating test case run for {TestName}, {ElapsedMillis}ms", test.Name, (int)Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds);
                        TestCaseRun testCaseRun = await AddTestCaseRunAsync(principal, testRun, test, testCase, testCaseFieldDefinitions, testRunFieldDefinitions, testCaseRunFieldDefinitions, options);

                        _logger.LogInformation("Updating requirements for {TestName}, {ElapsedMillis}ms", test.Name, (int)Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds);
                        await LinkWithRequirementsAsync(principal, testCase, test.Traits);
                        await AddMetricsAsync(principal, test, testCaseRun);

                        _logger.LogInformation("Importing result for {TestName}, completed in {ElapsedMillis}ms", test.Name, (int)Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds);
                    }
                }
            }
        }
        return testRun;
    }

    private async Task AddMetricsAsync(ClaimsPrincipal principal, TestCaseRunDto test, TestCaseRun testCaseRun)
    {
        if(test.Traits is null)
        {
            return;
        }
        foreach (var trait in test.Traits.Where(x => x.Name?.StartsWith("metric:") == true))
        {
            try
            {
                var testMetric = MetricSerializer.Deserialize(trait.Name, trait.Value);

                var metric = new Metric
                {
                    MeterName = testMetric.MeterName,
                    Name = testMetric.Name,
                    Value = testMetric.Value,
                    Unit = testMetric.Unit,
                    Created = testMetric.Created,
                    TestCaseRunId = testCaseRun.Id,
                    TestProjectId = testCaseRun.TestProjectId,
                    TeamId = testCaseRun.TeamId,
                    TenantId = testCaseRun.TenantId,
                };
                await _metricsManager.AddMetricAsync(principal, metric);
            }
            catch (FormatException ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize metric from trait {TraitName} with value {TraitValue}", trait.Name, trait.Value);
            }
        }
    }

    private async Task LinkWithRequirementsAsync(ClaimsPrincipal principal, TestCase testCase, List<TestTrait> traits)
    {
        foreach(var coveredRequirementTrait in traits.Where(x => x.Type == TraitType.CoveredRequirement))
        {
            var requirement = await _requirementManager.GetRequirementBySlugAsync(principal, coveredRequirementTrait.Value);
            if(requirement is null)
            {
                requirement = await _requirementManager.GetRequirementByExternalIdAsync(principal, coveredRequirementTrait.Value);
            }

            if(requirement is not null)
            {
                // We found a matching requirement, add a link
                await _requirementManager.AddRequirementLinkAsync(principal, requirement, testCase.Id);
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
        if(string.IsNullOrEmpty(run.Commit))
        {
            foreach (var suite in run.Suites)
            {
                if (!string.IsNullOrEmpty(suite.Commit))
                {
                    run.Commit = suite.Commit;
                }
            }
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
                if (!string.IsNullOrEmpty(test.Commit))
                {
                    suite.Commit = test.Commit;
                }
            }
        }
    }

    private async Task UpdateTestRunFieldsAsync(ClaimsPrincipal principal, TestRun testRun, TestRunDto run, IReadOnlyList<FieldDefinition> testRunFieldDefinitions)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        foreach (var fieldDefinition in testRunFieldDefinitions)
        {
            var field = new TestRunField
            {
                TenantId = tenantId,
                TestRunId = testRun.Id,
                FieldDefinitionId = fieldDefinition.Id,
            };
            var values = run.Traits.Where(x => x.Type == fieldDefinition.TraitType).Select(x => x.Value).ToArray();
            if (FieldValueConverter.TryAssignValue(fieldDefinition, field, values))
            {
                await _fieldManager.UpsertTestRunFieldAsync(principal, field);
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
                Open = false,
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

        // Metric
        if(trait.Name.StartsWith("metric:"))
        {
            return;
        }

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
            var values = traits.Where(x=>x.Type == fieldDefinition.TraitType).Select(x => x.Value).ToArray();
            if (FieldValueConverter.TryAssignValue(fieldDefinition, field, values))
            {
                await _fieldManager.UpsertTestCaseFieldAsync(principal, field);
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
        IReadOnlyList<FieldDefinition> testCaseRunFieldDefinitions,
        ImportHandlingOptions options)
    {
        if (testRun.TestProjectId is null)
        {
            throw new InvalidOperationException("Test runs must have a project");
        }
        var tenantId = principal.GetTenantIdOrThrow();

        var testName = testCase.Name ?? "-";
        var completedState = await _stateService.GetTestCaseRunFinalStateAsync(principal, testRun.TestProjectId.Value);
        var testCaseRun = new TestCaseRun()
        {
            Name = testName,
            TenantId = tenantId,
            TestRunId = testRun.Id,
            TestProjectId = testRun.TestProjectId.Value,
            Result = test.Result ?? TestResult.NoRun,
            TestCaseId = testCase.Id,
            Duration = (int)(test.Duration?.TotalMilliseconds ?? 0),
            CallStack = test.CallStack,
            SystemOut = test.SystemOut,
            SystemErr = test.SystemErr,
            Message = test.Message,
            State = completedState.Name,
            AssignedToUserName = options.AssignTestsToUserName
        };

        await _testRunManager.AddTestCaseRunAsync(principal, testCaseRun);
        await AddAttachmentsAsync(testCaseRun, test.Attachments);

        return testCaseRun;
    }


    private async Task AddAttachmentsAsync(TestCaseRun testCaseRun, List<AttachmentDto>? attachments)
    {
        Debug.Assert(testCaseRun.TenantId != null);

        if (attachments is not null)
        {
            foreach (var attachment in attachments)
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
                new FilterByProject<TestCase>(projectId),
                new FilterTestCasesByExternalId(test.ExternalId)
            ];
            var testCase = (await _testCaseRepository.SearchTestCasesAsync(0, 1, filters, x => x.Created, false)).Items.FirstOrDefault();
            if (testCase is not null)
            {
                bool changed = false;

                // Update the test case
                if (testCase.Description != test.Description)
                {
                    testCase.Description = test.Description;
                    changed = true;
                }
                if (changed)
                {
                    await _testCaseManager.SaveTestCaseAsync(principal, testCase);
                }

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