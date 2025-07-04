﻿using System.Security.Claims;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Fields.Models;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Models;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Teams.Models;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    public class ProjectFixture : IAsyncLifetime
    {
        private readonly TestBucketApp _app;
        private Team _team = new Team { Name = "", Slug="", ShortName = "" };
        private TestProject _project = new TestProject { Name = "", Slug = "", ShortName = "" };
        public TestProject Project => _project;
        public long ProjectId => _project.Id;
        public long TeamId => _team.Id;

        public IServiceProvider Services => _app.Services;

        /// <summary>
        /// Product architecture
        /// </summary>
        public ArchitectureFramework Architecture => new ArchitectureFramework(this);

        /// <summary>
        /// Labels
        /// </summary>
        public LabelsTestFramework Labels => new LabelsTestFramework(this);

        /// <summary>
        /// Commits
        /// </summary>
        public CommitFramework Commits => new CommitFramework(this);
        
        /// <summary>
        /// Heuristics
        /// </summary>
        public HeuristicsFramework Heuristics => new HeuristicsFramework(this);

        /// <summary>
        /// Modify user preferences
        /// </summary>
        public UserPreferencesFramework UserPreferences => new UserPreferencesFramework(this);

        /// <summary>
        /// Test framework for test suites/cases
        /// </summary>
        public TestRepoFramework Tests => new TestRepoFramework(this);

        /// <summary>
        /// Test framework for test runs
        /// </summary>
        public TestRunFramework Runs => new TestRunFramework(this);

        /// <summary>
        /// Test framework for test resources
        /// </summary>
        public TestResourcesFramework Resources => new TestResourcesFramework(this);

        /// <summary>
        /// Test framework for test accounts
        /// </summary>
        public TestAccountsTestFramework Accounts => new TestAccountsTestFramework(this);

        /// <summary>
        /// Test framework for requirements
        /// </summary>
        public RequirementsTestFramework Requirements => new RequirementsTestFramework(this);

        /// <summary>
        /// Framework for testing settings
        /// </summary>
        public SettingsTestFramework Settings => new SettingsTestFramework(this);

        /// <summary>
        /// Test framework for issues
        /// </summary>
        public IssuesTestFramework Issues => new IssuesTestFramework(this);

        public TestBucketApp App => _app;

        public ProjectFixture(TestBucketApp app)
        {
            _app = app;
        }

        public async Task<TestProject> CreateProjectAsync()
        {
            var name = "Project-" + Guid.NewGuid().ToString();
            return await CreateProjectAsync(name);
        }

        public async Task<TestProject> CreateProjectAsync(string name)
        {
            var user = Impersonation.Impersonate(App.Tenant);
            var project = new TestProject { Name = name, ShortName = "", Slug = "" };

            var projectManager = Services.GetRequiredService<IProjectManager>();
            await projectManager.AddAsync(user, project);
            return project;
        }

        /// <summary>
        /// Adds an integration
        /// </summary>
        /// <param name="system"></param>
        /// <returns></returns>
        public async Task AddIntegrationAsync(ExternalSystem system)
        {
            var user = Impersonation.Impersonate(App.Tenant);
            var projectManager = Services.GetRequiredService<IProjectManager>();
            await projectManager.SaveProjectIntegrationAsync(user, Project.Slug, system);
        }

        public async Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsByIdAsync()
        {
            var user = Impersonation.Impersonate(App.Tenant);
            var projectManager = Services.GetRequiredService<IProjectManager>();
            return await projectManager.GetProjectIntegrationsAsync(user, Project.Id);
        }
        public async Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsBySlugAsync()
        {
            var user = Impersonation.Impersonate(App.Tenant);
            var projectManager = Services.GetRequiredService<IProjectManager>();
            return await projectManager.GetProjectIntegrationsAsync(user, Project.Slug);
        }


        /// <summary>
        /// Deletes an integration
        /// </summary>
        /// <param name="system"></param>
        /// <returns></returns>
        public async Task DeleteIntegrationAsync(ExternalSystem system)
        {
            var user = Impersonation.Impersonate(App.Tenant);
            var projectManager = Services.GetRequiredService<IProjectManager>();
            await projectManager.DeleteProjectIntegrationAsync(user, system.Id);
        }

        public async Task<FieldDefinition> GetMilestoneFieldAsync()
        {
            var fieldDefinitionManager = Services.GetRequiredService<IFieldDefinitionManager>();
            var fieldDefinitions = await fieldDefinitionManager.GetDefinitionsAsync(App.SiteAdministrator, this.ProjectId);
            return fieldDefinitions.Where(x => x.TraitType == TraitType.Milestone).First();
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

        internal async Task DeleteProjectAsync(TestProject project)
        {
            var user = Impersonation.Impersonate(App.Tenant);
            var projectManager = Services.GetRequiredService<IProjectManager>();
            await projectManager.DeleteAsync(user, project);
        }
    }
}
