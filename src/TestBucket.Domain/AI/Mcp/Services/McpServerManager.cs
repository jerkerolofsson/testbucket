using Mediator;

using TestBucket.Domain.AI.Mcp.Events;
using TestBucket.Domain.AI.Mcp.Models;

namespace TestBucket.Domain.AI.Mcp.Services;
internal class McpServerManager : IMcpServerManager, IMcpServerUserInputProvider
{
    private readonly IMcpServerRepository _serverRepository;
    private readonly IMcpUserInputRepository _userInputRepository;
    private readonly TimeProvider _timeProvider;
    private readonly IMediator _mediator;

    public McpServerManager(IMcpServerRepository serverRepository, IMcpUserInputRepository userInputRepository, TimeProvider timeProvider, IMediator mediator)
    {
        _serverRepository = serverRepository;
        _userInputRepository = userInputRepository;
        _timeProvider = timeProvider;
        _mediator = mediator;
    }

    public async Task AddMcpServerRegistrationAsync(ClaimsPrincipal principal, McpServerRegistration registration)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.McpServer, PermissionLevel.Write);
        registration.TenantId = principal.GetTenantIdOrThrow();
        registration.ModifiedBy = registration.CreatedBy = principal.Identity?.Name ?? throw new Exception("Missing user identity");
        registration.Modified = registration.Created = _timeProvider.GetUtcNow();
        registration.PublicForProject = false;
        registration.Enabled = true;
        registration.Locked = false;

        if (registration.Configuration.Servers is null)
        {
            throw new ArgumentException("Servers not configureed");
        }
        foreach(var server in registration.Configuration.Servers)
        {
            if(string.IsNullOrEmpty(server.Value.Type))
            {
                if (!string.IsNullOrEmpty(server.Value.Command))
                {
                    server.Value.Type ??= "stdio";
                }
                if (!string.IsNullOrEmpty(server.Value.Url))
                {
                    server.Value.Command = null;
                    server.Value.Args = null;
                    server.Value.Type ??= "auto";
                }
            }
        }

        await _serverRepository.AddAsync(registration);

        await _mediator.Publish(new McpServerAdded(registration));
    }

    public async Task DeleteMcpServerRegistrationAsync(ClaimsPrincipal principal, McpServerRegistration registration)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.McpServer, PermissionLevel.Delete);
        principal.ThrowIfEntityTenantIsDifferent(registration);

        await _serverRepository.DeleteAsync(registration);

        await _mediator.Publish(new McpServerRemoved(registration));
    }

    public async Task UpdateMcpServerRegistrationAsync(ClaimsPrincipal principal, McpServerRegistration registration)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.McpServer, PermissionLevel.Write);
        principal.ThrowIfEntityTenantIsDifferent(registration);
        registration.TenantId = principal.GetTenantIdOrThrow();
        registration.CreatedBy = principal.Identity?.Name ?? throw new Exception("Missing user identity");
        registration.ModifiedBy = registration.CreatedBy;
        registration.Created = _timeProvider.GetUtcNow();
        registration.Modified = _timeProvider.GetUtcNow();

        await _serverRepository.UpdateAsync(registration);

        await _mediator.Publish(new McpServerUpdated(registration));
    }
    public async Task AddUserInputAsync(ClaimsPrincipal principal, McpServerUserInput userInput)
    {
        userInput.TenantId = principal.GetTenantIdOrThrow();
        userInput.UserName = userInput.ModifiedBy = userInput.CreatedBy = principal.Identity?.Name ?? throw new Exception("Missing user identity");
        userInput.Modified = userInput.Created = _timeProvider.GetUtcNow();

        await _userInputRepository.AddAsync(userInput);
    }

    public async Task ClearUserInputsAsync(ClaimsPrincipal principal, long projectId, long mcpServerRegistrationId)
    {
        var userName = principal.Identity?.Name ?? throw new Exception("Missing user identity");
        await _userInputRepository.ClearUserInputsAsync(projectId, userName, mcpServerRegistrationId);
    }

    public async Task DeleteUserInputAsync(ClaimsPrincipal principal, McpServerUserInput userInput)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.McpServer, PermissionLevel.Delete);
        principal.ThrowIfEntityTenantIsDifferent(userInput);

        await _userInputRepository.DeleteAsync(userInput);
    }

    public async Task<IReadOnlyList<McpServerRegistration>> GetAllMcpServerRegistationsAsync(ClaimsPrincipal principal, long projectId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.McpServer, PermissionLevel.Read);
        return await _serverRepository.GetAllAsync(principal.GetTenantIdOrThrow(), projectId);
    }

    public async Task<IReadOnlyList<McpServerRegistration>> GetUserMcpServerRegistationsAsync(ClaimsPrincipal principal, long projectId)
    {
        var userName = principal.Identity?.Name ?? throw new Exception("Missing user identity");
        principal.ThrowIfNoPermission(PermissionEntityType.McpServer, PermissionLevel.Read);
        return await _serverRepository.GetMcpServersForUserAsync(principal.GetTenantIdOrThrow(), projectId, userName);
    }

    public async Task<CompiledMcpServerConfiguration> GetMcpServerConfigurationAsync(ClaimsPrincipal principal, long projectId, McpServerRegistration server)
    {
        var compiler = new McpConfigurationCompiler(this);
        return await compiler.CompileAsync(principal, projectId, server.Id, server.Configuration);
    }

    public async Task<McpServerUserInput?> GetUserInputAsync(long projectId, string userName, long mcpServerRegistrationId, string id)
    {
        return await _userInputRepository.GetUserInputAsync(projectId, userName, mcpServerRegistrationId, id); 
    }


    public async Task UpdateUserInputAsync(ClaimsPrincipal principal, McpServerUserInput userInput)
    {
        userInput.TenantId = principal.GetTenantIdOrThrow();
        userInput.UserName = userInput.ModifiedBy = principal.Identity?.Name ?? throw new Exception("Missing user identity");
        userInput.Modified = _timeProvider.GetUtcNow();

        await _userInputRepository.UpdateAsync(userInput); 
    }

    //public async Task<McpAIFunction[]> GetMcpToolsForUserAsync(ClaimsPrincipal principal, long projectId)
    //{
    //    List<McpAIFunction> functions = [];
    //    var registrations = await GetUserMcpServerRegistationsAsync(principal, projectId);

    //    // Remove MCP servers that are locked and not enabled
    //    registrations = registrations.Where(x => x.Enabled && !x.Locked).ToList();

    //    // Todo: Locked by us is OK?

    //    // Check MCP servers that are running and get the tools


    //    return functions.ToArray();
    //}
}
