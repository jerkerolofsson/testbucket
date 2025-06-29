using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Insights;
public class DashboardDto
{
    public long Id { get; set; }
    public required string Name { get; set; }

    public List<InsightsVisualizationSpecification>? Specifications { get; set; }
}
