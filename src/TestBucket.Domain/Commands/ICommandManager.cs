using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Keyboard;

namespace TestBucket.Domain.Commands
{
    /// <summary>
    /// Manages commands
    /// </summary>
    public interface ICommandManager
    {
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
        /// Executes a command that doesn't require any arguments
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns></returns>
        Task ExecuteCommandAsync(string commandId);

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
    }
}
