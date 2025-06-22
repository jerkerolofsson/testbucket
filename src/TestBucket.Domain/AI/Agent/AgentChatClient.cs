using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

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
        ChatMessage[] chatMessages = [..references, ..context.Messages, userMessage];

        // Add to history
        context.Messages.Add(userMessage);

        var options = new ChatOptions
        {
            AllowMultipleToolCalls = true,
            ToolMode = ChatToolMode.Auto,
            Tools = [..GetTools(principal).Functions]
        };

        using var client = await _chatClientFactory.CreateChatClientAsync(AI.Models.ModelType.Default);
        if (client is not null)
        {
            List<ChatResponseUpdate> updates = [];
            try
            {
                var update = await client.GetResponseAsync(chatMessages, options, cancellationToken);

                foreach(var message in update.Messages)
                {
                    yield return new ChatResponseUpdate
                    {
                        Role = message.Role,
                        Contents = message.Contents,
                    };
                    context.Messages.Add(message);
                }
                /*
                await foreach (var update in client.GetStreamingResponseAsync(chatMessages, options, cancellationToken))
                {
                    updates.Add(update);
                    yield return update;
                }*/
            }
            finally
            {
                var response = ChatResponseExtensions.ToChatResponse(updates);
                context.Messages.AddRange(response.Messages);
                
                //context.Messages.AddRange(ToChatMessage(updates));
            }

        }
    }
}
