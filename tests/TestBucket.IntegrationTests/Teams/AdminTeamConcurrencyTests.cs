using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.IntegrationTests.Teams;

[ReliabilityTest]
[EnrichedTest]
[IntegrationTest]
public class AdminTeamConcurrencyTests(TestBucketApp App, ITestOutputHelper testOutputHelper)
{
    private async Task AddDeleteAsync(CancellationToken stoppingToken)
    {
        int count = 0;
        while(!stoppingToken.IsCancellationRequested)
        {
            var slug = await App.Client.Teams.AddAsync(Guid.NewGuid().ToString());
            await App.Client.Teams.DeleteAsync(slug);
            count++;
        }

        testOutputHelper.WriteLine($"Created and deleted {count} teams");
    }

    [Fact]
    [TestDescription("Adds and removes teams in multiple concurrent threads")]
    public async Task AddDeleteTeam_InMultipleThreads()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, TestContext.Current.CancellationToken);

        Task[] tasks = [
            Task.Run(async () => await AddDeleteAsync(linkedCts.Token), linkedCts.Token),
            Task.Run(async () => await AddDeleteAsync(linkedCts.Token), linkedCts.Token),
            Task.Run(async () => await AddDeleteAsync(linkedCts.Token), linkedCts.Token),
            Task.Run(async () => await AddDeleteAsync(linkedCts.Token), linkedCts.Token),
            Task.Run(async () => await AddDeleteAsync(linkedCts.Token), linkedCts.Token),
            ];
        
        await Task.WhenAll(tasks);
    }
}
