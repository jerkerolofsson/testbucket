using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
using Microsoft.SemanticKernel.Agents.Orchestration.Sequential;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.ChatCompletion;

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
    private readonly ILogger<AgentChatClient> _logger;

    public AgentChatClient(IChatClientFactory chatClientFactory, IServiceProvider serviceProvider)
    {
        _chatClientFactory = chatClientFactory;
        _serviceProvider = serviceProvider;
        _mcpServerRunnerManager = serviceProvider.GetRequiredService<McpServerRunnerManager>();
        _testCaseManager = serviceProvider.GetRequiredService<ITestCaseManager>();
        _testExecutionContextBuilder = serviceProvider.GetRequiredService<TestExecutionContextBuilder>();
        _semanticKernelFactory = serviceProvider.GetRequiredService<SemanticKernelFactory>();
        _logger = serviceProvider.GetRequiredService<ILogger<AgentChatClient>>();
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

    private ChatCompletionAgent CreateChatCompletionAgent(string name, string instructions, string description, Kernel kernel)
    {
        // This method is not used in the current implementation
        // It can be used to create a custom chat completion agent
        // var agent = new ChatCompletionAgent(name, instructions, description);
        // return agent;
        return new ChatCompletionAgent()
        {
            Name = name,
            Instructions = instructions,
            Description = description,
            Kernel = kernel,
            LoggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>(),
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };
    }

    public async IAsyncEnumerable<ChatResponseUpdate> InvokeAgentAsync(
        string? agentId,
        ClaimsPrincipal principal, 
        TestProject? project,
        AgentChatContext context,
        ChatMessage userMessage,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ChatHistory newMessages = []; // This only contain the new messages
        ChatHistory groupChatHistory = [];
//        var reducer = new ChatHistoryTruncationReducer(targetCount: 3);

        // Enrich context with references
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
            foreach(var historyMessage in context.Messages)
            {
                groupChatHistory.Add(historyMessage.ToSemanticKernelChatMessage());
            }
        }
        context.Messages.Add(userMessage);

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

        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        List<ChatCompletionAgent> members = [];
        switch(agentId)
        {
            case "test-designer":
                // Analyze inputs
                members.Add(RequirementAnalystAgent.Create(kernel, loggerFactory));
                //members.Add(TestCoverageAnalystAgent.Create(kernel, loggerFactory));

                // Create test
                members.Add(TestCaseDesignerAgent.Create(kernel, loggerFactory));

                // Review
                members.Add(TestReviewerAgent.Create(kernel, loggerFactory));

                // Terminate
                //members.Add(TestCaseDesignerTerminatorAgent.Create(kernel, loggerFactory));
                break;

            case "requirement-designer":
                // Analyze inputs
                members.Add(RequirementAnalystAgent.Create(kernel, loggerFactory));
                members.Add(TestCoverageAnalystAgent.Create(kernel, loggerFactory));

                // Create
                members.Add(RequirementDesignerAgent.Create(kernel, loggerFactory));

                // Review
                members.Add(RequirementReviewerAgent.Create(kernel, loggerFactory));
                break;

            case "requirement-creator":
                // Analyze inputs
                members.Add(RequirementAnalystAgent.Create(kernel, loggerFactory));
                members.Add(TestCoverageAnalystAgent.Create(kernel, loggerFactory));

                // Design/Draft
                members.Add(RequirementDesignerAgent.Create(kernel, loggerFactory));

                // Review
                members.Add(RequirementReviewerAgent.Create(kernel, loggerFactory));

                // Add
                members.Add(RequirementCreatorAgent.Create(kernel, loggerFactory));
                break;

            case "test-creator":
                // Analyze inputs
                members.Add(RequirementAnalystAgent.Create(kernel, loggerFactory));
                members.Add(TestCoverageAnalystAgent.Create(kernel, loggerFactory));

                // Design/Draft
                members.Add(TestCaseDesignerAgent.Create(kernel, loggerFactory));

                // Review
                members.Add(TestReviewerAgent.Create(kernel, loggerFactory));

                // Add
                members.Add(TestCreatorAgent.Create(kernel, loggerFactory));
                break;

            default:
                members.Add(DefaultAgent.Create(kernel, loggerFactory));
                break;
        }

        //ChatCompletionAgent analystAgent =
        //   this.CreateChatCompletionAgent(
        //       name: "Analyst",
        //       instructions:
        //       """
        //        You are a marketing analyst. Given a product description, identify:
        //        - Key features
        //        - Target audience
        //        - Unique selling points
        //        """,
        //       description: "A agent that extracts key concepts from a product description.", kernel);
        //ChatCompletionAgent writerAgent =
        //    this.CreateChatCompletionAgent(
        //        name: "copywriter",
        //        instructions:
        //        """
        //        You are a marketing copywriter. Given a block of text describing features, audience, and USPs,
        //        compose a compelling marketing copy (like a newsletter section) that highlights these points.
        //        Output should be short (around 150 words), output just the copy as a single text block.
        //        """,
        //        description: "An agent that writes a marketing copy based on the extracted concepts.", kernel);
        //ChatCompletionAgent editorAgent =
        //    this.CreateChatCompletionAgent(
        //        name: "editor",
        //        instructions:
        //        """
        //        You are an editor. Given the draft copy, correct grammar, improve clarity, ensure consistent tone,
        //        give format and make it polished. Output the final improved copy as a single text block.
        //        """,
        //        description: "An agent that formats and proofreads the marketing copy.", kernel);

        //members = [analystAgent, writerAgent, editorAgent];

        var channel = Channel.CreateUnbounded<StreamingChatMessageContent>();
        var chatManager = new LoggingRoundRobinGroupChatManager(_serviceProvider.GetRequiredService<ILogger<LoggingRoundRobinGroupChatManager>>())
        {
            MaximumInvocationCount = members.Count*2,
        };

        var orchestration = new GroupChatOrchestrationWithChatHistory(groupChatHistory, chatManager, members.ToArray())
        //var orchestration = new GroupChatOrchestration<string, string>(chatManager, members.ToArray())
        //var orchestration = new SequentialOrchestration(members.ToArray())
        {
            Name = $"GroupChatOrchestrationWithChatHistory ({agentId})",
            Description = $"Group chat orchestration for agent {agentId}",
            LoggerFactory = loggerFactory,
            ResponseCallback = (response) =>
            {
                _logger.LogInformation("Received response from agent {AuthorName}", response.AuthorName);
                newMessages.Add(response);
                groupChatHistory.Add(response);

                return ValueTask.CompletedTask;
            },
            StreamingResponseCallback = async (StreamingChatMessageContent response, bool isFinal) =>
            {
                await channel.Writer.WriteAsync(response);
            }
        };

        await using InProcessRuntime runtime = new InProcessRuntime();
        await runtime.StartAsync();

        try
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var task = Task.Run(async () =>
            {
                //string input = "An eco-friendly stainless steel water bottle that keeps drinks cold for 24 hours";
                var result = await orchestration.InvokeAsync(userMessage.Text, runtime, cancellationToken: cts.Token);
                //var result = await orchestration.InvokeAsync(input, runtime, cancellationToken: cts.Token);
                try
                {
                    string output = await result.GetValueAsync(TimeSpan.FromSeconds(300), cts.Token);
                    await runtime.RunUntilIdleAsync();
                }
                catch (Exception) { }
                finally
                {
                    cts.Cancel();
                }
            });

            // Read from channel and yield restult in order to update the UI
            while(!cts.IsCancellationRequested)
            {
                ChatResponseUpdate? update = null;
                try
                {
                    var response = await channel.Reader.ReadAsync(cts.Token);
                    update = response.ToChatResponseUpdate();
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error converting response to ChatResponseUpdate");
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

    public async IAsyncEnumerable<ChatResponseUpdate> AskAsync(ClaimsPrincipal principal, 
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
