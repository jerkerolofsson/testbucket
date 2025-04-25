using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts.Teams;

namespace TestBucket.IntegrationTests.Teams
{
    [EnrichedTest]
    [IntegrationTest]
    public class AdminTeamTests(TestBucketApp App)
    {
        [Fact]
        [TestDescription("Verifies that a team can be deleted")]
        public async Task Delete_BySlug_Successful()
        {
            var slug = await App.Client.Teams.AddAsync("Admin Team Number 4");
            Assert.NotEmpty(slug);

            var team = await App.Client.Teams.GetAsync(slug);
            Assert.NotNull(team);

            await App.Client.Teams.DeleteAsync(slug);

            // Should throw 404
            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await App.Client.Teams.GetAsync(slug);
            });
        }

        [Fact]
        [TestDescription("Verifies that a team can be retreived from the slug")]
        public async Task Get_BySlug_Successful()
        {
            var slug = await App.Client.Teams.AddAsync("Admin Team Number 3");
            Assert.NotEmpty(slug);

            var team = await App.Client.Teams.GetAsync(slug);
            Assert.NotNull(team);
            Assert.Equal("Admin Team Number 3", team.Name);

            await App.Client.Teams.DeleteAsync(slug);
        }


        [Fact]
        [TestDescription("Verifies that a newly created team has a slug assigned")]
        public async Task CreateTeam_SlugAssigned()
        {
            var slug = await App.Client.Teams.AddAsync("Admin Team Number 1");
            Assert.NotEmpty(slug);

            await App.Client.Teams.DeleteAsync(slug);
        }

        [Fact]
        [TestDescription("Verifies that a newly created team has a short name assigned")]
        public async Task CreateTeam_ShortNameAssigned()
        {
            var team = new TeamDto { Name = "Admin Team Number 2", ShortName = "", Slug = "" };

            var createdTeam = await App.Client.Teams.AddAsync(team);
            Assert.NotEmpty(createdTeam.ShortName);

            await App.Client.Teams.DeleteAsync(createdTeam.Slug);
        }

    }
}
