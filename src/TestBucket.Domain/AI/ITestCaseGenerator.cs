using TestBucket.Domain.AI.Models;
using TestBucket.Domain.Testing.Heuristics.Models;

namespace TestBucket.Domain.AI;
public interface ITestCaseGenerator
{
    bool Enabled { get; }

    IReadOnlyList<Heuristic> Heuristics { get; }

    Task GenerateTestsAsync(ClaimsPrincipal principal, GenerateTestOptions options);
}