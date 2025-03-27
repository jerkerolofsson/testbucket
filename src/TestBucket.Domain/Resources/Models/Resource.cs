using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Resources.Models
{
    public class Resource
    {
        public long Id { get; set; }

        /// <summary>
        /// Name of the environment
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Kind of resource
        /// </summary>
        public ResourceKind Kind { get; set; } = ResourceKind.Other;
    }
}
