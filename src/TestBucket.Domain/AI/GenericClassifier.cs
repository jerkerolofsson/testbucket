using System.ComponentModel;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.AI;

using TestBucket.Domain.AI.Models;
using TestBucket.Domain.Progress;
using TestBucket.Domain.Testing;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.AI;

/// <summary>
/// Classifies entities (tests, requirements, commits..) from a textual description to a predefined list of categories
/// </summary>
internal class GenericClassifier : IClassifier
{
    private readonly IChatClientFactory _chatClientFactory;
    private readonly IProgressManager _progressManager;

    public GenericClassifier(IChatClientFactory chatClientFactory, IProgressManager progressManager)
    {
        _chatClientFactory = chatClientFactory;
        _progressManager = progressManager;
    }

    private record class ClassificationResponse
    {
        public string? Category { get; set; }
    }

    public async Task<string[]> ClassifyAsync(string[] categories, string userPrompt)
    {
        if (categories.Length == 0)
        {
            return [];
        }
        if (categories.Length == 1)
        {
            return categories;
        }

        var systemPromptBuilder = new StringBuilder();
        systemPromptBuilder.AppendLine("Instructions:");
        //systemPromptBuilder.AppendLine(string.Join(", ", categories));
        systemPromptBuilder.AppendLine("Classify the input as one of the following categories:");
        foreach (var category in categories)
        {
            systemPromptBuilder.AppendLine("- " + category);
        }
        systemPromptBuilder.AppendLine("Do not select any other category, reply only with one of these: " + string.Join(", ", categories));

        var userPromptBuilder = new StringBuilder();
        userPromptBuilder.AppendLine("INPUT:");
        userPromptBuilder.AppendLine(userPrompt);

        var chatClient = await _chatClientFactory.CreateChatClientAsync(ModelType.Classification);
        if (chatClient is not null)
        {
            var systemPromptMessage = new ChatMessage(ChatRole.System, systemPromptBuilder.ToString());
            var chatMessage = new ChatMessage(ChatRole.User, userPromptBuilder.ToString());
            List<ChatMessage> chatHistory = [systemPromptMessage, chatMessage];
            
            var response = await chatClient.GetResponseAsync(chatHistory);
            if (response?.Text is not null)
            {
                foreach (var category in categories)
                {
                    if (response.Text.Contains(category, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return [category];
                    }
                }
            }

            //var response = await chatClient.GetResponseAsync(chatHistory);
            //if (response.Result is not null && response.Result.Category is not null)
            //{
            //    return [response.Result.Category];
            //}
            //else
            //{
            //}
        }
        return [];
    }
}
