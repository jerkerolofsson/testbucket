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
        public async Task ExecuteCommandAsync(ClaimsPrincipal principal, string commandId)
        {
            var command = GetCommandById(commandId);
            if (command is not null)
            {
                if (command.PermissionEntityType is not null && command.RequiredLevel is not null)
                {
                    if (!principal.HasPermission(command.PermissionEntityType.Value, command.RequiredLevel.Value))
                    {
                        return;
                    }
                }

                await command.ExecuteAsync(principal);
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
        public IReadOnlyList<CommandContextMenuItem> SearchCommandMenuItems(string searchText, int count)
        {
            Dictionary<string, CommandContextMenuItem> folderItems = [];
            var result = new List<CommandContextMenuItem>();

            HashSet<Type> processedTypes = [];

            foreach (var command in _commands.Values
                .Where(x => x.Enabled && x.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x=>x.SortOrder).Take(count))
            {
                if (processedTypes.Contains(command.GetType()))
                {
                    continue;
                }
                processedTypes.Add(command.GetType());
                result.Add(new CommandContextMenuItem { Command = command, SortOrder = command.SortOrder });
            }

            return result;
        }

        /// <inheritdoc/>
        public IReadOnlyList<CommandContextMenuItem> GetCommandMenuItems(string typeNames)
        {
            Dictionary<string, CommandContextMenuItem> folderItems = [];
            var result = new List<CommandContextMenuItem>();

            HashSet<Type> processedTypes = [];

            foreach (var typeName in typeNames.Split())
            {
                foreach (var command in GetCommandByContextMenuType(typeName))
                {
                    if(processedTypes.Contains(command.GetType()))
                    {
                        continue;
                    }
                    processedTypes.Add(command.GetType());

                    if (command.Folder is null)
                    {
                        result.Add(new CommandContextMenuItem { Command = command, SortOrder = command.SortOrder });
                    }
                    else
                    {
                        if (folderItems.TryGetValue(command.Folder, out var folder))
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
                            folderItem.SortOrder = Math.Min(folderItem.SortOrder, command.SortOrder);
                        }
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

            result.Sort((a, b) =>
            {
                return a.SortOrder - b.SortOrder;
            });

            return result;
        }

    }
}
