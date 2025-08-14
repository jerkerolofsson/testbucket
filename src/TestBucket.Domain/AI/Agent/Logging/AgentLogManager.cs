using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;

using TestBucket.Domain.AI.Billing;
using TestBucket.Domain.AI.Settings.LLM;

namespace TestBucket.Domain.AI.Agent.Logging;
internal class AgentLogManager : IAgentLogManager
{
    private readonly IAIUsageManager _manager;
    private readonly ISettingsProvider _settingsProvider;
    private LlmSettings? _settings = null;

    public AgentLogManager(IAIUsageManager manager, ISettingsProvider settingsProvider)
    {
        _manager = manager;
        _settingsProvider = settingsProvider;
    }

    public async ValueTask<ChatUsage?> LogResponseAsync(long? teamId, long? projectId, string orchestrationStrategy, ClaimsPrincipal principal, ChatMessageContent response)
    {
        if (response.Metadata is not null && response.Metadata.TryGetValue("Usage", out var usage))
        {
            string? modelId = response.ModelId;
            if (usage is OpenAI.Chat.ChatTokenUsage openAiUsage)
            {
                string category = orchestrationStrategy;
                return await LogUsageAsync( teamId, projectId, category, principal, openAiUsage.InputTokenCount, openAiUsage.OutputTokenCount, openAiUsage.TotalTokenCount);
            }
        }
        return null;
    }

    private async ValueTask<ChatUsage?> LogUsageAsync(long? teamId, long? projectId, string category, ClaimsPrincipal principal, long? inputTokenCount, long? outputTokenCount, long? totalTokenCount)
    {
        string tenantId = principal.GetTenantIdOrThrow();
        inputTokenCount ??= 0;
        outputTokenCount ??= 0;
        totalTokenCount ??= inputTokenCount + totalTokenCount;
        if(totalTokenCount > 0)
        {
            _settings ??= await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(tenantId, null);

            var cost = new ChatUsage
            {
                TenantId = tenantId,
                TestProjectId = projectId,
                TeamId = teamId,
                UsageCategory = category,
                InputTokenCount = inputTokenCount.Value,
                OutputTokenCount = outputTokenCount.Value,
                TotalTokenCount = totalTokenCount.Value,
                UsdPerMillionInputTokens = _settings?.LlmModelUsdPerMillionInputTokens ?? 0,
                UsdPerMillionOutputTokens = _settings?.LlmModelUsdPerMillionOutputTokens ?? 0
            };
            await _manager.AddCostAsync(principal, cost);
            return cost;
        }
        return null;
    }

    public async ValueTask<ChatUsage?> LogResponseAsync(long? teamId, long? projectId, ClaimsPrincipal principal, ChatResponseUpdate response)
    {
        var accumulatedUsage = new ChatUsage() { InputTokenCount = 0, OutputTokenCount = 0, UsageCategory = "", TotalTokenCount = 0 };
        if (response.Contents is not null)
        {
            string? modelId = response.ModelId;
            foreach (var content in response.Contents)
            {
                if(content is UsageContent usageContent)
                {
                    var usage = await LogUsageAsync(teamId, projectId, "Chat", principal, usageContent.Details.InputTokenCount, usageContent.Details.OutputTokenCount, usageContent.Details.TotalTokenCount);
                    if(usage is not null)
                    {
                        accumulatedUsage.Add(usage);
                    }
                }
            }
        }
        return accumulatedUsage;
    }
}
