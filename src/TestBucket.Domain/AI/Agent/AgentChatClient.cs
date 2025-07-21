using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;

using Azure;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Magentic;
using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

using TestBucket.Domain.AI.Agent.Agents;
using TestBucket.Domain.AI.Agent.Models;
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

    public AgentChatClient(IChatClientFactory chatClientFactory, IServiceProvider serviceProvider)
    {
        _chatClientFactory = chatClientFactory;
        _serviceProvider = serviceProvider;
        _mcpServerRunnerManager = serviceProvider.GetRequiredService<McpServerRunnerManager>();
        _testCaseManager = serviceProvider.GetRequiredService<ITestCaseManager>();
        _testExecutionContextBuilder = serviceProvider.GetRequiredService<TestExecutionContextBuilder>();
        _semanticKernelFactory = serviceProvider.GetRequiredService<SemanticKernelFactory>();
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

    /// <summary>
    /// Pre-processing is used to inject extra data into the context based on the user prompt
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="context"></param>
    /// <param name="userMessage"></param>
    /// <param name="contextMessages"></param>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<ChatResponseUpdate> PreProcessAsync(ClaimsPrincipal principal,
        AgentChatContext context,
        ChatMessage userMessage,
        List<ChatMessage> contextMessages, 
        IChatClient client,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        List<PipelineAction> actions = [];
        if(userMessage.Text.Contains("milestone", StringComparison.InvariantCulture))
        {
            actions.Add(new PipelineAction { Prompt = "Use the 'list-milestones' tool to collect information about milestones." });
        }

        foreach(var action in actions)
        {
            if(cancellationToken.IsCancellationRequested)
            {
                break;
            }

            // The pipeline action update is just used to update the UI. It has no relevance to the 
            var notifyUpdate = new PipelineActionUpdate() { ActionName = "Collecting information about milestones.." };
            yield return notifyUpdate;

            var message = new HiddenChatMessage(ChatRole.User, action.Prompt);
            await foreach (var update in CallModelAsync(principal, context, message, contextMessages, client, cancellationToken))
            {
                yield return update;
            }

        }
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
                //var xml = $"""
                //    <reference>
                //        <name>{reference.Name}</name>
                //        <id>{reference.Id}</url>
                //        <type>{reference.EntityTypeName ?? ""}</type>
                //        <description>{text}</description>
                //    </reference>
                //    """;
                //content.Add(new Microsoft.Extensions.AI.TextContent(xml));
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

    public async IAsyncEnumerable<ChatResponseUpdate> InvokeAsync(
        string? agentId,
        ClaimsPrincipal principal, 
        TestProject? project,
        AgentChatContext context,
        ChatMessage userMessage,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ChatHistory history = [];

        // Enrich context with references
        if (context.Messages.Count == 0)
        {
            var references = await GetReferencesAsChatMessagesAsync(principal, context, cancellationToken);
            //context.Messages.AddRange(references);
            foreach (var reference in references)
            {
                history.Add(reference.ToSemanticKernelChatMessage());
            }
        }

        // Create kernel
        var kernel = await _semanticKernelFactory.CreateKernelAsync(principal);

        // Add tools as plugins to semantic kernel
        ToolCollection tools = context.Tools ?? await GetToolsAsync(principal, context, cancellationToken);
        context.Tools = tools;
        foreach (var toolName in tools.ToolNames)
        {
            var kernelTools = tools.GetEnabledFunctionsByToolName(toolName).Select(x => x.AsKernelFunction());
            var toolNameForSemanticKernel = SemanticKernelToolNaming.GetSemanticKernelPluginName(toolName);
            kernel.Plugins.AddFromFunctions(toolNameForSemanticKernel, kernelTools);
        }

        agentId ??= "default";
        List<ChatCompletionAgent> members = [];
        switch(agentId)
        {
            case "test-designer":
                members.Add(TestCaseDesignerAgent.Create(kernel));
                members.Add(TestReviewerAgent.Create(kernel));
                members.Add(TestCaseDesignerTerminatorAgent.Create(kernel));
                break;

            case "test-creator":
                members.Add(TestCaseDesignerAgent.Create(kernel));
                members.Add(TestReviewerAgent.Create(kernel));
                members.Add(TestCreatorAgent.Create(kernel));
                break;

            default:
                members.Add(DefaultAgent.Create(kernel));
                break;
        }

        var channel = Channel.CreateUnbounded<StreamingChatMessageContent>();
        var chatManager = new RoundRobinGroupChatManager
        {
            MaximumInvocationCount = 5,
        };

        var orchestration = new GroupChatOrchestrationWithChatHistory(history, chatManager, members.ToArray())
        {
            ResponseCallback = (response) =>
            {
                history.Add(response);
                return ValueTask.CompletedTask;
            },
            StreamingResponseCallback = async (StreamingChatMessageContent response, bool isFinal) =>
            {
                await channel.Writer.WriteAsync(response);
            }
        };

        InProcessRuntime runtime = new InProcessRuntime();
        await runtime.StartAsync();

        //context.Messages.Add(userMessage);

        try
        {
            //await foreach (var message in agent.InvokeStreamingAsync(chat, thread:null, options, cancellationToken: cancellationToken))
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var task = Task.Run(async () =>
            {
                var result = await orchestration.InvokeAsync(userMessage.Text, runtime, cancellationToken: cts.Token);
                try
                {
                    string output = await result.GetValueAsync(TimeSpan.FromSeconds(300));
                    await runtime.RunUntilIdleAsync();
                }
                catch (Exception) { }
                finally
                {
                    cts.Cancel();
                }
            });
            

            while(!cts.IsCancellationRequested)
            {
                ChatResponseUpdate? update = null;
                try
                {
                    var response = await channel.Reader.ReadAsync(cts.Token);
                    update = response.ToChatResponseUpdate();

                    //var chatMessage = response.ToChatMessage();
                    //update = new ChatResponseUpdate(chatMessage.Role, chatMessage.Contents);
                }
                catch (OperationCanceledException) { }

                if(update is not null)
                {
                    yield return update;
                }
            }
        }
        finally
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                foreach (var message in history)
                {
                    context.Messages.Add(message.ToChatMessage());
                }
            }
            //var response = ChatResponseExtensions.ToChatResponse(updates);
            //context.Messages.AddRange(response.Messages);
        }
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(ClaimsPrincipal principal, 
        TestProject? project,
        AgentChatContext context, 
        ChatMessage userMessage, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var references = await GetReferencesAsChatMessagesAsync(principal, context, cancellationToken);
        if(references.Count > 0)
        {
            yield return new ChatResponseUpdate(ChatRole.System, $"Collected {references.Count} references..\n");
        }

        using var client = await _chatClientFactory.CreateChatClientAsync(principal, AI.Models.ModelType.Default);
        if (client is not null)
        {
            List<ChatMessage> chatMessagesContext = [.. references, .. context.Messages];
            //await foreach (var update in PreProcessAsync(principal, context, userMessage, chatMessagesContext, client, cancellationToken))
            //{
            //    yield return update;
            //}

            // Finally, add to the user message and call the model
            chatMessagesContext = [.. references, .. context.Messages];
            await foreach (var update in CallModelAsync(principal, context, userMessage, chatMessagesContext, client, cancellationToken))
            {
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
            AllowMultipleToolCalls = true,
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
