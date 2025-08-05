using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestCases.Templates;
internal class DotHttpTemplate : ITestCaseTemplate
{
    public string Name => "dothttp.api";

    public string? Icon => "<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" viewBox=\"0 0 180 180\" style=\"shape-rendering:geometricPrecision; text-rendering:geometricPrecision; image-rendering:optimizeQuality; fill-rule:evenodd; clip-rule:evenodd\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">\r\n<g><path style=\"opacity:0.99\" fill=\"#fefffe\" d=\"M 108.5,19.5 C 132.707,15.9062 147.54,25.9062 153,49.5C 153.711,74.5473 155.211,99.5473 157.5,124.5C 154.622,155.182 137.955,167.349 107.5,161C 86.1502,152.159 64.4835,144.159 42.5,137C 14.7262,119.347 8.89287,96.1803 25,67.5C 33.8153,56.0418 45.3153,48.5418 59.5,45C 70.8333,38.3333 82.1667,31.6667 93.5,25C 98.4466,22.6923 103.447,20.859 108.5,19.5 Z M 114.5,43.5 C 123.085,42.9149 127.918,46.9149 129,55.5C 130.168,80.176 131.168,104.843 132,129.5C 130.877,132.249 129.377,134.749 127.5,137C 124.964,137.339 122.464,137.839 120,138.5C 97.5,130.333 75,122.167 52.5,114C 37.2673,102.206 36.6007,89.5398 50.5,76C 70.0259,67.5709 88.6926,57.5709 106.5,46C 109.379,45.454 112.046,44.6206 114.5,43.5 Z\"/></g>\r\n</svg>\r\n";

    public ValueTask ApplyAsync(TestCase test)
    {
        test.RunnerLanguage = "http";
        test.ExecutionType = TestExecutionType.Automated;
        test.ScriptType = ScriptType.ScriptedDefault;

        test.Description = $"""
            # @name {test.Name}
            # @verify http status-code == 200
            GET https://www.github.com HTTP/1.1
            Accept: application/json
            """;

        return ValueTask.CompletedTask;
    }
}
