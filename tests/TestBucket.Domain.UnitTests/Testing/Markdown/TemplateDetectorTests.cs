using System.Security.Claims;
using TestBucket.Domain.Testing.Markdown;

namespace TestBucket.Domain.UnitTests.Testing.Markdown
{
    /// <summary>
    /// Unit tests for the <see cref="TemplateDetector"/> class, which determines if a <see cref="TestCase"/> is a template
    /// based on the presence of the <c>@Body</c> marker in its description.
    /// </summary>
    [EnrichedTest]
    [UnitTest]
    [FunctionalTest]
    [Component("Testing")]
    public class TemplateDetectorTests
    {
        private readonly TemplateDetector _detector = new();
        private readonly ClaimsPrincipal _principal = new();

        /// <summary>
        /// Verifies that <see cref="TemplateDetector.ProcessAsync"/> sets <c>IsTemplate</c> to <c>true</c>
        /// when the description contains the <c>@Body</c> marker.
        /// </summary>
        [Fact]
        public async Task SetsIsTemplate_True_WhenDescriptionContainsBody()
        {
            var testCase = new TestCase
            {
                Name = "Test 1",
                Description = "This is a template with @Body marker.",
                IsTemplate = false
            };

            await _detector.ProcessAsync(_principal, testCase);

            Assert.True(testCase.IsTemplate);
        }

        /// <summary>
        /// Verifies that <see cref="TemplateDetector.ProcessAsync"/> sets <c>IsTemplate</c> to <c>true</c>
        /// when the description contains the <c>@body</c> marker in a case-insensitive manner.
        /// </summary>
        [Fact]
        public async Task SetsIsTemplate_True_WhenDescriptionContainsBody_CaseInsensitive()
        {
            var testCase = new TestCase
            {
                Name = "Test 2",
                Description = "this contains @body in lowercase.",
                IsTemplate = false
            };

            await _detector.ProcessAsync(_principal, testCase);

            Assert.True(testCase.IsTemplate);
        }

        /// <summary>
        /// Verifies that <see cref="TemplateDetector.ProcessAsync"/> does not set <c>IsTemplate</c>
        /// when the description does not contain the <c>@Body</c> marker.
        /// </summary>
        [Fact]
        public async Task DoesNotSetIsTemplate_WhenDescriptionDoesNotContainBody()
        {
            var testCase = new TestCase
            {
                Name = "Test 3",
                Description = "No marker here.",
                IsTemplate = false
            };

            await _detector.ProcessAsync(_principal, testCase);

            Assert.False(testCase.IsTemplate);
        }

        /// <summary>
        /// Verifies that <see cref="TemplateDetector.ProcessAsync"/> does not change <c>IsTemplate</c>
        /// when the description is <c>null</c>.
        /// </summary>
        [Fact]
        public async Task DoesNotChangeIsTemplate_WhenDescriptionIsNull()
        {
            var testCase = new TestCase
            {
                Name = "Test 4",
                Description = null,
                IsTemplate = false
            };

            await _detector.ProcessAsync(_principal, testCase);

            Assert.False(testCase.IsTemplate);
        }

        /// <summary>
        /// Verifies that <see cref="TemplateDetector.ProcessAsync"/> does not change <c>IsTemplate</c>
        /// when it is already <c>true</c>, even if the description contains the <c>@Body</c> marker.
        /// </summary>
        [Fact]
        public async Task DoesNotChangeIsTemplate_WhenAlreadyTrue()
        {
            var testCase = new TestCase
            {
                Name = "Test 5",
                Description = "Contains @Body but already true.",
                IsTemplate = true
            };

            await _detector.ProcessAsync(_principal, testCase);

            Assert.True(testCase.IsTemplate);
        }
    }
}