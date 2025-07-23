using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace TestBucket.Domain.AI;
internal static class ChatMessageExtensions
{
    public static ChatMessageContent ToSemanticKernelChatMessage(this Microsoft.Extensions.AI.ChatMessage message)
    {
        // Map the role
        AuthorRole role;

        if (message.Role == ChatRole.User)
        {
            role = AuthorRole.User;
        }
        else if (message.Role == ChatRole.Assistant)
        {
            role = AuthorRole.Assistant;
        }
        else if (message.Role == ChatRole.System)
        {
            role = AuthorRole.System;
        }
        else if (message.Role == ChatRole.Tool)
        {
            role = AuthorRole.Tool;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(message.Role), "Unsupported role");
        }

        // Create a new SemanticKernel ChatMessage
        return new ChatMessageContent(role, message.Text);
    }
}
