using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;

namespace TestBucket.Domain.AI.Agent.Models;
public class HiddenChatMessage : ChatMessage
{
    public HiddenChatMessage(ChatRole role, string? content) : base(role, content)
    {
    }
}
