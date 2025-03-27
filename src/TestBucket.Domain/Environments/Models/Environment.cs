using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Environments.Models
{
    /// <summary>
    /// An environment contains information about the system, expressed through variables
    /// </summary>
    public class Environment : ProjectEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Name of the environment
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Markdown
        /// </summary>
        public string? Description { get; set; }
    }
}
