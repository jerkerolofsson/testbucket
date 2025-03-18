
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
        public async Task Import_EmptyTextDocument_NameIsFromFileName()
        {
            var doc = FileResourceCreator.CreateText("hello.txt", "Hello World");

            var importer = new RequirementImporter(NullLogger<RequirementImporter>.Instance);

            var requirementSpecification = await importer.ImportAsync(IdentityHelper.ValidPrincipal, null, null, doc);
            await Assert.That(requirementSpecification).IsNotNull();

            await Assert.That(requirementSpecification!.Name).IsEqualTo("hello.txt");
        }
    }
}
