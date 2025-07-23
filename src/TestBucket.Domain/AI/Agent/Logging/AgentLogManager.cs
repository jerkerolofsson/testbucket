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

    public async ValueTask LogResponseAsync(long? teamId, long? projectId, string orchestrationStrategy, ClaimsPrincipal principal, ChatMessageContent response)
    {
        if (response.Metadata is not null && response.Metadata.TryGetValue("Usage", out var usage))
        {
            string? modelId = response.ModelId;
            if (usage is OpenAI.Chat.ChatTokenUsage openAiUsage)
            {
                string category = orchestrationStrategy;
                await LogUsageAsync( teamId, projectId, category, principal, openAiUsage.InputTokenCount, openAiUsage.OutputTokenCount, openAiUsage.TotalTokenCount);
            }
        }
    }

    private async ValueTask LogUsageAsync(long? teamId, long? projectId, string category, ClaimsPrincipal principal, long? inputTokenCount, long? outputTokenCount, long? totalTokenCount)
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
                UsdPerMillionTokens = _settings?.LlmModelUsdPerMillionTokens ?? 0
            };
            await _manager.AddCostAsync(principal, cost);
        }
    }

    public async ValueTask LogResponseAsync(long? teamId, long? projectId, ClaimsPrincipal principal, ChatResponseUpdate response)
    {
        if (response.Contents is not null)
        {
            string? modelId = response.ModelId;
            foreach (var content in response.Contents)
            {
                if(content is UsageContent usageContent)
                {
                    await LogUsageAsync(teamId, projectId, "Chat", principal, usageContent.Details.InputTokenCount, usageContent.Details.OutputTokenCount, usageContent.Details.TotalTokenCount);
                }
            }
        }
    }
}
