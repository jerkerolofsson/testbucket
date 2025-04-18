using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Requirements.Import;
using TestBucket.Domain.UnitTests.TestHelpers;

namespace TestBucket.Domain.UnitTests.Runners
{
    [UnitTest]
    [EnrichedTest]
    public class RunnerTokenTests
    {
        [Fact]
        public async Task GenerateAccessToken_WithRunnerScope_HasCorrectScopeProjectAndTenant()
        {
            var principal = Impersonation.Impersonate("abc");

            var doc = FileResourceCreator.CreateText("hello.txt", "Hello World");

            var importer = new RequirementImporter(NullLogger<RequirementImporter>.Instance);

            var requirementSpecification = await importer.ImportFileAsync(IdentityHelper.ValidPrincipal, null, null, doc);

            Assert.NotNull(requirementSpecification);
            Assert.Equal("hello.txt", requirementSpecification!.Name);
        }
    }
}
