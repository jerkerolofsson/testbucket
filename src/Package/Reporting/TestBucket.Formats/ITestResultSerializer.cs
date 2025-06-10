namespace TestBucket.Formats;

/// <summary>
/// Serializes and deserializes test runs.
/// </summary>
public interface ITestResultSerializer
{
    /// <summary>
    /// Deserializes a test run from its string representation.
    /// </summary>
    /// <param name="text">The serialized test run as a string.</param>
    /// <returns>A <see cref="TestRunDto"/> representing the deserialized test run.</returns>
    TestRunDto Deserialize(string text);

    /// <summary>
    /// Serializes a test run to its string representation.
    /// </summary>
    /// <param name="testRun">The <see cref="TestRunDto"/> to serialize.</param>
    /// <returns>A string containing the serialized test run.</returns>
    string Serialize(TestRunDto testRun);
}