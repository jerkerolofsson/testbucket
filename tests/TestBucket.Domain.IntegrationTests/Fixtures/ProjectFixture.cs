using System.Security.Claims;
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
    /// <summary>
    /// Provides a fixture for managing projects and related entities in integration tests.
    /// </summary>
    public class ProjectFixture : IAsyncLifetime
    {
        private readonly TestBucketApp _app;
        private Team _team = new Team { Name = "", Slug="", ShortName = "" };
        private TestProject _project = new TestProject { Name = "", Slug = "", ShortName = "" };

        /// <summary>
        /// Gets the current test project.
        /// </summary>
        public TestProject Project => _project;

        /// <summary>
        /// Gets the ID of the current test project.
        /// </summary>
        public long ProjectId => _project.Id;

        /// <summary>
        /// Gets the ID of the current test team.
        /// </summary>
        public long TeamId => _team.Id;

        /// <summary>
        /// Gets the service provider for dependency injection.
        /// </summary>
        public IServiceProvider Services => _app.Services;

        /// <summary>
        /// Gets the architecture framework for testing product architecture.
        /// </summary>
        public ArchitectureFramework Architecture => new ArchitectureFramework(this);

        /// <summary>
        /// Gets the labels framework for testing labels.
        /// </summary>
        public LabelsTestFramework Labels => new LabelsTestFramework(this);

        /// <summary>
        /// Gets the commit framework for testing commits.
        /// </summary>
        public CommitFramework Commits => new CommitFramework(this);
        
        /// <summary>
        /// Gets the heuristics framework for testing heuristics.
        /// </summary>
        public HeuristicsFramework Heuristics => new HeuristicsFramework(this);

        /// <summary>
        /// Gets the user preferences framework for modifying user preferences.
        /// </summary>
        public UserPreferencesFramework UserPreferences => new UserPreferencesFramework(this);

        /// <summary>
        /// Gets the test repository framework for managing test suites and cases.
        /// </summary>
        public TestRepoFramework Tests => new TestRepoFramework(this);

        /// <summary>
        /// Gets the test run framework for managing test runs.
        /// </summary>
        public TestRunFramework Runs => new TestRunFramework(this);

        /// <summary>
        /// Gets the test resources framework for managing test resources.
        /// </summary>
        public TestResourcesFramework Resources => new TestResourcesFramework(this);

        /// <summary>
        /// Gets the test accounts framework for managing test accounts.
        /// </summary>
        public TestAccountsTestFramework Accounts => new TestAccountsTestFramework(this);

        /// <summary>
        /// Gets the requirements framework for managing requirements.
        /// </summary>
        public RequirementsTestFramework Requirements => new RequirementsTestFramework(this);

        /// <summary>
        /// Gets the settings framework for testing settings.
        /// </summary>
        public SettingsTestFramework Settings => new SettingsTestFramework(this);

        /// <summary>
        /// Gets the issues framework for managing issues.
        /// </summary>
        public IssuesTestFramework Issues => new IssuesTestFramework(this);

        /// <summary>
        /// Gets the application instance for the test bucket.
        /// </summary>
        public TestBucketApp App => _app;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectFixture"/> class.
        /// </summary>
        /// <param name="app">The application instance for the test bucket.</param>
        public ProjectFixture(TestBucketApp app)
        {
            _app = app;
        }

        /// <summary>
        /// Creates a new test project with a random name.
        /// </summary>
        /// <returns>The created test project.</returns>
        public async Task<TestProject> CreateProjectAsync()
        {
            var name = "Project-" + Guid.NewGuid().ToString();
            return await CreateProjectAsync(name);
        }

        /// <summary>
        /// Creates a new test project with the specified name.
        /// </summary>
        /// <param name="name">The name of the project to create.</param>
        /// <returns>The created test project.</returns>
        public async Task<TestProject> CreateProjectAsync(string name)
        {
            var user = Impersonation.Impersonate(App.Tenant);
            var project = new TestProject { Name = name, ShortName = "", Slug = "" };

            var projectManager = Services.GetRequiredService<IProjectManager>();
            await projectManager.AddAsync(user, project);
            return project;
        }

        /// <summary>
        /// Adds an integration to the current project.
        /// </summary>
        /// <param name="system">The external system to integrate.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddIntegrationAsync(ExternalSystem system)
        {
            var user = Impersonation.Impersonate(App.Tenant);
            var projectManager = Services.GetRequiredService<IProjectManager>();
            await projectManager.SaveProjectIntegrationAsync(user, Project.Slug, system);
        }

        /// <summary>
        /// Gets the integrations for the current project by project ID.
        /// </summary>
        /// <returns>A list of external systems integrated with the project.</returns>
        public async Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsByIdAsync()
        {
            var user = Impersonation.Impersonate(App.Tenant);
            var projectManager = Services.GetRequiredService<IProjectManager>();
            return await projectManager.GetProjectIntegrationsAsync(user, Project.Id);
        }

        /// <summary>
        /// Gets the integrations for the current project by project slug.
        /// </summary>
        /// <returns>A list of external systems integrated with the project.</returns>
        public async Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsBySlugAsync()
        {
            var user = Impersonation.Impersonate(App.Tenant);
            var projectManager = Services.GetRequiredService<IProjectManager>();
            return await projectManager.GetProjectIntegrationsAsync(user, Project.Slug);
        }

        /// <summary>
        /// Deletes an integration from the current project.
        /// </summary>
        /// <param name="system">The external system to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DeleteIntegrationAsync(ExternalSystem system)
        {
            var user = Impersonation.Impersonate(App.Tenant);
            var projectManager = Services.GetRequiredService<IProjectManager>();
            await projectManager.DeleteProjectIntegrationAsync(user, system.Id);
        }

        /// <summary>
        /// Gets the milestone field definition for the current project.
        /// </summary>
        /// <returns>The milestone field definition.</returns>
        public async Task<FieldDefinition> GetMilestoneFieldAsync()
        {
            var fieldDefinitionManager = Services.GetRequiredService<IFieldDefinitionManager>();
            var fieldDefinitions = await fieldDefinitionManager.GetDefinitionsAsync(App.SiteAdministrator, this.ProjectId);
            return fieldDefinitions.Where(x => x.TraitType == TraitType.Milestone).First();
        }

        /// <summary>
        /// Disposes of the resources used by the fixture.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Initializes the fixture by creating a test team and project.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Deletes the specified test project.
        /// </summary>
        /// <param name="project">The project to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        internal async Task DeleteProjectAsync(TestProject project)
        {
            var user = Impersonation.Impersonate(App.Tenant);
            var projectManager = Services.GetRequiredService<IProjectManager>();
            await projectManager.DeleteAsync(user, project);
        }
    }
}