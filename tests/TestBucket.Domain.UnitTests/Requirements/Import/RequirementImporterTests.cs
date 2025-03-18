
using Microsoft.Extensions.Logging.Abstractions;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Requirements.Import;
using TestBucket.Domain.UnitTests.TestHelpers;

namespace TestBucket.Domain.UnitTests.Requirements.Import
{
    [UnitTest]
    public class RequirementImporterTests
    {
        [Test]
        public async Task ImportFileAsync_EmptyTextDocument_NameIsFromFileName()
        {
            var doc = FileResourceCreator.CreateText("hello.txt", "Hello World");

            var importer = new RequirementImporter(NullLogger<RequirementImporter>.Instance);

            var requirementSpecification = await importer.ImportFileAsync(IdentityHelper.ValidPrincipal, null, null, doc);
            await Assert.That(requirementSpecification).IsNotNull();

            await Assert.That(requirementSpecification!.Name).IsEqualTo("hello.txt");
        }

        [Test]
        public async Task ExtractRequirementsAsync_WithSections_NamesExtractedFromMarkdownHeadings()
        {
            (RequirementImporter importer, Domain.Requirements.Models.RequirementSpecification? requirementSpecification) = await ImportTestDataAsync("sections.md");

            var requirements = await importer.ExtractRequirementsAsync(requirementSpecification!, default);
            await Assert.That(requirements.Count).IsEqualTo(2);

            await Assert.That(requirements[0].Name).IsEqualTo("1.1 Requirement");
            await Assert.That(requirements[1].Name).IsEqualTo("1.2 Requirement");
        }


        [Test]
        public async Task ExtractRequirementsAsync_WithSections_PathExtractedFromHeadings()
        {
            (RequirementImporter importer, Domain.Requirements.Models.RequirementSpecification? requirementSpecification) = await ImportTestDataAsync("sections.md");

            var requirements = await importer.ExtractRequirementsAsync(requirementSpecification!, default);
            await Assert.That(requirements.Count).IsEqualTo(2);

            await Assert.That(requirements[0].Path).IsEqualTo("1. TITLE");
            await Assert.That(requirements[1].Path).IsEqualTo("1. TITLE");
        }

        [Test]
        public async Task ExtractRequirementsAsync_WithRequirementIdInBrackes_ExtractedAsExternalId()
        {
            (RequirementImporter importer, Domain.Requirements.Models.RequirementSpecification? requirementSpecification) = await ImportTestDataAsync("sections.md");

            var requirements = await importer.ExtractRequirementsAsync(requirementSpecification!, default);
            await Assert.That(requirements.Count).IsEqualTo(2);

            await Assert.That(requirements[0].ExternalId).IsEqualTo("REQ-1.1");
            await Assert.That(requirements[1].ExternalId).IsEqualTo("REQ-1.2");
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
            await Assert.That(requirementSpecification).IsNotNull();
            return (importer, requirementSpecification);
        }
    }
}
