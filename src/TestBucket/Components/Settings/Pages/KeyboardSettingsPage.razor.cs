using TestBucket.Contracts.Keyboard;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Components.Settings.Pages;
public partial class KeyboardSettingsPage
{
    private KeyboardBindings? _bindings;
    private readonly List<CommandBinding> _commands = [];

    private record class CommandBinding(ICommand Command, KeyboardBinding? UserBinding);

    [Parameter] public string? TenantId { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_bindings is null)
        {
            var userPreferences = await userPreferencesController.LoadUserPreferencesAsync();
            userPreferences.KeyboardBindings ??= new();
            _bindings = userPreferences.KeyboardBindings;
            _bindings.Commands ??= [];

            ReloadCommandBindings();

            this.StateHasChanged();
        }
    }

    private void ReloadCommandBindings()
    {
        var commands = commandManager.GetCommands();
        _commands.Clear();
        foreach (var command in commands)
        {
            if (_bindings.Commands is not null && _bindings.Commands.TryGetValue(command.Id, out var binding))
            {
                _commands.Add(new CommandBinding(command, binding));
            }
            else
            {
                _commands.Add(new CommandBinding(command, null));
            }
        }
    }

    private async Task OnCommandBindingChangedAsync(CommandBinding commandBinding, (ModifierKey Modifiers, string? Key) shortcut)
    {
        if (_bindings is null)
        {
            return;
        }
        _bindings.Commands ??= [];
        if (shortcut.Key is not null)
        {
            _bindings.Commands[commandBinding.Command.Id] = new KeyboardBinding
            {
                CommandId = commandBinding.Command.Id,
                ModifierKeys = shortcut.Modifiers,
                Key = shortcut.Key
            };
        }
        else
        {
            _bindings.Commands.Remove(commandBinding.Command.Id);
        }

        ReloadCommandBindings();

        var userPreferences = await userPreferencesController.LoadUserPreferencesAsync();
        userPreferences.KeyboardBindings = _bindings;
        await userPreferencesController.SaveUserPreferencesAsync(userPreferences);
    }

    private async Task OnCommandPaletteBindingChangedAsync((ModifierKey Modifiers, string? Key) shortcut)
    {
        if (_bindings is null)
        {
            return;
        }
        if (shortcut.Key is not null)
        {
            _bindings.CommandPaletteBinding = new KeyboardBinding
            {
                CommandId = "",
                ModifierKeys = shortcut.Modifiers,
                Key = shortcut.Key
            };
        }
        else
        {
            _bindings.CommandPaletteBinding = null;
        }

        var userPreferences = await userPreferencesController.LoadUserPreferencesAsync();
        userPreferences.KeyboardBindings = _bindings;
        await userPreferencesController.SaveUserPreferencesAsync(userPreferences);
    }
    private async Task OnUnifiedSearchBindingChangedAsync((ModifierKey Modifiers, string? Key) shortcut)
    {
        if (_bindings is null)
        {
            return;
        }
        if (shortcut.Key is not null)
        {
            _bindings.UnifiedSearchBinding = new KeyboardBinding
            {
                CommandId = "",
                ModifierKeys = shortcut.Modifiers,
                Key = shortcut.Key
            };
        }
        else
        {
            _bindings.UnifiedSearchBinding = null;
        }

        var userPreferences = await userPreferencesController.LoadUserPreferencesAsync();
        userPreferences.KeyboardBindings = _bindings;
        await userPreferencesController.SaveUserPreferencesAsync(userPreferences);
    }
}