using System.Net;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Controllers.Api;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Fields.Mapping;
using TestBucket.Contracts.Fields;

namespace TestBucket.Components.Shared.Fields.Api;

[ApiController]
public class FieldsApiController : ProjectApiControllerBase
{
    private readonly IProjectManager _projectManager;
    private readonly IFieldDefinitionManager _fieldsManager;

    public FieldsApiController(IProjectManager manager, IFieldDefinitionManager fieldsManager)
    {
        _projectManager = manager;
        _fieldsManager = fieldsManager;
    }


    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/fields/projects/{slug}/definitions")]
    [ProducesDefaultResponseType(typeof(List<FieldDefinitionDto>))]
    public async Task<IActionResult> GetProjectFieldsAsync([FromRoute] string slug)
    {
        if (!User.HasPermission(PermissionEntityType.Project, PermissionLevel.Read))
        {
            return Unauthorized();
        }
        if (string.IsNullOrEmpty(slug))
        {
            return BadRequest("Project slug cannot be empty");
        }

        var project = await _projectManager.GetTestProjectBySlugAsync(User, slug);
        if (project is null)
        {
            return NotFound();
        }
        var fieldDefinitions = await _fieldsManager.GetDefinitionsAsync(User, project.Id);

        var dtos = fieldDefinitions.Select(x => x.ToDto(project.Slug));

        return Ok(dtos);
    }
}
