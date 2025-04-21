using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Testing.Models
{
    public record class AttributeRequirement
    {
        /// <summary>
        /// Name of the attribute
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Comparison
        /// </summary>
        public required AttributeOperator Operator { get; set; }

        /// <summary>
        /// Valud to compare with attribute
        /// </summary>
        public string? Value { get; set; }
    }
}
