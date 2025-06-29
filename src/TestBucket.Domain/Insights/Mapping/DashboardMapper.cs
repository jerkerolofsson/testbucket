using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Insights;
using TestBucket.Domain.Insights.Model;

namespace TestBucket.Domain.Insights.Mapping;
public static class DashboardMapper
{
    public static DashboardDto ToDto(this Dashboard dashboard)
    {
        return new DashboardDto 
        { 
            Name = dashboard.Name, 
            Id = dashboard.Id, 
            Specifications = dashboard.Specifications?.ToList() ?? new List<InsightsVisualizationSpecification>() 
        };
    }

    public static Dashboard ToDomain(this DashboardDto dashboardDto)
    {
        return new Dashboard 
        { 
            Name = dashboardDto.Name, 
            Id = dashboardDto.Id, 
            Specifications = dashboardDto.Specifications?.ToList() ?? new List<InsightsVisualizationSpecification>() 
        };
    }
}
