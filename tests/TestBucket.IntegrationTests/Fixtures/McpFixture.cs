using ModelContextProtocol.Client;
using System.Security.Claims;
using TestBucket.Contracts.Projects;
using TestBucket.Domain.Identity;
using TestBucket.Formats.Dtos;

namespace TestBucket.IntegrationTests.Fixtures
{
    public class McpFixture(TestBucketApp App) : IAsyncLifetime
    {
        private string? _team;
        private ProjectDto? _project;
        private TestSuiteDto? _testSuite;
        private string? _accessToken;
        private IMcpClient? _mcpClient;

        public TestBucketApp TestBucket => App;

        public string ProjectSlug => _project?.Slug ?? throw new InvalidOperationException("Not initialized yet");
        public string TeamSlug => _team ?? throw new InvalidOperationException("Not initialized yet");
        public string TestSuiteSlug => _testSuite?.Slug ?? throw new InvalidOperationException("Not initialized yet");

        public Uri McpUrl => new Uri(App.HttpBaseUrl + "mcp");

        public IMcpClient McpClient => _mcpClient ?? throw new InvalidOperationException("Not initialized yet");

        public ClaimsPrincipal Principal => Impersonation.Impersonate(App.Tenant);
                
        protected async Task<IMcpClient> CreateAsync()
        {
            var clientTransport = new SseClientTransport(new SseClientTransportOptions
            {
                Endpoint = new Uri(App.HttpBaseUrl + "mcp"),
                AdditionalHeaders = new Dictionary<string, string>
                {
                    ["Authorization"] = "Bearer " + _accessToken
                }
            });

            var client = await McpClientFactory.CreateAsync(clientTransport, null, null, TestContext.Current.CancellationToken);
            return client;
        }


        public async ValueTask DisposeAsync()
        {
            if (_project is not null)
            {
                await App.Client.Projects.DeleteAsync(_project.Slug);
            }
            if (_team is not null)
            {
                await App.Client.Teams.DeleteAsync(_team);
            }
            if(_mcpClient is not null)
            {
                await _mcpClient.DisposeAsync();
            }
        }

        public async ValueTask InitializeAsync()
        {
            _team = await App.Client.Teams.AddAsync("Team " + Guid.NewGuid().ToString());
            _project = await App.Client.Projects.AddAsync(new ProjectDto { Team = _team, Name = "My project " + Guid.NewGuid().ToString(), ExternalSystems = [], ShortName = "", Slug = "" });

            var testSuite = new TestSuiteDto
            {
                Name = "Suite" + Guid.NewGuid().ToString(),
                ProjectSlug = ProjectSlug, 
                TeamSlug = TeamSlug,
            };
            _testSuite = await App.Client.TestRepository.AddSuiteAsync(testSuite);

            _accessToken = App.GenerateProjectAccessToken(Principal, _project.Id);
            _mcpClient = await CreateAsync();
        }
    }
}
