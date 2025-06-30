using System.Runtime.CompilerServices;

using Microsoft.Extensions.AI;

using TestBucket.Domain.AI.Agent.Models;
using TestBucket.Domain.AI.Tools;
using TestBucket.Domain.Identity;

namespace TestBucket.Domain.AI.Agent;
public class AgentChatClient
{
    private readonly IChatClientFactory _chatClientFactory;
    private readonly IServiceProvider _serviceProvider;

    public AgentChatClient(IChatClientFactory chatClientFactory, IServiceProvider serviceProvider)
    {
        _chatClientFactory = chatClientFactory;
        _serviceProvider = serviceProvider;
    }

    private ToolCollection GetTools(ClaimsPrincipal principal)
    {
        var toolCollection = new ToolCollection(_serviceProvider);

        // Index all MCP tools in assembly
        toolCollection.AddMcpServerToolsFromAssembly(principal, GetType().Assembly);

        return toolCollection;
    }

    /// <summary>
    /// Pre-processing is used to inject extra data into the context based on the user prompt
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="project"></param>
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

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(ClaimsPrincipal principal, 
        TestProject? project,
        AgentChatContext context, 
        ChatMessage userMessage, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if(project is not null)
        {
            // Add the project claim
            principal = Impersonation.ChangeProject(principal, project.Id);
        }

        // Chat context
        var references = context.GetReferencesAsChatMessages();
        

        using var client = await _chatClientFactory.CreateChatClientAsync(AI.Models.ModelType.Default);
        if (client is not null)
        {
            List<ChatMessage> chatMessagesContext = [.. references, .. context.Messages];
            await foreach (var update in PreProcessAsync(principal, context, userMessage, chatMessagesContext, client, cancellationToken))
            {
                yield return update;
            }

            // Finally, add to the user message and call the model
            chatMessagesContext = [.. references, .. context.Messages];
            await foreach (var update in CallModelAsync(principal, context, userMessage, chatMessagesContext, client, cancellationToken))
            {
                yield return update;
            }
        }
    }

    internal async IAsyncEnumerable<ChatResponseUpdate> CallModelAsync(ClaimsPrincipal principal, AgentChatContext context, ChatMessage userMessage, List<ChatMessage> chatMessages, IChatClient client,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        chatMessages.Add(userMessage);
        context.Messages.Add(userMessage);

        // Trigger a UI update
        yield return new ChatResponseUpdate();

        var options = new ChatOptions
        {
            AllowMultipleToolCalls = true,
            ToolMode = ChatToolMode.Auto,
            Tools = [.. GetTools(principal).Functions]
        };

        List<ChatResponseUpdate> updates = [];
        try
        {
            var update = await client.GetResponseAsync(chatMessages, options, cancellationToken);

            foreach (var message in update.Messages)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                yield return new ChatResponseUpdate
                {
                    Role = message.Role,
                    Contents = message.Contents,
                };
                context.Messages.Add(message);
            }
        }
        finally
        {
            var response = ChatResponseExtensions.ToChatResponse(updates);
            context.Messages.AddRange(response.Messages);
        }
    }
}
