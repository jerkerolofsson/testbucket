using TestBucket.Formats;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Automation.Helpers;
public class TestRunnerResultParser
{
    public static TestCaseRunDto? ParseSingleResult(string? content, TestResultFormat? format)
    {
        if (content is not null && format is not null && format != TestResultFormat.UnknownFormat)
        {
            var serializer = TestResultSerializerFactory.Create(format.Value);
            var testRun = serializer.Deserialize(content);
            if (testRun.Suites.Count > 0 && testRun.Suites[0].Tests.Count > 0)
            {
                return testRun.Suites[0].Tests[0];
            }
        }
        return null;
    }
}
