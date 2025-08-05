using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestCases.Templates;
public interface ITestCaseTemplate
{
    /// <summary>
    /// Name of template
    /// </summary>
    string Name { get; }

    /// <summary>
    /// SVG icon
    /// </summary>
    string? Icon { get; }

    /// <summary>
    /// Applies the template to the test case
    /// </summary>
    /// <param name="test"></param>
    /// <returns></returns>
    ValueTask ApplyAsync(TestCase test);
}
