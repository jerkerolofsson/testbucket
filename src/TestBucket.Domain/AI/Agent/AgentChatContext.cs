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
    public long? ProjectId { get; set; }

    /// <summary>
    /// Documents etc
    /// </summary>
    public List<ChatReference> References { get; } = [];

    /// <summary>
    /// Messages
    /// </summary>
    public List<ChatMessage> Messages { get; set; } = [];

    public void ClearMessages()
    {
        Messages.Clear();
    }

    public void ClearReferences()
    {
        References.Clear();
    }

}
