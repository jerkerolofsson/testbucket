using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Teams.Models;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Domain.Requirements.Models
{
    [Table("spec")]
    [Index(nameof(TenantId), nameof(Created))]
    public class RequirementSpecification
    {
        /// <summary>
        /// Database ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Optional: Original file
        /// </summary>
        public long? FileResourceId { get; set; }

        /// <summary>
        /// Timestamp when the test case was created
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// Name of the test case
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Test case description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// ID of tenant
        /// </summary>
        public required string TenantId { get; set; }

        /// <summary>
        /// ID of project
        /// </summary>
        public long? TeamId { get; set; }

        /// <summary>
        /// ID of project
        /// </summary>
        public long? TestProjectId { get; set; }

        // Customization

        /// <summary>
        /// SVG icon
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// HTML color
        /// </summary>
        public string? Color { get; set; }

        // Navigation
        public Team? Team { get; set; }
        public Tenant? Tenant { get; set; }
        public TestProject? TestProject { get; set; }
    }
}
