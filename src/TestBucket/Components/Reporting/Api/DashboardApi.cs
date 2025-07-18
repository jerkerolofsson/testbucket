using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Contracts.Insights;
using TestBucket.Contracts.Projects;
using TestBucket.Controllers.Api;
using TestBucket.Domain.Insights;
using TestBucket.Domain.Insights.Mapping;

namespace TestBucket.Components.Reporting.Api;

[ApiController]
public class DashboardApi : ProjectApiControllerBase
{
    private readonly IDashboardManager _dashboardManager;

    public DashboardApi(IDashboardManager dashboardManager)
    {
        _dashboardManager = dashboardManager;
    }

    [Authorize]
    [HttpPut("/api/dashboards")]
    [HttpPost("/api/dashboards")]
    [ProducesDefaultResponseType(typeof(DashboardDto))]
    public async Task<IActionResult> AddAsync([FromBody] DashboardDto dashboard)
    {
        var domainDashboard = dashboard.ToDomain();
        await _dashboardManager.AddDashboardAsync(User, domainDashboard);
        return Ok(dashboard);
    }

    [Authorize]
    [HttpDelete("/api/dashboards/{id:long}")]
    [ProducesDefaultResponseType(typeof(DashboardDto))]
    public async Task DeleteAsync([FromRoute] long id)
    {
        await _dashboardManager.DeleteDashboardAsync(User, id);
    }
}
