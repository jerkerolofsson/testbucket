using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Teams.Models;

namespace TestBucket.Domain.Shared
{
    /// <summary>
    /// An entity belong to a project
    /// </summary>
    public class ProjectEntity : Entity
    {
        public long? TeamId { get; set; }
        public long? TestProjectId { get; set; }
        public TestProject? TestProject { get; set; }
        public Team? Team { get; set; }
    }
}
