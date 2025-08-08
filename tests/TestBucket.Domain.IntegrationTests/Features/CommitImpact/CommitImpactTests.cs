using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Code.Models;

namespace TestBucket.Domain.IntegrationTests.Features.CommitImpact
{
    [IntegrationTest]
    [EnrichedTest]
    [FunctionalTest]
    public class CommitImpactTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that a commit with files that map to a component will be linked by comparing the modified files
        /// for the commit
        /// </summary>
        [Fact]
        public async Task AddCommit_WithModifiedFileAndACorrespondingComponent_ComponentAddedToCommit()
        {
            // Arrange: Add a code repo
            var repo = new Repository { Url = "http://localhost:1234" };
            await Fixture.Commits.AddRepoAsync(repo);

            // Arrange: Define architecture
            var component = new Component { GlobPatterns = ["/src/Data/Area1/**/*.*"], Name = "DataArea1" };
            await Fixture.Architecture.AddComponentAsync(component);

            // Act
            var commit = new Commit 
            { 
                Reference = "123",
                RepositoryId = repo.Id,
                CommitFiles = [new CommitFile { Path = "/src/Data/Area1/test.cs", Sha = "2efcbfc3d0df4157bb3f47338e508dfc771f6654" }],
                Sha = "2efcbfc3d0df4157bb3f47338e508dfc771f6654" + Guid.NewGuid().ToString(),
            };
            await Fixture.Commits.AddCommitAsync(commit);

            // Assert
            Assert.NotNull(commit.ComponentNames);
            Assert.NotEmpty(commit.ComponentNames);
            Assert.Contains("DataArea1", commit.ComponentNames);
        }
    }
}
