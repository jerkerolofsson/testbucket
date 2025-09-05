using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Octokit;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Issues.Models;
using TestBucket.Github.Models;
using TestBucket.Integrations;

namespace TestBucket.Github.Milestones;
internal class GithubMilestoneProvider : GithubIntegrationBaseClient, IExternalMilestoneProvider
{
    public string SystemName => ExtensionConstants.SystemName;

    public async Task<IReadOnlyList<MilestoneDto>> GetMilestonesAsync(ExternalSystemDto system, DateTimeOffset? from, DateTimeOffset until, CancellationToken cancellationToken)
    {
        var ownerProject = GithubOwnerProject.Parse(system.ExternalProjectId);
        var client = CreateClient(system);

        int pageToFetch = 0;

        var results = new List<MilestoneDto>();

        while (!cancellationToken.IsCancellationRequested)
        {
            var options = new ApiOptions
            {
                PageCount = 1,
                PageSize = 100,
                StartPage = pageToFetch
            };
            var request = new MilestoneRequest { State = ItemStateFilter.All };
            var milestones = await client.Issue.Milestone.GetAllForRepository(ownerProject.Owner, ownerProject.Project, request, options);
            foreach (var milestone in milestones)
            {
                if (milestone.UpdatedAt is null || 
                    ((from is null || milestone.UpdatedAt.Value >= from.Value) &&
                    (milestone.UpdatedAt.Value < until)))
                {
                    results.Add(new MilestoneDto 
                    { 
                        Title = milestone.Title, 
                        Description = milestone.Description, 
                        DueDate = milestone.DueOn, 
                        ExternalId = milestone.Number.ToString(), 
                        ExternalSystemId = system.Id, 
                        ExternalSystemName = system.Name, 
                        State = milestone.State == ItemState.Open ? MilestoneState.Open : MilestoneState.Closed });
                }
            }

            pageToFetch++;
            if(milestones.Count == 0)
            {
                break;
            }
        }

        return results;
    }
}
