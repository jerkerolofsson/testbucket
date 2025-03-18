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
    public class RequirementSpecification : ProjectEntity
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
        /// Created by user name
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Timestamp when the entity was created
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// Modified by user name
        /// </summary>
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Timestamp when the entity was Modified
        /// </summary>
        public DateTimeOffset Modified { get; set; }

        /// <summary>
        /// Name of the test case
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Test case description
        /// </summary>
        public string? Description { get; set; }

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
    }
}
