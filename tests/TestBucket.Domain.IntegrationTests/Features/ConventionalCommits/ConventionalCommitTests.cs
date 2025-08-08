using TestBucket.Domain.Code.Models;

namespace TestBucket.Domain.IntegrationTests.Features.ConventionalCommits
{
    /// <summary>
    /// Tests related to conventional commits
    /// </summary>
    /// <param name="Fixture"></param>
    [IntegrationTest]
    [EnrichedTest]
    [FunctionalTest]
    [Feature("Conventional Commits")]
    public class ConventionalCommitTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that a commit with a conventional commit with a 'feat' type adds the feature as a reference if the 
        /// feature declares the feature name as scope
        /// 
        /// Commit message:
        /// ```
        /// feat(Feature Name): Some work done on the feature
        /// ```
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddCommit_WithFeatureType_FeatureScopeAddedAsFeatureName()
        {
            var repo = new Repository { Url = "http://localhost:1234" };
            await Fixture.Commits.AddRepoAsync(repo);
            var commit = new Commit
            {
                RepositoryId = repo.Id,
                Reference = "123",
                Sha = "2efcbfc3d0df4157bb3f47338e508dfc771f6654" + Guid.NewGuid().ToString(),
                Message = """
                feat(Feature Name): Some work done on the feature
                """
            };

            // Act
            await Fixture.Commits.AddCommitAsync(commit);

            // Assert
            Assert.NotNull(commit.FeatureNames);
            Assert.NotEmpty(commit.FeatureNames);
            Assert.Contains("Feature Name", commit.FeatureNames);
        }

        /// <summary>
        /// Verifies that a commit with a conventional commit with a 'feat' type in upper case adds the feature as a reference if the
        /// </summary>
        /// <returns></returns>
        [Fact]
        [CoveredRequirement("CC-12")]
        public async Task AddCommit_WithFeatureTypeInUpperCase_FeatureScopeAddedAsFeatureName()
        {
            var repo = new Repository { Url = "http://localhost:1234" };
            await Fixture.Commits.AddRepoAsync(repo);
            var commit = new Commit
            {
                RepositoryId = repo.Id,
                Reference = "123",
                Sha = "2efcbfc3d0df4157bb3f47338e508dfc771f6654" + Guid.NewGuid().ToString(),
                Message = """
                FEAT(Feature Name): Some work done on the feature
                """
            };

            // Act
            await Fixture.Commits.AddCommitAsync(commit);

            // Assert
            Assert.NotNull(commit.FeatureNames);
            Assert.NotEmpty(commit.FeatureNames);
            Assert.Contains("Feature Name", commit.FeatureNames);
        }

        /// <summary>
        /// Verifies that a commit with a conventional commit with a Fixes git-trailer registers the fix description
        /// for the commit
        /// 
        /// Commit message:
        /// ```
        /// fix: Some fix done
        /// 
        /// Fixes #TB-1
        /// ```
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddCommit_WithIssueFixesGitTrailer_LinkCreated()
        {
            var repo = new Repository { Url = "http://localhost:1234" };
            await Fixture.Commits.AddRepoAsync(repo);
            var commit = new Commit
            {
                RepositoryId = repo.Id,
                Reference = "123", 
                Sha = "2efcbfc3d0df4157bb3f47338e508dfc771f6654" + Guid.NewGuid().ToString(), 
                Message = """
                fix: Some fix done
                
                Fixes #TB-1
                """
            };

            // Act
            await Fixture.Commits.AddCommitAsync(commit);

            // Assert
            Assert.NotNull(commit.Fixes);
            Assert.NotEmpty(commit.Fixes);
            Assert.Contains("#TB-1", commit.Fixes);
        }


        /// <summary>
        /// Verifies that a commit with a conventional commit with a Fixes footer registers the fix description
        /// for the commit
        /// 
        /// Commit message:
        /// ```
        /// fix: Some fix done
        /// 
        /// fixes: #TB-1
        /// ```
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddCommit_WithIssueFixFooter_LinkCreated()
        {
            var repo = new Repository { Url = "http://localhost:1234" };
            await Fixture.Commits.AddRepoAsync(repo);
            var commit = new Commit
            {
                RepositoryId = repo.Id,
                Reference = "123",
                Sha = "2efcbfc3d0df4157bb3f47338e508dfc771f6654" + Guid.NewGuid().ToString(),
                Message = """
                fix: Some fix done
                
                fixes: #TB-1
                """
            };

            // Act
            await Fixture.Commits.AddCommitAsync(commit);

            // Assert
            Assert.NotNull(commit.Fixes);
            Assert.NotEmpty(commit.Fixes);
            Assert.Contains("#TB-1", commit.Fixes);
        }
    }
}
