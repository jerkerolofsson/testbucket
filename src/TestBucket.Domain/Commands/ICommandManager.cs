using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Commands.Models;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Domain.Commands
{
    /// <summary>
    /// Manages commands
    /// </summary>
    public interface ICommandManager
    {
        /// <summary>
        /// Invoked before a command is executed
        /// </summary>
        event EventHandler<ICommand>? CommandExecuting;

        /// <summary>
        /// Returns a command by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ICommand? GetCommandById(string id);

        /// <summary>
        /// Returns a list of commands applicable for a context menu for an entity type defined by typeName
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        IReadOnlyList<ICommand> GetCommandByContextMenuType(string typeName);

        /// <summary>
        /// Returns a list of commands applicable for a context menu for an entity type defined by typeName
        /// </summary>
        /// <param name="typeNames"></param>
        /// <returns></returns>
        IReadOnlyList<CommandContextMenuItem> GetCommandMenuItems(string typeNames);

        /// <summary>
        /// Executes a command that doesn't require any arguments
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns></returns>
        Task ExecuteCommandAsync(ClaimsPrincipal principal, string commandId);

        /// <summary>
        /// Executes a command that doesn't require any arguments
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task ExecuteCommandAsync(ClaimsPrincipal principal, ICommand command);

        /// <summary>
        /// Gets a keyboard binding for a command
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        Task<KeyboardBindings> GetKeyboardBindingsAsync(ClaimsPrincipal principal);

        /// <summary>
        /// Saves a keyboard binding for a command for the current user
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="commandId"></param>
        /// <param name="binding"></param>
        /// <returns></returns>
        Task SetKeyboardBindingAsync(ClaimsPrincipal principal, string commandId, KeyboardBinding binding);
        IReadOnlyList<CommandContextMenuItem> SearchCommandMenuItems(string searchText, int count);
    }
}
