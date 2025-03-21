
using System.Security.Claims;

using Microsoft.Extensions.AI;

using TestBucket.Domain.AI.Models;

namespace TestBucket.Domain.AI;
public interface ITestCaseGenerator
{
    bool Enabled { get; }

    IReadOnlyList<Heuristic> Heuristics { get; }

    Task GenerateTestsAsync(ClaimsPrincipal principal, GenerateTestOptions options);
}