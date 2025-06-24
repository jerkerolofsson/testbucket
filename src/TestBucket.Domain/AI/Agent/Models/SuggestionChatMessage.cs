using Microsoft.Extensions.AI;

namespace TestBucket.Domain.AI.Agent.Models;

/// <summary>
/// A message invoked when a user clicked on a suggestion.
/// The complete prompt will be hidden in the chat UI when it is clicked on
/// </summary>
public class SuggestionChatMessage : ChatMessage
{
    public Suggestion Suggestion { get; }
    public SuggestionChatMessage(Suggestion suggestion) : base(ChatRole.User, suggestion.Text)
    {
        Suggestion = suggestion;
    }
}
