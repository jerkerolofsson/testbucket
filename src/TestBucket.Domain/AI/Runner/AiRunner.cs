using System.Diagnostics;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Testing.States;
using TestBucket.Contracts.TestResources;
using TestBucket.Domain.AI.Agent;
using TestBucket.Domain.AI.Agent.Orchestration;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Metrics;
using TestBucket.Domain.Metrics.Models;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.States;
using TestBucket.Domain.Tenants;
using TestBucket.Domain.Testing.Compiler;
using TestBucket.Domain.Testing.Execution;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCaseRuns;
using TestBucket.Domain.TestResources.Mapping;

namespace TestBucket.Domain.AI.Runner;
internal class AiRunner : BackgroundService
{
    private readonly AiRunnerJobQueue _queue;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AiRunner> _logger;

    public AiRunner(AiRunnerJobQueue queue, IServiceScopeFactory serviceScopeFactory, ILogger<AiRunner> logger)
    {
        _queue = queue;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("AI runner is started, waiting for new tests..");

            var job = await _queue.DequeueAsync(stoppingToken);
            if (job == null)
            {
                continue;
            }
            using var scope = _serviceScopeFactory.CreateScope();

            // Process the test that was enqueued
            try
            {

                await ProcessTestCaseRunAsync(scope, job.TestCaseRun, stoppingToken);
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AI runner job");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Processing pending tests..");
                    var result = await ProcessPendingTestsAsync(scope, stoppingToken);
                    if(!result)
                    {
                        // No more tests, go back and wait for item in queue
                        break;
                    }
                }
                catch (OperationCanceledException)
                {

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing AI runner job");
                }
            }
        }
    }

    private async Task<bool> ProcessPendingTestsAsync(IServiceScope scope, CancellationToken stoppingToken)
    {
        bool foundAiRunnerTest = false;

        var tenantRepository = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
        var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
        await foreach(var tenant in tenantRepository.EnumerateAsync(stoppingToken))
        {
            await foreach(var project in projectRepository.EnumerateAsync(tenant.Id, stoppingToken))
            {
                if(await ProcessPendingProjectTestsAsync(scope, tenant.Id, project.Id, stoppingToken))
                {
                    foundAiRunnerTest = true;
                }
            }
        }
        return foundAiRunnerTest;
    }

    private async Task ProcessTestCaseRunAsync(IServiceScope scope, TestCaseRun testCaseRun, CancellationToken cancellationToken)
    {
        if (testCaseRun.AssignedToUserName != AiRunnerConstants.Username)
        {
            return;
        }

        if (testCaseRun.MappedState == Contracts.Testing.States.MappedTestState.Completed)
        {
            return;
        }

        if (testCaseRun.TenantId is null)
        {
            var message = $"{AiRunnerConstants.Username}: Failed to run as TestCaseRun.TenantId is null";
            await OnTestCaseRunFailedAsync(scope, testCaseRun, message);
            return;
        }

        _logger.LogInformation("Processing AI test case run {TestCaseRunId}..", testCaseRun.Id);

        var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
        if (testCaseRun.TestProjectId is null)
        {
            string message = $"{AiRunnerConstants.Username}: Failed to start as test case run does not belong to a project";
            await OnTestCaseRunFailedAsync(scope, testCaseRun, message);
            return;
        }

        var project = await projectRepository.GetProjectByIdAsync(testCaseRun.TenantId, testCaseRun.TestProjectId.Value);
        if (project is null)
        {
            string message = $"{AiRunnerConstants.Username}: Project {testCaseRun.TestProjectId} was not found";
            await OnTestCaseRunFailedAsync(scope, testCaseRun, message);
            return;
        }

        var testCaseRepository = scope.ServiceProvider.GetRequiredService<ITestCaseRepository>();
        var testCase = await testCaseRepository.GetTestCaseByIdAsync(testCaseRun.TenantId, testCaseRun.TestCaseId);
        if (testCase is null)
        {
            string message = $"{AiRunnerConstants.Username}: Test case {testCaseRun.TestCaseId} was not found";
            await OnTestCaseRunFailedAsync(scope, testCaseRun, message);
            return;
        }
        if (string.IsNullOrEmpty(testCase.Description))
        {
            string message = $"{AiRunnerConstants.Username}: Test case {testCase.Name} has no description";
            await OnTestCaseRunFailedAsync(scope, testCaseRun, message);
            return;
        }

        var testRun = await testCaseRepository.GetTestRunByIdAsync(testCaseRun.TenantId, testCaseRun.TestRunId);
        if (testRun is null)
        {
            string message = $"{AiRunnerConstants.Username}: Test case run {testCaseRun.TestRunId} was not found";
            await OnTestCaseRunFailedAsync(scope, testCaseRun, message);
            return;
        }

        await CompileAndRunAsync(scope, project, testCaseRun, testCase, testRun, cancellationToken);
    }

    private async Task<bool> CompileAndRunAsync(IServiceScope scope, TestProject project, TestCaseRun testCaseRun, TestCase testCase, TestRun testRun, CancellationToken cancellationToken)
    {
        var principal = Impersonation.Impersonate(testCaseRun.TenantId, testCaseRun.TestProjectId);

        var builder = scope.ServiceProvider.GetRequiredService<TestExecutionContextBuilder>();
        List<CompilerError> errors = [];
        var options = new CompilationOptions(testCase, testCase.Description ?? "")
        {
            AllocateResources = true,
            ReleaseResourceDirectly = false,
            TestRunId = testRun.Id
        };


        var testExecutionContext = await builder.BuildAsync(principal, options, errors, cancellationToken);
        if (testExecutionContext is null)
        {
            string message = $"{AiRunnerConstants.Username}: Test case run could not be started. Building the test execution context failed. This indicates a data-level error with broken relationships between entities.";
            await OnTestCaseRunFailedAsync(scope, testCaseRun, message);
            return false;
        }

        if (errors.Count > 0)
        {
            string errorList = string.Join("- ", errors.Select(x => x.Message));
            string message = $"{AiRunnerConstants.Username}: Test case run could not be started as there were compilation errors:\n{errorList}";
            await OnTestCaseRunFailedAsync(scope, testCaseRun, message);
            return false;
        }
        try
        {
            await OnStartingAsync(scope, testCaseRun);

            var startTimestamp = Stopwatch.GetTimestamp();

            StringBuilder responseBuilder = await RunWithAgentAsync(scope, testCaseRun, project, testCase, principal, testExecutionContext, cancellationToken);
            var responseMarkdown = responseBuilder.ToString();

            var interpreter = scope.ServiceProvider.GetRequiredService<AiResultInterpreter>();
            var result = await interpreter.InterpretAiRunnerAsync(principal, project.Id, responseMarkdown);

            // Log the test duration
            testCaseRun.Duration = (int)Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;

            await OnTestCaseRunResultAsync(principal, scope, testCaseRun, responseMarkdown, result, testExecutionContext);
        }
        catch (Exception ex)
        {
            testCaseRun.CallStack = ex.ToString();
            await OnTestCaseRunFailedAsync(scope, testCaseRun, $"An error occured when running the test: {ex.Message}");
            return false;
        }
        finally
        {
            // Release resources and accounts that was locked when building the context
            if (testExecutionContext is not null)
            {
                await builder.ReleaseResourcesAsync(testExecutionContext);
            }
        }
        return true;
    }

    private async Task<StringBuilder> RunWithAgentAsync(IServiceScope scope, TestCaseRun testCaseRun, TestProject project, TestCase testCase, ClaimsPrincipal principal, TestExecutionContext testExecutionContext, CancellationToken cancellationToken)
    {
        var chatClientFactory = scope.ServiceProvider.GetRequiredService<IChatClientFactory>();

        AgentChatContext agentContext = CreateContext(scope, project, testCase, testExecutionContext);

        var chatMessage = new ChatMessage(ChatRole.User, SuggestionProvider.GetAiRunTestPrompt(testCase.Name));
        var client = new AgentChatClient(chatClientFactory, scope.ServiceProvider);
        var responseBuilder = new StringBuilder();
        int numToolCalls = 0;
        await foreach (var update in client.InvokeAgentAsync(OrchestrationStrategies.AiRunner, principal, project, agentContext, chatMessage, cancellationToken))
        {
            foreach (var content in update.Contents)
            {
                if (content is FunctionCallContent)
                {
                    numToolCalls++;
                }
            }

            AiRunnerTestLogBuilder.Append(responseBuilder, update);
        }

        await AddMetricsAsync(scope, testCaseRun, principal, agentContext, numToolCalls);

        return responseBuilder;
    }

    private static async Task AddMetricsAsync(IServiceScope scope, TestCaseRun testCaseRun, ClaimsPrincipal principal, AgentChatContext agentContext, int numToolCalls)
    {

        var metricsManager = scope.ServiceProvider.GetRequiredService<IMetricsManager>();
        if (agentContext.InvokationUsage is not null)
        {
            var totalTokensMetric = new Metric { Unit = "tokens", MeterName = "ai_runner", Name = "total_tokens", Value = agentContext.InvokationUsage.TotalTokenCount, TestCaseRunId = testCaseRun.Id, TestProjectId = testCaseRun.TestProjectId, TeamId = testCaseRun.TeamId };
            await metricsManager.AddMetricAsync(principal, totalTokensMetric);
        }

        var numToolCallsMetric = new Metric { Unit = "calls", MeterName = "ai_runner", Name = "tool_calls", Value = numToolCalls, TestCaseRunId = testCaseRun.Id, TestProjectId = testCaseRun.TestProjectId, TeamId = testCaseRun.TeamId };
        await metricsManager.AddMetricAsync(principal, numToolCallsMetric);
    }

    private static AgentChatContext CreateContext(IServiceScope scope, TestProject project, TestCase testCase, TestExecutionContext testExecutionContext)
    {
        var agentContext = scope.ServiceProvider.GetRequiredService<AgentChatContext>();
        agentContext.ProjectId = project.Id;
        testCase.Description = testExecutionContext!.CompiledText;
        agentContext.ClearReferences();
        if (testExecutionContext?.Resources?.Count > 0)
        {
            foreach (var resource in testExecutionContext.Resources)
            {
                agentContext.References.Add(ChatReferenceBuilder.Create(resource.ToDbo()));
            }
        }
        agentContext.References.Add(ChatReferenceBuilder.Create(testCase));
        return agentContext;
    }

    private static async Task OnStartingAsync(IServiceScope scope, TestCaseRun testCaseRun)
    {
        await SetStateAsync(scope, testCaseRun, MappedTestState.Ongoing);
        var testCaseRepository = scope.ServiceProvider.GetRequiredService<ITestCaseRepository>();
        await testCaseRepository.UpdateTestCaseRunAsync(testCaseRun);
    }

    private static async Task OnTestCaseRunResultAsync(ClaimsPrincipal principal, IServiceScope scope, TestCaseRun testCaseRun, string message, TestResult result, TestExecutionContext testExecutionContext)
    {
        var testCaseRepository = scope.ServiceProvider.GetRequiredService<ITestCaseRepository>();
        var fieldDefinitionManager = scope.ServiceProvider.GetRequiredService<IFieldDefinitionManager>();
        var fieldManager = scope.ServiceProvider.GetRequiredService<IFieldManager>();
        testCaseRun.Message = message;
        testCaseRun.Result = result;

        await CopyFieldsFromTestResourcesToTestCaseRunAsync(principal, fieldManager, fieldDefinitionManager, testCaseRun, testExecutionContext.Resources);

        // Set completed state
        await SetStateAsync(scope, testCaseRun, MappedTestState.Completed);
        await testCaseRepository.UpdateTestCaseRunAsync(testCaseRun);
    }

    /// <summary>
    /// Extracts fields from the resource and assigns the to the test case run
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="fieldDefinitionManager"></param>
    /// <param name="testCaseRun"></param>
    /// <param name="resources"></param>
    /// <returns></returns>
    private static async Task CopyFieldsFromTestResourcesToTestCaseRunAsync(
        ClaimsPrincipal principal,
        IFieldManager fieldManager,
        IFieldDefinitionManager fieldDefinitionManager, 
        TestCaseRun testCaseRun, 
        List<TestResourceDto> resources)
    {
        if (resources is null || resources.Count == 0)
        {
            return;
        }
        var fields = await fieldDefinitionManager.GetDefinitionsAsync(principal, testCaseRun.TestProjectId, FieldTarget.TestCaseRun);

        List<TestCaseRunField> mappedFields = TestResourceFieldMapper.MapResourcesToFields(fields, testCaseRun, resources);
        foreach(var field in mappedFields)
        {
            await fieldManager.UpsertTestCaseRunFieldAsync(principal, field);
        }
    }

    private static async Task OnTestCaseRunFailedAsync(IServiceScope scope, TestCaseRun testCaseRun, string message)
    {
        var testCaseRepository = scope.ServiceProvider.GetRequiredService<ITestCaseRepository>();
        testCaseRun.Message = message;
        testCaseRun.Result = TestResult.Failed;

        // Set completed state
        await SetStateAsync(scope, testCaseRun, MappedTestState.Completed);
        await testCaseRepository.UpdateTestCaseRunAsync(testCaseRun);
    }

    private static async Task SetStateAsync(IServiceScope scope, TestCaseRun testCaseRun, MappedTestState newState)
    {
        var stateService = scope.ServiceProvider.GetRequiredService<IStateService>();
        if (testCaseRun.TestProjectId is not null)
        {
            var principal = Impersonation.Impersonate(testCaseRun.TenantId, testCaseRun.TestProjectId);
            var states = await stateService.GetTestCaseRunStatesAsync(principal, testCaseRun.TestProjectId!.Value);
            var state = states.Where(x => x.MappedState == newState).FirstOrDefault();
            testCaseRun.State = state?.Name ?? newState.ToString();
            testCaseRun.MappedState = newState;
        }
        else
        {
            testCaseRun.State = newState.ToString();
            testCaseRun.MappedState = newState;
        }
    }

    private async Task<bool> ProcessPendingProjectTestsAsync(IServiceScope scope, string tenantId, long projectId, CancellationToken stoppingToken)
    {
        _logger.LogInformation("Processing AI tests for project {ProjectId} in tenant {TenantId}", projectId, tenantId);

        var testRepository = scope.ServiceProvider.GetRequiredService<ITestCaseRepository>();

        List<FilterSpecification<TestCaseRun>> filters = [
            new FilterByTenant<TestCaseRun>(tenantId), 
            new FilterByProject<TestCaseRun>(projectId),
            new FilterTestCaseRunsByAssignment(AiRunnerConstants.Username),
            new FilterTestCaseRunsByMappedState(Contracts.Testing.States.MappedTestState.Assigned)
        ];

        int offset = 0;
        int count = 1;
        var result = await testRepository.SearchTestCaseRunsAsync(filters, offset, count);
        foreach(var testCaseRun in result.Items)
        {
            await ProcessTestCaseRunAsync(scope, testCaseRun, stoppingToken);
        }

        return result.TotalCount > 0;
    }
}
