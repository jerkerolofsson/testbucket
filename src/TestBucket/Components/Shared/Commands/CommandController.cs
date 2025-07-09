
using TestBucket.Domain.Commands;
using TestBucket.Domain.Commands.Models;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Components.Shared.Commands;

internal class CommandController : TenantBaseService
{
    private readonly ICommandManager _commandManager;

    public event EventHandler<ICommand>? CommandExecuting
    {
        add => _commandManager.CommandExecuting += value;
        remove => _commandManager.CommandExecuting -= value;
    }

    public CommandController(AuthenticationStateProvider authenticationStateProvider, ICommandManager commandManager) : base(authenticationStateProvider)
    {
        _commandManager = commandManager;
    }

    public IReadOnlyList<ICommand> GetCommands() => _commandManager.GetCommands();

    /// <summary>
    /// Returns a command by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ICommand? GetCommandById(string id) => _commandManager.GetCommandById(id);

    /// <summary>
    /// Returns a list of commands applicable for a context menu for an entity type defined by typeName
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public IReadOnlyList<ICommand> GetCommandByContextMenuType(string typeName) => _commandManager.GetCommandByContextMenuType(typeName);

    /// <summary>
    /// Returns a list of commands applicable for a context menu for an entity type defined by typeName
    /// </summary>
    /// <param name="typeNames"></param>
    /// <returns></returns>
    public IReadOnlyList<CommandContextMenuItem> GetCommandMenuItems(string typeNames) => _commandManager.GetCommandMenuItems(typeNames);

    /// <summary>
    /// Executes a command that doesn't require any arguments
    /// </summary>
    /// <param name="commandId"></param>
    /// <returns></returns>
    public async Task ExecuteAsync(string commandId)
    {
        var user = await GetUserClaimsPrincipalAsync();
        await _commandManager.ExecuteCommandAsync(user, commandId);
    }
    /// <summary>
    /// Executes a command that doesn't require any arguments
    /// </summary>
    /// <param name="commandId"></param>
    /// <returns></returns>
    public async Task ExecuteCommandAsync(string commandId)
    {
        var user = await GetUserClaimsPrincipalAsync();
        await _commandManager.ExecuteCommandAsync(user, commandId);
    }

    /// <summary>
    /// Executes a command that doesn't require any arguments
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task ExecuteCommandAsync(ICommand command)
    {
        var user = await GetUserClaimsPrincipalAsync();
        await _commandManager.ExecuteCommandAsync(user, command);
    }

    /// <summary>
    /// Gets a keyboard binding for a command
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public async Task<KeyboardBindings> GetKeyboardBindingsAsync()
    {
        var user = await GetUserClaimsPrincipalAsync();
        return await _commandManager.GetKeyboardBindingsAsync(user);
    }

    /// <summary>
    /// Saves a keyboard binding for a command for the current user
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="commandId"></param>
    /// <param name="binding"></param>
    /// <returns></returns>
    public async Task SetKeyboardBindingAsync(string commandId, KeyboardBinding binding)
    {
        var user = await GetUserClaimsPrincipalAsync();
        await _commandManager.SetKeyboardBindingAsync(user, commandId, binding);
    }
    public IReadOnlyList<CommandContextMenuItem> SearchCommandMenuItems(string searchText, int count) => _commandManager.SearchCommandMenuItems(searchText, count);
}
