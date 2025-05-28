using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.IntegrationTests.Requirements
{
    [IntegrationTest]
    [EnrichedTest]
    [FunctionalTest]
    [Feature("Import Requirements")]
    public class ImportRequirementTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        private string mConventionalCommitsMarkdownSpec = """
            # feat type
            [CC-1]
            The type feat MUST be used when a commit adds a new feature to your application or library.

            # fix type
            [CC-2]
            The type fix MUST be used when a commit represents a bug fix for your application.

            # Scope after type
            [CC-3]
            A scope MAY be provided after a type. A scope MUST consist of a noun describing a section of the codebase surrounded by parenthesis, e.g., fix(parser):


            # Description follows colon
            [CC-4]
            A description MUST immediately follow the colon and space after the type/scope prefix. The description is a short summary of the code changes, e.g., fix: array parsing issue when multiple spaces were contained in string.

            # Longer commit body may be provided
            [CC-5]
            A longer commit body MAY be provided after the short description, providing additional contextual information about the code changes. The body MUST begin one blank line after the description.

            # Commit body is free-form
            [CC-6]
            A commit body is free-form and MAY consist of any number of newline separated paragraphs.
            One or more footers MAY be provided one blank line after the body. Each footer MUST consist of a word token, followed by either a :<space> or <space># separator, followed by a string value (this is inspired by the git trailer convention).

            # Token footer token whitespace replacement
            [CC-7]
            A footer’s token MUST use - in place of whitespace characters, e.g., Acked-by (this helps differentiate the footer section from a multi-paragraph body). An exception is made for BREAKING CHANGE, which MAY also be used as a token.

            # Token value may contain spaces	
            [CC-8]
            A footer’s value MAY contain spaces and newlines, and parsing MUST terminate when the next valid footer token/separator pair is observed.
            Breaking changes MUST be indicated in the type/scope prefix of a commit, or as an entry in the footer.

            # Spaces in footer
            [CC-9]
            If included as a footer, a breaking change MUST consist of the uppercase text BREAKING CHANGE, followed by a colon, space, and description, e.g., BREAKING CHANGE: environment variables now take precedence over config files.

            # Breaking changes in prefix
            [CC-10]
            If included in the type/scope prefix, breaking changes MUST be indicated by a ! immediately before the :. If ! is used, BREAKING CHANGE: MAY be omitted from the footer section, and the commit description SHALL be used to describe the breaking change.

            # Type extensions
            [CC-11]
            Types other than feat and fix MAY be used in your commit messages, e.g., docs: update ref docs.

            # Case sensitibity for units of information
            [CC-12]
            The units of information that make up Conventional Commits MUST NOT be treated as case sensitive by implementors, with the exception of BREAKING CHANGE which MUST be uppercase.

            # BREAKING-CHANGE
            [CC-13]
            BREAKING-CHANGE MUST be synonymous with BREAKING CHANGE, when used as a token in a footer.
            """;

        /// <summary>
        /// Verifies that a markdown specification cand be imported and split into requirements and that the
        /// [EXTERNAL-ID] tags are parsed and set as the ExternalId property on the individual requirements
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ExtractRequirementsFromSpecification_WithMarkdown()
        {
            // Arrange
            var specification = await Fixture.Requirements.AddSpecificationAsync();
            specification.Description = mConventionalCommitsMarkdownSpec;
            await Fixture.Requirements.UpdateAsync(specification);

            // Act 
            await Fixture.Requirements.ExtractRequirementsFromSpecificationAsync(specification);

            // Assert
            var requirements = await Fixture.Requirements.GetRequirementsAsync(specification.Id);
            Assert.Equal(13, requirements.TotalCount);

            Assert.NotNull(requirements.Items.FirstOrDefault(x => x.ExternalId == "CC-1"));
            Assert.NotNull(requirements.Items.FirstOrDefault(x => x.ExternalId == "CC-13"));
        }
    }
}
