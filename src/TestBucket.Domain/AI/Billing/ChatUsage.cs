namespace TestBucket.Domain.AI.Billing;
public class ChatUsage : ProjectEntity
{
    public long Id { get; set; }

    public required string UsageCategory { get; set; }
    public required long InputTokenCount { get; set; }
    public required long OutputTokenCount { get; set; }
    public required long TotalTokenCount { get; set; }

    /// <summary>
    /// Price for 1 000 000 tokens in USD.
    /// </summary>
    public double UsdPerMillionTokens { get; set; }

}
