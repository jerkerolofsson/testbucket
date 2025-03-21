using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Identity.Models;

using TestBucket.Domain.Identity;

namespace TestBucket.Domain.Projects;
internal class ProjectManager
{
    private readonly IProjectRepository _projectRepository;

    public ProjectManager(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public Task AddAsync(ClaimsPrincipal principal, TestProject project)
    {
        principal.ThrowIfNotAdmin();
        var tenantId = principal.GetTentantIdOrThrow();

        project.TenantId = tenantId;
        //project.CreatedBy..

        throw new Exception("TODO");
    }
}
