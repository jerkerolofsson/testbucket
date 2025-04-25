using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Controllers.Api;
using TestBucket.Domain.ApiKeys.Validation;
using TestBucket.Domain.Projects;
using TestBucket.Formats;

namespace TestBucket.Components.Tests.TestCases.Api;

[ApiController]
public class ResultsApiController : ProjectApiControllerBase
{
    private readonly ITextTestResultsImporter _textTestResultsImporter;
    private readonly IProjectManager _projectManager;

    public ResultsApiController(ITextTestResultsImporter textTestResultsImporter, IProjectManager projectManager)
    {
        _textTestResultsImporter = textTestResultsImporter;
        _projectManager = projectManager;
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpPut("/api/results")]
    [HttpPost("/api/results")]
    public async Task<IActionResult> ImportResultsAsync()
    {
        var validator = new PrincipalValidator(User);

        var tenantId = validator.GetTenantId();
        var projectId = validator.GetProjectId();
        var runId = validator.GetTestRunId();
        var testSuiteId = validator.GetTestSuiteId();
        if (projectId is null)
        {
            return Unauthorized("Access token missing project ID");
        }
        if (runId is null )
        {
            return Unauthorized("Access token missing run ID");
        }
        if (tenantId is null)
        {
            return Unauthorized("Access token missing tenant ID");
        }
        if (testSuiteId is null)
        {
            return Unauthorized("Access token missing testsuite ID");
        }
        TestProject? project = await _projectManager.GetTestProjectByIdAsync(User, projectId.Value);
        if(project is null || project.TeamId is null)
        {
            return NotFound("Project not found");
        }

        var options = new ImportHandlingOptions
        {
            TestRunId = runId,
            TestSuiteId = testSuiteId,
        };

        var text = await ReadRequestBodyAsync();

        var format = TestResultFormat.xUnitXml;
        TestResultSerializerFactory.GetFormatFromContentType(Request.ContentType);

        if(string.IsNullOrEmpty(text))
        {
            return BadRequest("No body");
        }
        await _textTestResultsImporter.ImportTextAsync(User, project.TeamId.Value, projectId.Value, format, text, options);

        return Ok();
    }
}
