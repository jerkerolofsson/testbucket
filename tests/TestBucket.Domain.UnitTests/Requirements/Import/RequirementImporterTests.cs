using Microsoft.Extensions.Logging.Abstractions;
using TestBucket.Contracts.Requirements;
using TestBucket.Domain.Requirements.Import;
using TestBucket.Domain.Requirements.Mapping;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.UnitTests.TestHelpers;

namespace TestBucket.Domain.UnitTests.Requirements.Import
{
    /// <summary>
    /// Contains unit tests for the <see cref="RequirementImporter"/> class, 
    /// verifying the import and extraction of requirements from files.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    [Feature("Backup")]
    [Component("Requirements")]
    public class RequirementImporterTests
    {
        /// <summary>
        /// Verifies that importing a backup file results in a non-empty specification collection.
        /// </summary>
        [Fact]
        public async Task ImportFileAsync_WithBackupFile_SpecificationImported()
        {
            // Arrange
            var doc = FileResourceCreator.CreateText("hello.md", File.ReadAllText($"Requirements/Import/TestData/backup.tbz"));
            var importer = new RequirementImporter(NullLogger<RequirementImporter>.Instance);

            // Act
            var entities = await importer.ImportFileAsync(IdentityHelper.ValidPrincipal, doc);

            // Assert
            Assert.NotEmpty(entities);
        }

        /// <summary>
        /// Verifies that importing an empty text document sets the specification name from the file name.
        /// </summary>
        [Fact]
        public async Task ImportFileAsync_EmptyTextDocument_NameIsFromFileName()
        {
            var doc = FileResourceCreator.CreateText("hello.txt", "Hello World");

            var importer = new RequirementImporter(NullLogger<RequirementImporter>.Instance);

            var entities = await importer.ImportFileAsync(IdentityHelper.ValidPrincipal, doc);
            Assert.Single(entities);

            if (entities[0] is RequirementSpecificationDto requirementSpecification)
            {
                Assert.Equal("hello.txt", requirementSpecification!.Name);
            }
            else
            {
                Assert.Fail("Expected returned entity to be a requirement specification");
            }
        }

        /// <summary>
        /// Verifies that requirement names are extracted from markdown headings in the specification.
        /// </summary>
        [Fact]
        public async Task ExtractRequirementsAsync_WithSections_NamesExtractedFromMarkdownHeadings()
        {
            (RequirementImporter importer, RequirementSpecification requirementSpecification) = await ImportTestDataAsync("sections.md");

            var requirements = await importer.ExtractRequirementsAsync(requirementSpecification, TestContext.Current.CancellationToken);

            Assert.Equal(2, requirements.Count);
            Assert.Equal("1.1 Requirement", requirements[0].Name);
            Assert.Equal("1.2 Requirement", requirements[1].Name);
        }

        /// <summary>
        /// Verifies that the path is extracted from headings when parsing requirements.
        /// </summary>
        [Fact]
        public async Task ExtractRequirementsAsync_WithSections_PathExtractedFromHeadings()
        {
            (RequirementImporter importer, RequirementSpecification requirementSpecification) = await ImportTestDataAsync("sections.md");

            var requirements = await importer.ExtractRequirementsAsync(requirementSpecification, TestContext.Current.CancellationToken);

            Assert.Equal(2, requirements.Count);
            Assert.Equal("1. TITLE", requirements[0].Path);
            Assert.Equal("1. TITLE", requirements[1].Path);
        }

        /// <summary>
        /// Verifies that requirement IDs in brackets are extracted as external IDs.
        /// </summary>
        [Fact]
        public async Task ExtractRequirementsAsync_WithRequirementIdInBrackes_ExtractedAsExternalId()
        {
            (RequirementImporter importer, RequirementSpecification requirementSpecification) = await ImportTestDataAsync("sections.md");

            var requirements = await importer.ExtractRequirementsAsync(requirementSpecification, TestContext.Current.CancellationToken);

            Assert.Equal(2, requirements.Count);
            Assert.Equal("REQ-1.1", requirements[0].ExternalId);
            Assert.Equal("REQ-1.2", requirements[1].ExternalId);
        }

        /// <summary>
        /// Imports test data and returns a tuple containing the importer and the imported requirement specification.
        /// </summary>
        /// <param name="name">The name of the test data file to import.</param>
        /// <returns>
        /// A tuple containing the <see cref="RequirementImporter"/> and the imported <see cref="RequirementSpecification"/>.
        /// </returns>
        private static async Task<(RequirementImporter importer, RequirementSpecification requirementSpecification)> ImportTestDataAsync(string name)
        {
            var doc = FileResourceCreator.CreateText("hello.md", File.ReadAllText($"Requirements/Import/TestData/{name}"));

            var importer = new RequirementImporter(NullLogger<RequirementImporter>.Instance);

            var entities = await importer.ImportFileAsync(IdentityHelper.ValidPrincipal, doc);

            Assert.Single(entities);
            var requirementSpecification = entities[0] as RequirementSpecificationDto;
            Assert.NotNull(requirementSpecification);
            return (importer, requirementSpecification.ToDbo());
        }
    }
}