using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;

namespace TestBucket.Contracts.Projects;
public record class ProjectDto
{
    public required string Name { get; set;  }
    public required string Slug { get; set; }
    public required ExternalSystemDto[] ExternalSystems { get; set; }
    public required string ShortName { get; set; }
    public TestState[]? TestStates { get; set; }

    /// <summary>
    /// ID of team
    /// </summary>
    public long? TeamId { get; set; }
}
