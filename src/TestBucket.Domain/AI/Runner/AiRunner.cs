using System.Text;
using System.Text.Json;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.AI.Agent;
using TestBucket.Domain.AI.Agent.Orchestration;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.States;
using TestBucket.Domain.Tenants;
using TestBucket.Domain.Testing.Compiler;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCaseRuns;

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

    private static async Task<bool> CompileAndRunAsync(IServiceScope scope, TestProject project, TestCaseRun testCaseRun, TestCase testCase, TestRun testRun, CancellationToken cancellationToken)
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

        if (errors.Count > 0)
        {
            string errorList = string.Join("- ", errors.Select(x => x.Message));
            string message = $"{AiRunnerConstants.Username}: Test case run could not be started as there were compilation errors:\n{errorList}";
            await OnTestCaseRunFailedAsync(scope, testCaseRun, message);
            return false;
        }

        var testExecutionContext = await builder.BuildAsync(principal, options, errors, cancellationToken);
        if (testExecutionContext is null)
        {
            string message = $"{AiRunnerConstants.Username}: Test case run could not be started. Building the test execution context failed. This indicates a data-level error with broken relationships between entities.";
            await OnTestCaseRunFailedAsync(scope, testCaseRun, message);
            return false;
        }
        try
        {
            await OnStartingAsync(scope, testCaseRun);

            StringBuilder responseBuilder = await RunWithAgentAsync(scope, project, testCase, principal, testExecutionContext, cancellationToken);
            var responseMarkdown = responseBuilder.ToString();

            var interpreter = scope.ServiceProvider.GetRequiredService<AiResultInterpreter>();
            var result = await interpreter.InterpretAiRunnerAsync(principal, project.Id, responseMarkdown);

            await OnTestCaseRunResultAsync(scope, testCaseRun, responseMarkdown, result);
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

    private static async Task<StringBuilder> RunWithAgentAsync(IServiceScope scope, TestProject project, TestCase testCase, ClaimsPrincipal principal, TestExecutionContext? testExecutionContext, CancellationToken cancellationToken)
    {
        var chatClientFactory = scope.ServiceProvider.GetRequiredService<IChatClientFactory>();
      
        var agentContext = scope.ServiceProvider.GetRequiredService<AgentChatContext>();
        agentContext.ProjectId = project.Id;
        testCase.Description = testExecutionContext!.CompiledText;
        agentContext.References.Add(ChatReferenceBuilder.Create(testCase));

        var chatMessage = new ChatMessage(ChatRole.User, SuggestionProvider.GetAiRunTestPrompt(testCase.Name));
        var client = new AgentChatClient(chatClientFactory, scope.ServiceProvider);
        var responseBuilder = new StringBuilder();
        await foreach (var update in client.InvokeAgentAsync(OrchestrationStrategies.AiRunner, principal, project, agentContext, chatMessage, cancellationToken))
        {
            foreach (var content in update.Contents)
            {
                if (content is FunctionCallContent functionCallContent)
                {
                    responseBuilder.AppendLine();
                    responseBuilder.AppendLine($"#### Function Call: {functionCallContent.Name}");
                    if (functionCallContent.Arguments is not null)
                    {
                        foreach (var arg in functionCallContent.Arguments)
                        {
                            responseBuilder.AppendLine($"- {arg.Key}: {arg.Value}");
                        }
                    }
                    if (functionCallContent.RawRepresentation is not null)
                    {
                        responseBuilder.AppendLine($"""
                            ```json
                            {JsonSerializer.Serialize(functionCallContent.RawRepresentation)}
                            ```
                            """);
                        responseBuilder.AppendLine();
                    }
                }
                else if(content is FunctionResultContent functionResult)
                {
                    if (functionResult.RawRepresentation is not null)
                    {
                        responseBuilder.AppendLine($"""
                            ```json
                            {JsonSerializer.Serialize(functionResult.RawRepresentation)}
                            ```
                            """);
                    }
                }
                else if (content is TextContent textContent)
                {
                    responseBuilder.Append(textContent.Text);
                }
                else
                {
                    responseBuilder.Append(content.ToString());
                }
            }
        }

        return responseBuilder;
    }

    private static async Task OnStartingAsync(IServiceScope scope, TestCaseRun testCaseRun)
    {
        await SetStateAsync(scope, testCaseRun, MappedTestState.Ongoing);
        var testCaseRepository = scope.ServiceProvider.GetRequiredService<ITestCaseRepository>();
        await testCaseRepository.UpdateTestCaseRunAsync(testCaseRun);
    }

    private static async Task OnTestCaseRunResultAsync(IServiceScope scope, TestCaseRun testCaseRun, string message, TestResult result)
    {
        var testCaseRepository = scope.ServiceProvider.GetRequiredService<ITestCaseRepository>();
        testCaseRun.Message = message;
        testCaseRun.Result = result;

        // Set completed state
        await SetStateAsync(scope, testCaseRun, MappedTestState.Completed);
        await testCaseRepository.UpdateTestCaseRunAsync(testCaseRun);
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
