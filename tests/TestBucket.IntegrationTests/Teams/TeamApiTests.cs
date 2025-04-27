using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts.Teams;

namespace TestBucket.IntegrationTests.Teams
{
    [FunctionalTest]
    [EnrichedTest]
    [ApiTest]
    public class TeamApiTests(TestBucketApp App)
    {
        [Fact]
        [TestDescription("Verifies that 404 is returned if trying to delete a team that doesn't exist")]
        public async Task Get_WithInvalidSlug_404NotFound()
        {
            // Should throw 404
            var requestException = await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await App.Client.Teams.GetAsync(Guid.NewGuid().ToString());
            });
            Assert.Equal(HttpStatusCode.NotFound, requestException.StatusCode);
        }

        [Fact]
        [TestDescription("Verifies that 404 is returned if trying to delete a team that doesn't exist")]
        public async Task Delete_WithInvalidSlug_404NotFound()
        {
            // Should throw 404
            var requestException = await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await App.Client.Teams.DeleteAsync(Guid.NewGuid().ToString());
            });
            Assert.Equal(HttpStatusCode.NotFound, requestException.StatusCode);
        }

    }
}
