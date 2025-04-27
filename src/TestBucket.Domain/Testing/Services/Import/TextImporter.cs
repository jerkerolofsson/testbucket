using System.Text;

using Mediator;

using TestBucket.Domain.Projects;
using TestBucket.Domain.Teams;
using TestBucket.Formats;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Testing.Services.Import;

internal class TextImporter : ITextTestResultsImporter
{
    private readonly IMediator _mediator;
    private readonly IProjectManager _projectManager;
    private readonly ITeamManager _teamManager;

    public TextImporter(IMediator mediator, IProjectManager projectManager, ITeamManager teamManager)
    {
        _mediator = mediator;
        _projectManager = projectManager;
        _teamManager = teamManager;
    }

    /// <summary>
    /// Imports a text based test result document
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="teamId"></param>
    /// <param name="projectId"></param>
    /// <param name="format"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task ImportTextAsync(ClaimsPrincipal principal, long teamId, long projectId, TestResultFormat format, string text, ImportHandlingOptions options)
    {
        if(format == TestResultFormat.UnknownFormat)
        {
            format = TestResultDetector.Detect(Encoding.UTF8.GetBytes(text));
        }

        var serializer = TestResultSerializerFactory.Create(format);
        var run = serializer.Deserialize(text);


        await ImportRunAsync(principal, teamId, projectId, run, options);
    }


    public async Task ImportRunAsync(ClaimsPrincipal principal, long teamId, long projectId, TestRunDto run, ImportHandlingOptions options)
    {

        var team = await _teamManager.GetTeamByIdAsync(principal, teamId);
        var project = await _projectManager.GetTestProjectByIdAsync(principal, projectId);

        if (project is null)
        {
            throw new ArgumentException("Project not found");
        }
        if (team is null)
        {
            throw new ArgumentException("Team not found");
        }

        run.Team = team.Slug;
        run.Project = project.Slug;

        await _mediator.Send(new ImportRunRequest(principal, run, options));
    }
}
