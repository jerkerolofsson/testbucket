namespace TestBucket.Domain.AI.Billing;
public class TokenUsage
{
    public long InputTokenCount { get; set; }
    public long OutputTokenCount { get; set; }
    public long TotalTokenCount { get; set; }
    public double InputSumUSD { get; set; }
    public double OutputSumUSD { get; set; }
    public double TotalSumUSD { get; set; }
}
