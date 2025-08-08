using TestBucket.Contracts.Requirements;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Requirements.Import.Strategies;

namespace TestBucket.Domain.IntegrationTests.Requirements
{
    /// <summary>
    /// Tests related to importing requirements
    /// </summary>
    /// <param name="Fixture"></param>
    [IntegrationTest]
    [EnrichedTest]
    [FunctionalTest]
    [Feature("Import Requirements")]
    [Component("Requirements")]
    public class PdfImportRequirementTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that a PDF specification can be converted to markdown when the PDF has numerical headers such as:
        /// ```
        /// 1. Header-1
        /// 
        /// 1.2. Header-2
        /// 
        /// 1.3 Header-3
        /// ```
        /// 
        /// These headers are translated to markdown equivallent:
        /// ```
        /// # Header-1
        /// 
        /// ## Header-2
        /// 
        /// ## Header-3
        /// ```
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ExtractRequirementsFromSpecification_WithPdfWithNumericHeaders_MarkdownChaptersCreated()
        {
            // Arrange
            var pdfImporter = new PdfImporter();
            var fileResource = new FileResource
            {
                ContentType = "application/pdf",
                TenantId = Fixture.App.Tenant,
                Name = "NumericalHeaders.pdf",
                Data = File.ReadAllBytes("TestData/NumericalHeaders.pdf")
            };
            var spec = new RequirementSpecificationDto() { Name = "NumericalHeaders" };

            // Act
            await pdfImporter.ImportAsync(spec, fileResource);

            // Assert
            Assert.NotNull(spec.Description);
            Assert.Contains("# 1. Test Bucket Sample", spec.Description);
            Assert.Contains("## 1.1. Requirement #1", spec.Description);
            Assert.Contains("## 1.2 Requirement #2", spec.Description);

        }
    }
}
