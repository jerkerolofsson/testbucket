using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts.Teams;

namespace TestBucket.IntegrationTests.TestProjects
{
    [FunctionalTest]
    [EnrichedTest]
    [IntegrationTest]
    public class AdminProjectTests(TestBucketApp App)
    {
        [Fact]
        [TestDescription("Verifies that a newly created project has a short name and slug assigned")]
        public async Task CreateProject_ShortNameAssigned()
        {
            var team = await App.Client.Teams.AddAsync("team " + Guid.NewGuid().ToString());
            try
            {
                var slug = await App.Client.Projects.AddAsync(team, "My project " + Guid.NewGuid().ToString());

                var project = await App.Client.Projects.GetAsync(slug);
                Assert.NotNull(project.ShortName);
                Assert.NotNull(project.Slug);

                await App.Client.Projects.DeleteAsync(slug);

            }
            finally
            {
                await App.Client.Teams.DeleteAsync(team);
            }
        }

        [Fact]
        [TestDescription("Verifies that a newly created project has a team slug")]
        public async Task CreateProject_TeamSlugAssigned()
        {
            var team = await App.Client.Teams.AddAsync("team " + Guid.NewGuid().ToString());
            try
            {
                var slug = await App.Client.Projects.AddAsync(team, "My project " + Guid.NewGuid().ToString());

                var project = await App.Client.Projects.GetAsync(slug);
                Assert.NotNull(project.Team);
                Assert.Equal(team, project.Team);

                await App.Client.Projects.DeleteAsync(slug);

            }
            finally
            {
                await App.Client.Teams.DeleteAsync(team);
            }
        }
    }
}
