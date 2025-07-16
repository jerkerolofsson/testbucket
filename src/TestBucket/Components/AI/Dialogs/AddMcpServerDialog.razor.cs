using TestBucket.Domain.AI.Mcp.Models;

namespace TestBucket.Components.AI.Dialogs;
public partial class AddMcpServerDialog
{
    private string _errorMessage = "";

    private McpServerConfiguration? _configuration;

    [Parameter] public TestProject? Project { get; set; }

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;

    private string _json = """
        {
          "mcpServers": {
            "acuvity-mcp-server-playwright": {
              "url": "http://localhost:8000/sse"
            }
          }
        }
        """;
    private void OnJsonChanged(string value)
    {
        _json = value;
        Validate();
    }

    private void Close()
    {
        MudDialog.Close();
    }

    public McpServerRegistration? Validate()
    {
        _errorMessage = "";
        _configuration = null;
        if (Project is null)
        {
            _errorMessage = "project missing";
            return null;
        }

        if (string.IsNullOrEmpty(_json))
        {
            _errorMessage = loc.Validation["validation-value-empty"];
            return null;
        }

        try
        {
            var configuration = McpServerConfiguration.FromJson(_json);
            if (configuration is null)
            {
                _errorMessage = loc.Validation["validation-invalid-json"];
                return null;
            }
            if (configuration.Servers is null || configuration.Servers.Count == 0)
            {
                _errorMessage = loc.Validation["validation-mcp-server-empty"];
                return null;
            }

            foreach (var serverPair in configuration.Servers)
            {
                var name = serverPair.Key;
                var server = serverPair.Value;
                if (string.IsNullOrEmpty(server.Url) && string.IsNullOrEmpty(server.Command))
                {
                    _errorMessage = loc.Validation["validation-mcp-server-url-and-command-empty"];
                    return null;
                }
            }

            _configuration = configuration;
            var registration = new McpServerRegistration
            {
                Configuration = configuration,
                TestProjectId = Project.Id
            };
            return registration;
        }
        catch (Exception)
        {
            _errorMessage = loc.Validation["validation-invalid-json"];
            return null;
        }
    }

    private void OnValidSubmit()
    {
        var registration = Validate();
        if (registration is not null)
        {
            MudDialog.Close(registration);
        }
    }
}