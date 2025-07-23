using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;

using Azure;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
using Microsoft.SemanticKernel.Agents.Orchestration.Sequential;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.ChatCompletion;

using TestBucket.Domain.AI.Agent.Agents;
using TestBucket.Domain.AI.Agent.Logging;
using TestBucket.Domain.AI.Agent.Models;
using TestBucket.Domain.AI.Agent.Orchestration;
using TestBucket.Domain.AI.Mcp.Services;
using TestBucket.Domain.AI.Tools;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Testing.Compiler;
using TestBucket.Domain.Testing.TestCases;

namespace TestBucket.Domain.AI.Agent;

// AsKernelFunction
#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0110

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
                if (reference.EntityTypeName == "TestCase")
                {
                    // Compile it
                    List<CompilerError> errors = [];
                    var testCase = await _testCaseManager.GetTestCaseByIdAsync(principal, reference.Id);
                    if (testCase is not null)
                    {
                        var options = new CompilationOptions(testCase, testCase.Description ?? "");
                        var testExecutionContext = await _testExecutionContextBuilder.BuildAsync(principal, options, errors, cancellationToken);
                        if (testExecutionContext?.CompiledText is not null)
                        {
                            text = testExecutionContext.CompiledText;
                        }
                    }
                }

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
        if(orchestrationStrategy is null || orchestrationStrategy == OrchestrationStrategies.Default)
        {
            await foreach(var update in AskAsync(principal, project, context,userMessage, cancellationToken))
            {
                yield return update;
            }
            yield break;
        }

        ChatHistory newMessages = []; // This only contain the new messages
        ChatHistory groupChatHistory = [];

        // Enrich context with references
        await SetupReferencesInContextAsync(principal, context, userMessage, groupChatHistory, cancellationToken);

        // Create kernel
        var kernel = await _semanticKernelFactory.CreateKernelAsync(principal);

        // Add tools as plugins to semantic kernel
        await PrepareToolsAsync(principal, context, kernel, cancellationToken);

        OrchestrationResponseCallback responseCallback = async (response) =>
        {
            _logger.LogInformation("Received response from agent {AuthorName}", response.AuthorName);
            groupChatHistory.Add(response);

            // Save response into history, we use this to build the context
            newMessages.Add(response);

            // Log the message for usage/billing information
            await _agentLogManager.LogResponseAsync(project?.TeamId, project?.TeamId, orchestrationStrategy, principal, response);
        };

        // Channel that sends updates from the agents to us
        var channel = Channel.CreateUnbounded<ChatResponseUpdate>();
        OrchestrationStreamingCallback streamingCallback = async (StreamingChatMessageContent response, bool isFinal) =>
        {
            try
            {
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
        await runtime.StartAsync();

        try
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var task = Task.Run(async () =>
            {
                var result = await orchestration.InvokeAsync(userMessage.Text, runtime, cancellationToken: cts.Token);
                try
                {
                    string output = await result.GetValueAsync(TimeSpan.FromSeconds(300), cts.Token);
                    await runtime.RunUntilIdleAsync();
                }
                catch (Exception ex)
                {
                    await channel.Writer.WriteAsync(new ChatResponseUpdate(ChatRole.Assistant, $"An error occured: {ex.Message}"));
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
        ToolCollection tools = context.Tools ?? await GetToolsAsync(principal, context, cancellationToken);
        context.Tools = tools;
        foreach (var toolName in tools.ToolNames)
        {
            var kernelTools = tools.GetEnabledFunctionsByToolName(toolName).Select(x => x.AsKernelFunction());
            var toolNameForSemanticKernel = SemanticKernelToolNaming.GetSemanticKernelPluginName(toolName);
            kernel.Plugins.AddFromFunctions(toolNameForSemanticKernel, kernelTools);
        }
    }

    private async Task SetupReferencesInContextAsync(ClaimsPrincipal principal, AgentChatContext context, ChatMessage userMessage, ChatHistory groupChatHistory, CancellationToken cancellationToken)
    {
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
        var references = await GetReferencesAsChatMessagesAsync(principal, context, cancellationToken);

        using var client = await _chatClientFactory.CreateChatClientAsync(principal, AI.Models.ModelType.Default);
        if (client is not null)
        {
            List<ChatMessage> chatMessagesContext = [.. references, .. context.Messages];

            // Finally, add to the user message and call the model
            chatMessagesContext = [.. references, .. context.Messages];
            await foreach (var update in CallModelAsync(principal, context, userMessage, chatMessagesContext, client, cancellationToken))
            {
                await _agentLogManager.LogResponseAsync(project?.TeamId, project?.Id, principal, update);

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
