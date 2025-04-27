using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Commands.Models;
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
            foreach (var command in _commands.Values.Where(x => x.Enabled))
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
            else
            {
                Debug.Assert(command is not null, $"Command not registered: {commandId}");
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<ICommand> GetCommandByContextMenuType(string typeName)
        {
            return _commands.Values.Where(x => x.ContextMenuTypes.Contains(typeName)).OrderBy(x=>x.SortOrder).ToList();
        }

        /// <inheritdoc/>
        public IReadOnlyList<CommandContextMenuItem> GetCommandMenuItems(string typeName)
        {
            Dictionary<string, CommandContextMenuItem> folderItems = [];
            var result = new List<CommandContextMenuItem>();

            foreach(var command in GetCommandByContextMenuType(typeName))
            {
                if (command.Folder is null)
                {
                    result.Add(new CommandContextMenuItem { Command = command });
                }
                else
                {
                    if(folderItems.TryGetValue(command.Folder, out var folder))
                    {
                        folder.FolderCommands.Add(command);
                    }
                    else
                    {
                        var folderItem = new CommandContextMenuItem();
                        folderItems[command.Folder] = folderItem;
                        result.Add(folderItem);
                        folderItem.Folder = command.Folder;
                        folderItem.FolderCommands.Add(command);
                    }
                }
            }

            // Sort all folders
            foreach(var folderItem in folderItems.Values)
            {
                folderItem.FolderCommands.Sort((a,b) =>
                {
                    return a.SortOrder - b.SortOrder;
                });
            }

            return result;
        }

    }
}
