using System.ComponentModel;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

using Mediator;

using Microsoft.Extensions.AI;

using TestBucket.Contracts.Integrations;
using TestBucket.Domain.AI;
using TestBucket.Domain.AI.Embeddings;
using TestBucket.Domain.AI.Models;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Progress;
using TestBucket.Domain.Testing;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Traceability;

using static System.Net.Mime.MediaTypeNames;

namespace TestBucket.Domain.Features.Classification;

/// <summary>
/// Classifies entities (tests, requirements, commits..) from a textual description to a predefined list of categories
/// </summary>
internal class GenericClassifier : IClassifier
{
    private readonly IMediator _mediator;
    private readonly IChatClientFactory _chatClientFactory;
    private readonly IProgressManager _progressManager;

    public GenericClassifier(
        IMediator mediator,
        IChatClientFactory chatClientFactory, IProgressManager progressManager)
    {
        _mediator = mediator;
        _chatClientFactory = chatClientFactory;
        _progressManager = progressManager;
    }

    private record class ClassificationResponse
    {
        public string? Category { get; set; }
    }
    public Task<string?> GetModelNameAsync(ModelType modelType)
    {
        return _chatClientFactory.GetModelNameAsync(modelType);
    }

    public async Task<string[]> ClassifyAsync(ClaimsPrincipal principal, string fieldName, IReadOnlyList<GenericVisualEntity> categories, LocalIssue issue)
    {
        var hasEmbeddings = categories.Where(x=>x.Embedding != null).Any();
        if(hasEmbeddings && issue.TestProjectId is not null)
        {
            var response = await _mediator.Send(new GenerateEmbeddingRequest(issue.TestProjectId.Value, issue.Title + " " + issue.Description));
            if (response.EmbeddingVector is not null)
            {
                var queryVector = response.EmbeddingVector.Value.ToArray();
                var ordered = categories.Where(x=>x.Embedding is not null).OrderByDescending(x => CosineSimilarity.Calculate(queryVector, x.Embedding)).ToList();
                return [ordered.First().Title!];
            }
        }
        return await ClassifyAsync(principal, fieldName, categories.Where(x => x.Title != null).Select(x => x.Title!).ToArray(), issue);
    }

    public async Task<string[]> ClassifyAsync(ClaimsPrincipal principal, string fieldName, string[] categories, LocalIssue issue)
    {
        string title = issue.Title ?? "";
        string description = issue.Description ?? title;
        if (categories.Length == 0)
        {
            return [];
        }
        if (categories.Length == 1)
        {
            return categories;
        }

        var systemPromptBuilder = new StringBuilder();
        systemPromptBuilder.AppendLine("""
            You are a classification agent that analyzes information about a bug report and summarizes and categorizes
            what it does.
            """);
        var systemPromptMessage = new ChatMessage(ChatRole.System, systemPromptBuilder.ToString());

        var reasoning = new StringBuilder();
        reasoning.AppendLine($"""
            Below is the information about the bug report.
            Can you analyze it and tell me what it is about?

            Title: {title}
            Description:
            {description}
            """);
        reasoning.AppendLine();

        var classification = new StringBuilder();
        classification.AppendLine($"Which one of these catagories describes \"{fieldName}\" the best?");

        foreach (var category in categories)
        {
            classification.AppendLine("- " + category);
        }
        classification.AppendLine("Respond **only** with a single category.");

        var chatClient = await _chatClientFactory.CreateChatClientAsync(ModelType.Classification);
        if (chatClient is not null)
        {
            List<ChatMessage> chatHistory =
                [
                    new ChatMessage(ChatRole.System, systemPromptBuilder.ToString()),
                    new ChatMessage(ChatRole.User, reasoning.ToString()),
                ];

            var reasoningResponse = await chatClient.GetResponseAsync(chatHistory);
            chatHistory.AddRange(reasoningResponse.Messages);
            chatHistory.Add(new ChatMessage(ChatRole.User, classification.ToString()));
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
        }
        return [];
    }

    public async Task<string[]> ClassifyAsync(ClaimsPrincipal principal, string fieldName, IReadOnlyList<GenericVisualEntity> categories, TestCase issue)
    {
        var hasEmbeddings = categories.Where(x => x.Embedding != null).Any();
        if (hasEmbeddings && issue.TestProjectId is not null)
        {
            var response = await _mediator.Send(new GenerateEmbeddingRequest(issue.TestProjectId.Value, issue.Name + " " + issue.Description));
            if (response.EmbeddingVector is not null)
            {
                var queryVector = response.EmbeddingVector.Value.ToArray();
                var ordered = categories.Where(x => x.Embedding is not null).OrderBy(x => CosineSimilarity.Calculate(queryVector, x.Embedding)).ToList();
                return [ordered.First().Title!];
            }
        }
        return await ClassifyAsync(principal, fieldName, categories.Where(x => x.Title != null).Select(x => x.Title!).ToArray(), issue);
    }
    public async Task<string[]> ClassifyAsync(ClaimsPrincipal principal, string fieldName, string[] categories, TestCase testCase)
    {
        string testName = testCase.Name;
        string description = testCase.Description ?? testCase.Name;
        if (categories.Length == 0)
        {
            return [];
        }
        if (categories.Length == 1)
        {
            return categories;
        }

        var traceResult = await _mediator.Send(new DiscoverTestCaseRelationshipsRequest(principal, testCase, 2));

        var systemPromptBuilder = new StringBuilder();
        systemPromptBuilder.AppendLine("""
            You are a classification agent that analyzes information about a test case and summarizes and categorizes
            what it does.
            """);
        var systemPromptMessage = new ChatMessage(ChatRole.System, systemPromptBuilder.ToString());

        var reasoning = new StringBuilder();
        reasoning.AppendLine($"""
            Below is the information about the test case.
            Can you analyze it and tell me what it is testing?

            Test name: {testName}
            Folder path: {testCase.Path}
            Description:
            {description}
            """);
        reasoning.AppendLine();

        foreach (var relationship in traceResult.Upstream)
        {
            if(relationship.Requirement is not null)
            {
                reasoning.AppendLine("Here is a requirement related to the test case:");
                reasoning.AppendLine(relationship.Requirement.Description);
                reasoning.AppendLine();
            }
        }

        var classification = new StringBuilder();
        classification.AppendLine($"Which one of these catagories matches what the tested \"{fieldName}\" the best?");

        foreach (var category in categories)
        {
            classification.AppendLine("- " + category);
        }
        classification.AppendLine("Respond **only** with a single category.");

        var chatClient = await _chatClientFactory.CreateChatClientAsync(ModelType.Classification);
        if (chatClient is not null)
        {
            List<ChatMessage> chatHistory = 
                [
                    new ChatMessage(ChatRole.System, systemPromptBuilder.ToString()),
                    new ChatMessage(ChatRole.User, reasoning.ToString()),
                ];

            var reasoningResponse = await chatClient.GetResponseAsync(chatHistory);
            chatHistory.AddRange(reasoningResponse.Messages);
            chatHistory.Add(new ChatMessage(ChatRole.User, classification.ToString()));
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
        }
        return [];
    }
}
