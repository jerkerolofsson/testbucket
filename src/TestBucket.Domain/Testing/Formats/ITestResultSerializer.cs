using TestBucket.Domain.Testing.Formats.Dtos;

namespace TestBucket.Domain.Testing.Formats;

/// <summary>
/// Serializes and deserializes test runs
/// </summary>
public interface ITestResultSerializer
{
    TestRunDto Deserialize(string text);
    string Serialize(TestRunDto testRun);
}