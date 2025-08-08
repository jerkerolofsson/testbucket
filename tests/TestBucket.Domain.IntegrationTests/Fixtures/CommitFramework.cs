using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Models;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    /// <summary>
    /// Test fixture
    /// </summary>
    /// <param name="Fixture"></param>

    public class CommitFramework(ProjectFixture Fixture)
    {
        /// <summary>
        /// Adds a source code repo
        /// </summary>
        /// <returns></returns>
        internal async Task AddRepoAsync(Repository repo)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            repo.TestProjectId = Fixture.ProjectId;
            repo.TeamId = Fixture.TeamId;

            if(repo.ExternalSystemId == default)
            {
                var externalSystem = new ExternalSystem { Name = Guid.NewGuid().ToString() };
                var projectManager = Fixture.Services.GetRequiredService<IProjectManager>();
                var project = await projectManager.GetTestProjectByIdAsync(principal, Fixture.ProjectId);
                if(project is null)
                {
                    throw new Exception("Fixture project not found");
                }
                await projectManager.SaveProjectIntegrationAsync(principal, project.Slug, externalSystem);
                repo.ExternalSystemId = externalSystem.Id;
            }

            var manager = Fixture.Services.GetRequiredService<ICommitManager>();

            await manager.AddRepositoryAsync(principal, repo);
        }
        /// <summary>
        /// Adds a commit
        /// </summary>
        /// <param name="commit"></param>
        /// <returns></returns>
        public async Task AddCommitAsync(Commit commit)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            commit.TestProjectId = Fixture.ProjectId;
            commit.TeamId = Fixture.TeamId;

            var manager = Fixture.Services.GetRequiredService<ICommitManager>();

            await manager.AddCommitAsync(principal, commit);
        }
    }
}
