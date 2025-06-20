using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;

using TestBucket.Domain.AI.Agent.Models;

namespace TestBucket.Domain.AI.Agent;
public class AgentChatContext
{
    /// <summary>
    /// Documents etc
    /// </summary>
    public List<ChatReference> References { get; } = [];

    /// <summary>
    /// Messages
    /// </summary>
    public List<ChatMessage> Messages { get; set; } = [];

    internal IReadOnlyList<ChatMessage> GetReferencesAsChatMessages()
    {
        List<ChatMessage> messages = [];

        foreach(var reference in References)
        {
            IList<AIContent> content = [];
            var xml = $"""
                <reference>
                    <name>{reference.Name}</name>
                    <id>{reference.Id}</url>
                    <type>{reference.EntityTypeName ?? ""}</type>
                    <description>{reference.Text}</description>
                </reference>
                """;
            content.Add(new TextContent(xml));
            messages.Add(new ChatMessage(ChatRole.System, content));
        }

        return messages;
    }
}
