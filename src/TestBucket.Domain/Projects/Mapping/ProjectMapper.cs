using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Projects;

namespace TestBucket.Domain.Projects.Mapping;
public static class ProjectMapper
{
    public static TestProject ToDbo(this ProjectDto project)
    {
        var dto = new TestProject
        {
            Id= project.Id,
            Name = project.Name,
            Slug = project.Slug,
            ShortName = project.ShortName,
            TestStates = project.TestStates?.ToArray(),
            ExternalSystems = []
        };

        if (project.ExternalSystems is not null)
        {
            dto.ExternalSystems = project.ExternalSystems.Select(x => x.ToDbo()).ToArray();
        }

        return dto;
    }

    public static ProjectDto ToDto(this TestProject project, bool includeSensitiveDetails)
    {
        var dto = new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Slug = project.Slug,
            ShortName = project.ShortName,
            TestStates = project.TestStates?.ToArray(),
            ExternalSystems = [],
            Team = project.Team?.Slug
        };

        if (project.ExternalSystems is not null)
        {
            dto.ExternalSystems = project.ExternalSystems.Select(x => x.ToDto()).ToArray();

            if (!includeSensitiveDetails)
            {
                foreach (var externalSystem in dto.ExternalSystems)
                {
                    externalSystem.AccessToken = null;
                    externalSystem.ApiKey = null;
                }
            }
        }

        return dto;
    }
}
