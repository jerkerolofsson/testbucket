using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.ChatCompletion;

using TestBucket.Domain.AI.Agent.Logging;
using TestBucket.Domain.AI.Agent.Orchestration;
using TestBucket.Domain.AI.Billing;
using TestBucket.Domain.AI.Diagnostics.Filters;
using TestBucket.Domain.AI.Mcp.Services;
using TestBucket.Domain.AI.Tools;
using TestBucket.Domain.Diagnostics;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Testing.Compiler;
using TestBucket.Domain.Testing.TestCases;

namespace TestBucket.Domain.AI.Agent;

public class AgentChatClient
{
    private readonly IChatClientFactory _chatClientFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly McpServerRunnerManager _mcpServerRunnerManager;
    private readonly ITestCaseManager _testCaseManager;
    private readonly TestExecutionContextBuilder _testExecutionContextBuilder;
    private readonly SemanticKernelFactory _semanticKernelFactory;
    private readonly ILogger<AgentChatClient> _logger;
    private readonly IAgentLogManager _agentLogManager;

    private ActivitySource ActivitySource => TestBucketActivitySource.ActivitySource;

    public AgentChatClient(IChatClientFactory chatClientFactory, IServiceProvider serviceProvider)
    {
        _chatClientFactory = chatClientFactory;
        _serviceProvider = serviceProvider;
        _mcpServerRunnerManager = serviceProvider.GetRequiredService<McpServerRunnerManager>();
        _testCaseManager = serviceProvider.GetRequiredService<ITestCaseManager>();
        _testExecutionContextBuilder = serviceProvider.GetRequiredService<TestExecutionContextBuilder>();
        _semanticKernelFactory = serviceProvider.GetRequiredService<SemanticKernelFactory>();
        _logger = serviceProvider.GetRequiredService<ILogger<AgentChatClient>>();
        _agentLogManager = serviceProvider.GetRequiredService<IAgentLogManager>();
    }

    public async Task<ToolCollection> GetToolsAsync(ClaimsPrincipal principal, AgentChatContext context, CancellationToken cancellationToken = default)
    {
        var toolCollection = new ToolCollection(_serviceProvider);

        if (context.ProjectId is not null)
        {
            // Add the project claim
            principal = Impersonation.ChangeProject(principal, context.ProjectId.Value);
        }

        // Index all MCP tools in assembly
        toolCollection.AddMcpServerToolsFromAssembly(principal, GetType().Assembly);

        // Add MCP tools from the MCP server manager
        if (context.ProjectId is not null)
        {
            var mcpTools = await _mcpServerRunnerManager.GetMcpToolsForUserAsync(principal, context, cancellationToken);
            foreach (var mcpTool in mcpTools)
            {
                toolCollection.Add(mcpTool.ToolName??"UnknownTool", mcpTool.AIFunction, mcpTool.Enabled);
            }
        }

        return toolCollection;
    }

    internal async ValueTask<IReadOnlyList<ChatMessage>> GetReferencesAsChatMessagesAsync(
        ClaimsPrincipal principal, 
        AgentChatContext context,
        CancellationToken cancellationToken)
    {
        List<ChatMessage> messages = [];

        if (context.References.Count > 0)
        {
            foreach (var reference in context.References)
            {
                var text = reference.Text ?? "";
                if (reference.EntityTypeName == "TestCase" && long.TryParse(reference.Id, out var dbid))
                {
                    // Compile it
                    List<CompilerError> errors = [];
                    var testCase = await _testCaseManager.GetTestCaseByIdAsync(principal, dbid);
                    if (testCase is not null)
                    {
                        var options = new CompilationOptions(testCase, testCase.Description ?? "");
                        var testExecutionContext = await _testExecutionContextBuilder.BuildAsync(principal, options, errors, cancellationToken);
                        if (testExecutionContext?.CompiledText is not null)
                        {
                            text = testExecutionContext.CompiledText;
                        }
                    }

                    IList<AIContent> content = [];
                    var prompt = $"""
                        <testcase>
                            <name>{reference.Name}</name>
                            <id>{reference.Id}</id>
                            <description>{text}</description>
                        </testcase>
                        """;
                    content.Add(new Microsoft.Extensions.AI.TextContent(prompt));
                    messages.Add(new ChatMessage(ChatRole.User, content));
                }
                else
                {

                    IList<AIContent> content = [];
                    var prompt = $"""
                        # {reference.Name}
                        - ID: {reference.Id}
                        - Type: {reference.EntityTypeName}

                        {text}
                        """;
                    content.Add(new Microsoft.Extensions.AI.TextContent(prompt));
                    messages.Add(new ChatMessage(ChatRole.System, content));
                }
            }
        }
        return messages;
    }

    public async IAsyncEnumerable<ChatResponseUpdate> InvokeAgentAsync(
        string? orchestrationStrategy,
        ClaimsPrincipal principal, 
        TestProject? project,
        AgentChatContext context,
        ChatMessage userMessage,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Track usage for this session
        context.InvokationUsage = new ChatUsage() { InputTokenCount = 0, OutputTokenCount = 0, TotalTokenCount = 0, UsageCategory = "" };

        if (orchestrationStrategy is null || orchestrationStrategy is OrchestrationStrategies.Default)
        {
            await foreach(var update in AskAsync(principal, project, context, userMessage, cancellationToken))
            {
                yield return update;
            }
            yield break;
        }

        using var activity =  ActivitySource.StartActivity("InvokeAgentAsync");

        ChatHistory newMessages = []; // This only contain the new messages
        ChatHistory groupChatHistory = [];

        // Enrich context with references
        await SetupReferencesInContextAsync(principal, context, userMessage, groupChatHistory, cancellationToken);

        // Create kernel
        var kernel = await _semanticKernelFactory.CreateKernelAsync(principal);
        kernel.FunctionInvocationFilters.Add(new FunctionTracingFilter(ActivitySource));
        kernel.PromptRenderFilters.Add(new PromptTracingFilter(ActivitySource));

        // Add tools as plugins to semantic kernel
        await PrepareToolsAsync(principal, context, kernel, cancellationToken);

        OrchestrationResponseCallback responseCallback = async (response) =>
        {
            activity?.AddEvent(new ActivityEvent("OrchestrationResponseCallback"));

            _logger.LogInformation("Received response from agent {AuthorName}", response.AuthorName);
            groupChatHistory.Add(response);

            // Save response into history, we use this to build the context
            newMessages.Add(response);

            // Log the message for usage/billing information
            var usage = await _agentLogManager.LogResponseAsync(project?.TeamId, project?.TeamId, orchestrationStrategy, principal, response);
            if(usage is not null)
            {
                context.InvokationUsage.Add(usage);
            }
        };

        // Channel that sends updates from the agents to us
        var channel = Channel.CreateUnbounded<ChatResponseUpdate>();
        OrchestrationStreamingCallback streamingCallback = async (StreamingChatMessageContent response, bool isFinal) =>
        {
            try
            {
                activity?.AddEvent(new ActivityEvent("OrchestrationStreamingCallback"));

                await channel.Writer.WriteAsync(response.ToChatResponseUpdate());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting response to ChatResponseUpdate");
            }
        };

        orchestrationStrategy ??= OrchestrationStrategies.Default;
        var orchestration = new OrchestrationBuilder().Build(orchestrationStrategy, kernel, groupChatHistory, _serviceProvider, responseCallback, streamingCallback);

        // We start running..
        await using InProcessRuntime runtime = new InProcessRuntime();
        await runtime.StartAsync(cancellationToken);

        try
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var task = Task.Run(async () =>
            {
                var result = await orchestration.InvokeAsync(userMessage.Text, runtime, cancellationToken: cts.Token);
                try
                {
                    activity?.AddEvent(new ActivityEvent("InvokeAsync"));

                    string output = await result.GetValueAsync(TimeSpan.FromSeconds(300), cts.Token);
                    await runtime.RunUntilIdleAsync();
                }
                catch (Exception ex)
                {
                    var errorMessage = $"An error occured: {ex.Message}";

                    await channel.Writer.WriteAsync(new ChatResponseUpdate(ChatRole.Assistant, errorMessage));
                }
                finally
                {
                    cts.Cancel();
                }
            });

            // Read from channel and yield restult in order to update the UI
            while (!cts.IsCancellationRequested)
            {
                ChatResponseUpdate? update = null;
                try
                {
                    update = await channel.Reader.ReadAsync(cts.Token);
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading chat updates from channel");
                }

                if (update is not null)
                {
                    yield return update;
                }
            }
        }
        finally
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                foreach (var message in newMessages)
                {
                    // We add user messages first so the UI is updated directly after the user asks
                    // the question
                    if (message.Role != AuthorRole.User)
                    {
                        context.Messages.Add(message.ToChatMessage());
                    }
                }
            }
        }
    }

    private async Task PrepareToolsAsync(ClaimsPrincipal principal, AgentChatContext context, Kernel kernel, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("PrepareToolsAsync");
        activity?.Start();

        ToolCollection tools = context.Tools = await GetToolsAsync(principal, context, cancellationToken);
        foreach (var toolName in tools.ToolNames)
        {
            var kernelTools = tools.GetEnabledFunctionsByToolName(toolName).Select(x => x.AsKernelFunction());
            var toolNameForSemanticKernel = SemanticKernelToolNaming.GetSemanticKernelPluginName(toolName);
            kernel.Plugins.AddFromFunctions(toolNameForSemanticKernel, kernelTools);

            activity?.AddTag("tools.count", kernelTools.Count().ToString());
        }
    }

    private async Task SetupReferencesInContextAsync(ClaimsPrincipal principal, AgentChatContext context, ChatMessage userMessage, ChatHistory groupChatHistory, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("SetupReferencesInContextAsync");
        activity?.Start();

        if (context.Messages.Count == 0)
        {
            var references = await GetReferencesAsChatMessagesAsync(principal, context, cancellationToken);
            foreach (var reference in references)
            {
                groupChatHistory.Add(reference.ToSemanticKernelChatMessage());
            }
        }
        else
        {
            foreach (var historyMessage in context.Messages)
            {
                groupChatHistory.Add(historyMessage.ToSemanticKernelChatMessage());
            }
        }
        context.Messages.Add(userMessage);
    }

    public async IAsyncEnumerable<ChatResponseUpdate> AskAsync(ClaimsPrincipal principal,
        TestProject? project,
        AgentChatContext context, 
        ChatMessage userMessage, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Track usage for this session
        context.InvokationUsage = new ChatUsage() { InputTokenCount = 0, OutputTokenCount = 0, TotalTokenCount = 0, UsageCategory = "" };

        var references = await GetReferencesAsChatMessagesAsync(principal, context, cancellationToken);

        using var client = await _chatClientFactory.CreateChatClientAsync(principal, AI.Models.ModelType.Default);
        if (client is not null)
        {
            List<ChatMessage> chatMessagesContext = [.. references, .. context.Messages];

            // Finally, add to the user message and call the model
            chatMessagesContext = [.. references, .. context.Messages];
            await foreach (var update in CallModelAsync(principal, context, userMessage, chatMessagesContext, client, cancellationToken))
            {
                var usage = await _agentLogManager.LogResponseAsync(project?.TeamId, project?.Id, principal, update);
                if(usage is not null)
                {
                    context.InvokationUsage.Add(usage);
                }

                cancellationToken.ThrowIfCancellationRequested();
                yield return update;
            }
        }
        else
        {
            string errorMessage = "AI provider configuration is incorrect or missing\n";
            context.Messages.Add(new ChatMessage(ChatRole.System, errorMessage));
            yield return new ChatResponseUpdate(ChatRole.System, errorMessage);
        }
    }

    internal async IAsyncEnumerable<ChatResponseUpdate> CallModelAsync(ClaimsPrincipal principal, AgentChatContext context, ChatMessage userMessage, List<ChatMessage> chatMessages, IChatClient client,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        chatMessages.Add(userMessage);
        context.Messages.Add(userMessage);

        // Trigger a UI update
        yield return new ChatResponseUpdate();

        ToolCollection tools = context.Tools ?? await GetToolsAsync(principal, context, cancellationToken);
        context.Tools = tools;

        var options = new ChatOptions
        {
            ToolMode = ChatToolMode.Auto,
            Tools = [.. tools.EnabledFunctions]
        };

        List<ChatResponseUpdate> updates = [];
        try
        {
            //List<ChatMessage> withoutToolCalls = [.. chatMessages.Where(x => !x.Contents.Any(c => c is FunctionCallContent))];

            await foreach(var message in client.GetStreamingResponseAsync(chatMessages, options, cancellationToken))
            {
                updates.Add(message);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                yield return message;
            }
        }
        finally
        {
            var response = ChatResponseExtensions.ToChatResponse(updates);
            context.Messages.AddRange(response.Messages);
        }
    }
}
