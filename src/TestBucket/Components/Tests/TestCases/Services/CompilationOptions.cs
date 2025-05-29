namespace TestBucket.Components.Tests.TestCases.Services;

public record class CompilationOptions(TestCase TestCase, string Text)
{
    public string ContextGuid { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// If true, resources are released directly after allocated
    /// </summary>
    public bool ReleaseResourceDirectly { get; set; } = true;

    /// <summary>
    /// If true, resources will be allocated
    /// </summary>
    public bool AllocateResources { get; set; } = false;

    /// <summary>
    /// ID of the run
    /// </summary>
    public long? TestRunId { get; set; }
}
