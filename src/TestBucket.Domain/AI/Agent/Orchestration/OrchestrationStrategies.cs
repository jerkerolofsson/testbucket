namespace TestBucket.Domain.AI.Agent.Orchestration;
public sealed class OrchestrationStrategies
{
    public const string Default = "default-orchestration";
    public const string AiRunner = "ai-runner";
    public const string AddTests = "add-tests";
    public const string DraftTests = "draft-tests";
    public const string ReviewTests = "review-tests";


    public const string AddRequirements = "add-requirements";
    public const string DraftRequirements = "draft-requirements";

    public const string FindSimilarIssues = "find-similar-issues";
    public const string SummarizeIssue = "summarize-issue";

}
