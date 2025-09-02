using Mediator;

using TestBucket.Domain.Projects;
using TestBucket.Domain.Requirements.Import;
using TestBucket.Domain.Teams;
using TestBucket.Formats;
using TestBucket.Formats.Csv;

namespace TestBucket.Domain.Testing.Services.Import;
internal class TestCaseImporter : ITestCaseImporter
{
    private readonly IMediator _mediator;
    private readonly IProjectManager _projectManager;
    private readonly ITeamManager _teamManager;

    public TestCaseImporter(IMediator mediator, IProjectManager projectManager, ITeamManager teamManager)
    {
        _mediator = mediator;
        _projectManager = projectManager;
        _teamManager = teamManager;
    }

    public async Task ImportAsync(ClaimsPrincipal principal, long teamId, long projectId, byte[] bytes, string mediaType, ImportHandlingOptions options)
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

        ITestRepositorySerializer? serializer = null;
        switch (mediaType)
        {
            case "text/csv":
                serializer = new CsvRepoSerializer();
                break;
            case "application/zip":
                serializer = new TestZipImporter();
                break;
        }

        if (serializer is not null) 
        {
            using var source = new MemoryStream(bytes);
            var repo = await serializer.DeserializeAsync(source);
            if (repo is not null)
            {
                await _mediator.Send(new ImportTestsRequest(principal, team, project, repo, options));
            }
        }
        else
        {
            throw new ArgumentException("Unsupported media type " + mediaType);
        }
    }
}
