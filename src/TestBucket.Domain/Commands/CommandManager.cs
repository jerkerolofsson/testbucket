using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Identity;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Domain.Commands
{
    internal class CommandManager : ICommandManager
    { 
        private readonly Dictionary<string, ICommand> _commands = [];
        private readonly IUserPreferencesManager _userPreferencesManager;

        public CommandManager(IEnumerable<ICommand> commands, IUserPreferencesManager userPreferencesManager)
        {
            foreach(var command in commands)
            {
                _commands[command.Id] = command;
            }
            _userPreferencesManager = userPreferencesManager;
        }

        /// <inheritdoc/>
        public ICommand? GetCommandById(string id)
        {
            if(_commands.TryGetValue(id, out var command))
            {
                return command;
            }
            return null;
        }

        /// <inheritdoc/>
        public async Task SetKeyboardBindingAsync(ClaimsPrincipal principal, string commandId, KeyboardBinding binding)
        {
            var userPreferences = await _userPreferencesManager.LoadUserPreferencesAsync(principal);
            var bindings = userPreferences.KeyboardBindings ?? new();
            bindings.Commands ??= new();
            bindings.Commands[commandId] = binding;
            await _userPreferencesManager.SaveUserPreferencesAsync(principal, userPreferences);
        }

        /// <inheritdoc/>
        public async Task<KeyboardBindings> GetKeyboardBindingsAsync(ClaimsPrincipal principal)
        {
            var userPreferences = await _userPreferencesManager.LoadUserPreferencesAsync(principal);

            var bindings = userPreferences.KeyboardBindings ?? new();
            bindings.Commands ??= new();

            // Add default bindings for commands
            foreach (var command in _commands.Values)
            {
                if(!bindings.Commands.ContainsKey(command.Id) && command.DefaultKeyboardBinding is not null)
                {
                    bindings.Commands[command.Id] = command.DefaultKeyboardBinding;
                }
            }

            return bindings;
        }

        /// <inheritdoc/>
        public async Task ExecuteCommandAsync(string commandId)
        {
            var command = GetCommandById(commandId);
            if (command is not null)
            {
                await command.ExecuteAsync();
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<ICommand> GetCommandByContextMenuType(string typeName)
        {
            return _commands.Values.Where(x => x.ContextMenuTypes.Contains(typeName)).ToList();
        }
    }
}
