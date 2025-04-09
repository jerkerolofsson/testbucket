
namespace TestBucket.Formats.Builders;

public interface ISharedBuilderBase
{
    /// <summary>
    /// Adds a new test suite
    /// </summary>
    /// <returns></returns>
    ITestSuiteBuilder AddTestSuite();

    /// <summary>
    /// Builds the result file, returning the content as a string (XML, json etc depending on format)
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    string Build(TestResultFormat format);
}