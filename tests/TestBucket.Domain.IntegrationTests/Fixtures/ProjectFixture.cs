using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Models;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Teams.Models;
using Xunit;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    public class ProjectFixture : IAsyncLifetime
    {
        private readonly TestBucketApp _app;
        private Team _team = new Team { Name = "", Slug="", ShortName = "" };
        private TestProject _project = new TestProject { Name = "", Slug = "", ShortName = "" };

        public long ProjectId => _project.Id;
        public long TeamId => _team.Id;

        public IServiceProvider Services => _app.Services;

        public TestBucketApp App => _app;

        public ProjectFixture(TestBucketApp app)
        {
            _app = app;
        }

        public async ValueTask DisposeAsync()
        {
            using var scope = _app.Services.CreateScope();
            if (_project is not null)
            {
                var projectManager = scope.ServiceProvider.GetRequiredService<IProjectManager>();
                await projectManager.DeleteAsync(_app.SiteAdministrator, _project);
            }
            if (_team is not null)
            {
                var teamManager = scope.ServiceProvider.GetRequiredService<ITeamManager>();
                await teamManager.DeleteAsync(_app.SiteAdministrator, _team);
            }
        }

        public async ValueTask InitializeAsync()
        {
            _team.Name = "fixture:team-" + Guid.NewGuid().ToString();
            _project.Name = "fixture:project-" + Guid.NewGuid().ToString();

            using var scope = _app.Services.CreateScope();
            var projectManager = scope.ServiceProvider.GetRequiredService<IProjectManager>();
            var teamManager = scope.ServiceProvider.GetRequiredService<ITeamManager>();

            await teamManager.AddAsync(_app.SiteAdministrator, _team);

            _project.TeamId = _team.Id;
            await projectManager.AddAsync(_app.SiteAdministrator, _project);
        }
    }
}
