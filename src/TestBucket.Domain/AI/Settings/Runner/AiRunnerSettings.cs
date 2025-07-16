namespace TestBucket.Domain.AI.Settings.Runner;
internal class AiRunnerSettings
{
    /// <summary>
    /// Is it enabled?
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Max tokens that can be used in a day
    /// </summary>
    public long MaxTokensPerDay { get; set; } = 1_000_000;
}
