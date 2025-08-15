using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModelContextProtocol.Server;

using TestBucket.Contracts.Issues.Models;
using TestBucket.Domain.AI.Mcp.Tools;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.Milestones.Mcp;

[McpServerToolType]
[DisplayName("milestones")]
public class MilstonesMcpTools : AuthenticatedTool
{
    private readonly IMilestoneManager _manager;

    public MilstonesMcpTools(IMilestoneManager manager, IApiKeyAuthenticator apiKeyAuthenticator) : base(apiKeyAuthenticator)
    {
        _manager = manager;
    }

    public class MilestoneMcpDto
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public long Id { get; set; }

        /// <summary>
        /// True if this is the next milestone
        /// </summary>
        public bool IsNext { get; set; }

        /// <summary>
        /// True if the milestone is open, i.e. not completed
        /// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        /// True if the due date is in the past
        /// </summary>
        public bool IsOverdue { get; set; }
    }


    /// <summary>
    /// Returns the latest test results
    /// </summary>
    /// <returns></returns>
    [McpServerTool(Name = "list-milestones"), Description("Returns a list of all project milestones")]
    public async Task<MilestoneMcpDto[]> GetMilestonesAsync()
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var projectId = _principal.GetProjectId();
            if (projectId is not null)
            {
                var result = await _manager.GetMilestonesAsync(_principal, projectId.Value);
                var dtos = result.Where(x=>x.Title != null).Select(x => new MilestoneMcpDto
                {
                    Title = x.Title ?? "",
                    Description = x.Description,
                    DueDate = x.EndDate,
                    Id = x.Id,
                    IsOpen = x.State == MilestoneState.Open,
                }).ToArray();

                var next = dtos.Where(x => x.DueDate > DateTime.UtcNow).FirstOrDefault();
                if (next is not null)
                {
                    next.IsNext = true;
                }
                foreach(var dto in dtos.Where(x => x.DueDate < DateTime.UtcNow && x.IsOpen))
                {
                    dto.IsOverdue = true;
                }

                return dtos;
            }

        }
        return [];
    }
}
