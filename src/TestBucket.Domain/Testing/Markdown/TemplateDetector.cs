using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Markdown
{
    /// <summary>
    /// Detects whether a <see cref="TestCase"/> is a template based on its description content.
    /// </summary>
    internal class TemplateDetector : IMarkdownDetector
    {
        /// <summary>
        /// Processes the specified <see cref="TestCase"/> and sets its <c>IsTemplate</c> property to <c>true</c>
        /// if the description contains the <c>@Body</c> marker (case-insensitive).
        /// </summary>
        /// <param name="principal">The user principal performing the operation.</param>
        /// <param name="testCase">The test case to process and update.</param>
        public Task ProcessAsync(ClaimsPrincipal principal, TestCase testCase)
        {
            if (testCase.Description is not null && testCase.IsTemplate == false)
            {
                testCase.IsTemplate = testCase.Description.Contains("@Body", StringComparison.InvariantCultureIgnoreCase);
            }
            return Task.CompletedTask;
        }
    }
}