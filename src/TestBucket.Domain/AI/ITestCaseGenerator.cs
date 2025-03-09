
using Microsoft.Extensions.AI;

using TestBucket.Domain.AI.Models;

namespace TestBucket.Domain.AI;
public interface ITestCaseGenerator
{
    bool Enabled { get; }

    IReadOnlyList<Heuristic> Heuristics { get; }

    IAsyncEnumerable<LlmGeneratedTestCase?> GetStreamingResponseAsync(GenerateTestOptions options);
}