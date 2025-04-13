using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;

namespace TestBucket.Contracts.Projects;
public record class ProjectDto
{
    public required string Name { get; init;  }
    public required string Slug { get; init; }
    public required ExternalSystemDto[] ExternalSystems { get; init; }
}
