using TestBucket.Domain.Commands;

namespace TestBucket.Components.Commands;

internal class CommandController : TenantBaseService
{
    private readonly ICommandManager _manager;

    public CommandController(AuthenticationStateProvider authenticationStateProvider, ICommandManager manager) 
        : base(authenticationStateProvider)
    {
        _manager = manager;
    }

    public async Task ExecuteAsync(string commandId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.ExecuteCommandAsync(principal, commandId);
    }
}
