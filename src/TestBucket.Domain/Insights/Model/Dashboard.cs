using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Insights;

namespace TestBucket.Domain.Insights.Model;
public class Dashboard : ProjectEntity
{
    /// <summary>
    /// ID of the dashboard
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Name of the dashboard
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Specifications for visualization. 
    /// Each item respresents a tile on the dashboard.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<InsightsVisualizationSpecification>? Specifications { get; set; }
}
