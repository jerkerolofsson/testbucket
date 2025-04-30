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
    [Index(nameof(TenantId), nameof(Slug))]
    [Index(nameof(ExternalId))]
    public class RequirementSpecification : RequirementEntity
    {
        /// <summary>
        /// Database ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// External ID
        /// </summary>
        public string? ExternalId { get; set; }

        /// <summary>
        /// External Provider
        /// </summary>
        public string? ExternalProvider { get; set; }

        /// <summary>
        /// Optional: Original file
        /// </summary>
        public long? FileResourceId { get; set; }

        /// <summary>
        /// Name of the specification
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Slug
        /// </summary>
        public string? Slug { get; set; }

        /// <summary>
        /// Read-only requirements are managed outside
        /// </summary>
        public bool ReadOnly { get; set; }

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
