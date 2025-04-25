using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Projects;

namespace TestBucket.Domain.Projects.Mapping;
public static class ProjectMapper
{
    public static ProjectDto ToDto(this TestProject project)
    {
        var dto = new ProjectDto
        {
            Name = project.Name,
            Slug = project.Slug,
            ShortName = project.ShortName,
            TestStates = project.TestStates?.ToArray(),
            ExternalSystems = []
        };

        if (project.ExternalSystems is not null)
        {
            dto.ExternalSystems = project.ExternalSystems.Select(x => x.ToDto()).ToArray();
        }

        return dto;
    }
}
