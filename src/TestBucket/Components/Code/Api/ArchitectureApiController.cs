using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Contracts.Projects;
using TestBucket.Controllers.Api;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Code.Yaml;
using TestBucket.Domain.Projects;
using TestBucket.Formats.Dtos;

namespace TestBucket.Components.Code.Api;

[ApiController]
public class ArchitectureApiController : ProjectApiControllerBase
{
    private readonly IProjectManager _projectManager;
    private readonly IArchitectureManager _architectureManager;

    public ArchitectureApiController(IArchitectureManager architectureManager, IProjectManager projectManager)
    {
        _architectureManager = architectureManager;
        _projectManager = projectManager;
    }


    /// <summary>
    /// Gets a project architecture model
    /// </summary>
    /// <param name="projectSlug"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/architecture/projects/{projectSlug}/model")]
    public async Task<IActionResult> GetProductArchitectureAsync(
        [FromRoute] string projectSlug)
    {
        var project = await _projectManager.GetTestProjectBySlugAsync(User, projectSlug);
        if (project is null)
        {
            return NotFound();
        }

        var model = await _architectureManager.GetProductArchitectureAsync(User, project);
        return Ok(model);
    }

    /// <summary>
    /// Imports a project architecture model
    /// </summary>
    /// <param name="projectSlug"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [Authorize("ApiKeyOrBearer")]
    [HttpPut("/api/architecture/projects/{projectSlug}/model")]
    public async Task<IActionResult> ImportProductArchitectureAsync(
        [FromRoute]string projectSlug,
        [FromBody] ProjectArchitectureModel model)
    {
        var project = await _projectManager.GetTestProjectBySlugAsync(User, projectSlug);
        if(project is null)
        {
            return NotFound();
        }

        await _architectureManager.ImportProductArchitectureAsync(User, project, model);

        return Ok();
    }
}
