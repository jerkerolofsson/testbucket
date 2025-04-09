namespace TestBucket.Formats;

/// <summary>
/// Serializes and deserializes test runs
/// </summary>
public interface ITestResultSerializer
{
    TestRunDto Deserialize(string text);
    string Serialize(TestRunDto testRun);
}