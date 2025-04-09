
using Microsoft.Extensions.Logging.Abstractions;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Requirements.Import;
using TestBucket.Domain.UnitTests.TestHelpers;

namespace TestBucket.Domain.UnitTests.Requirements.Import
{
    [UnitTest]
    [EnrichedTest]
    public class RequirementImporterTests
    {
        [Fact]
        public async Task ImportFileAsync_EmptyTextDocument_NameIsFromFileName()
        {
            var doc = FileResourceCreator.CreateText("hello.txt", "Hello World");

            var importer = new RequirementImporter(NullLogger<RequirementImporter>.Instance);

            var requirementSpecification = await importer.ImportFileAsync(IdentityHelper.ValidPrincipal, null, null, doc);

            Assert.NotNull(requirementSpecification);
            Assert.Equal("hello.txt", requirementSpecification!.Name);
        }

        [Fact]
        public async Task ExtractRequirementsAsync_WithSections_NamesExtractedFromMarkdownHeadings()
        {
            (RequirementImporter importer, Domain.Requirements.Models.RequirementSpecification? requirementSpecification) = await ImportTestDataAsync("sections.md");

            var requirements = await importer.ExtractRequirementsAsync(requirementSpecification!, TestContext.Current.CancellationToken);

            Assert.Equal(2, requirements.Count);
            Assert.Equal("1.1 Requirement", requirements[0].Name);
            Assert.Equal("1.2 Requirement", requirements[1].Name);
        }

        [Fact]
        public async Task ExtractRequirementsAsync_WithSections_PathExtractedFromHeadings()
        {
            (RequirementImporter importer, Domain.Requirements.Models.RequirementSpecification? requirementSpecification) = await ImportTestDataAsync("sections.md");

            var requirements = await importer.ExtractRequirementsAsync(requirementSpecification!, TestContext.Current.CancellationToken);

            Assert.Equal(2, requirements.Count);
            Assert.Equal("1. TITLE", requirements[0].Path);
            Assert.Equal("1. TITLE", requirements[1].Path);
        }

        [Fact]
        public async Task ExtractRequirementsAsync_WithRequirementIdInBrackes_ExtractedAsExternalId()
        {
            (RequirementImporter importer, Domain.Requirements.Models.RequirementSpecification? requirementSpecification) = await ImportTestDataAsync("sections.md");

            var requirements = await importer.ExtractRequirementsAsync(requirementSpecification!, TestContext.Current.CancellationToken);

            Assert.Equal(2, requirements.Count);
            Assert.Equal("REQ-1.1", requirements[0].ExternalId);
            Assert.Equal("REQ-1.2", requirements[1].ExternalId);
        }

        /// <summary>
        /// Helper 
        /// </summary>
        /// <returns></returns>
        private static async Task<(RequirementImporter importer, Domain.Requirements.Models.RequirementSpecification? requirementSpecification)> ImportTestDataAsync(string name)
        {
            var doc = FileResourceCreator.CreateText("hello.md", File.ReadAllText($"Requirements/Import/TestData/{name}"));

            var importer = new RequirementImporter(NullLogger<RequirementImporter>.Instance);

            var requirementSpecification = await importer.ImportFileAsync(IdentityHelper.ValidPrincipal, null, null, doc);

            Assert.NotNull(requirementSpecification);

            return (importer, requirementSpecification);
        }
    }
}
