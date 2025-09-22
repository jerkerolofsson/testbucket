using System.Security.Claims;

using NSubstitute;

using TestBucket.Contracts.Keyboard;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Models;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Domain.UnitTests.Commands;

/// <summary>
/// Unit tests for <see cref="CommandManager"/> and command-related functionality.
/// </summary>
[UnitTest]
[EnrichedTest]
[Component("Commands")]
[Feature("Commands")]
public class CommandManagerTests
{
    /// <summary>
    /// Mocked user preferences manager.
    /// </summary>
    private readonly IUserPreferencesManager _userPreferencesManager;

    /// <summary>
    /// Test principal used for simulating user context.
    /// </summary>
    private readonly ClaimsPrincipal _principal;

    /// <summary>
    /// List of test commands used in the tests.
    /// </summary>
    private readonly List<ICommand> _commands;

    /// <summary>
    /// The <see cref="CommandManager"/> instance under test.
    /// </summary>
    private readonly CommandManager _manager;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandManagerTests"/> class.
    /// </summary>
    public CommandManagerTests()
    {
        _userPreferencesManager = Substitute.For<IUserPreferencesManager>();
        _principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("name", "user") }));
        _commands = new List<ICommand>
        {
            new FakeCommand1("cmd1", 1, enabled: true),
            new FakeCommand2("cmd2", 2, enabled: false, contextMenuTypes: new[] { "TypeA" }),
            new FakeCommand3("cmd3", 3, enabled: true, contextMenuTypes: new[] { "TypeA", "TypeB" })
        };
        _manager = new CommandManager(_commands, _userPreferencesManager);
    }

    /// <summary>
    /// Verifies that <see cref="CommandManager.GetCommands"/> returns all commands sorted by their sort order.
    /// </summary>
    [Fact]
    public void GetCommands_ReturnsAllCommandsSorted()
    {
        var result = _manager.GetCommands();
        Assert.Equal(3, result.Count);
        Assert.Equal("cmd1", result[0].Id);
        Assert.Equal("cmd2", result[1].Id);
        Assert.Equal("cmd3", result[2].Id);
    }

    /// <summary>
    /// Verifies that <see cref="CommandManager.GetCommandById(string)"/> returns the correct command or null if not found.
    /// </summary>
    [Fact]
    public void GetCommandById_ReturnsCorrectCommand()
    {
        var cmd = _manager.GetCommandById("cmd1");
        Assert.NotNull(cmd);
        Assert.Equal("cmd1", cmd!.Id);

        var missing = _manager.GetCommandById("notfound");
        Assert.Null(missing);
    }

    /// <summary>
    /// Verifies that <see cref="CommandManager.SetKeyboardBindingAsync"/> saves a keyboard binding if it does not already exist.
    /// </summary>
    [Fact]
    public async Task SetKeyboardBindingAsync_SavesBindingIfNotExists()
    {
        var prefs = new UserPreferences { TenantId = "t", UserName = "u", KeyboardBindings = new KeyboardBindings { Commands = new() } };
        _userPreferencesManager.LoadUserPreferencesAsync(_principal).Returns(prefs);

        var binding = new KeyboardBinding { CommandId = "cmd1", Key = "A", ModifierKeys = ModifierKey.Ctrl };
        await _manager.SetKeyboardBindingAsync(_principal, "cmd1", binding);

        Assert.True(prefs.KeyboardBindings!.Commands!.ContainsKey("cmd1"));
        await _userPreferencesManager.Received(1).SaveUserPreferencesAsync(_principal, prefs);
    }

    /// <summary>
    /// Verifies that <see cref="CommandManager.GetKeyboardBindingsAsync"/> adds default bindings for enabled commands.
    /// </summary>
    [Fact]
    public async Task GetKeyboardBindingsAsync_AddsDefaultBindingsForEnabledCommands()
    {
        var prefs = new UserPreferences { TenantId = "t", UserName = "u", KeyboardBindings = new KeyboardBindings { Commands = new() } };
        _userPreferencesManager.LoadUserPreferencesAsync(_principal).Returns(prefs);

        var result = await _manager.GetKeyboardBindingsAsync(_principal);

        // Only enabled commands with DefaultKeyboardBinding should be added
        Assert.All(_commands.Where(c => c.Enabled && c.DefaultKeyboardBinding != null), c =>
            Assert.True(result.Commands!.ContainsKey(c.Id)));
    }

    /// <summary>
    /// Verifies that <see cref="CommandManager.ExecuteCommandAsync(ClaimsPrincipal, string)"/> invokes the correct command.
    /// </summary>
    [Fact]
    public async Task ExecuteCommandAsync_ById_InvokesCommand()
    {
        var fake = Substitute.For<ICommand>();
        fake.Id.Returns("cmdX");
        fake.Enabled.Returns(true);
        fake.ExecuteAsync(_principal).Returns(ValueTask.CompletedTask);

        var manager = new CommandManager(new[] { fake }, _userPreferencesManager);
        await manager.ExecuteCommandAsync(_principal, "cmdX");

        await fake.Received(1).ExecuteAsync(_principal);
    }

    /// <summary>
    /// Verifies that <see cref="CommandManager.GetCommandByContextMenuType(string)"/> returns commands matching the specified context menu type.
    /// </summary>
    [Fact]
    public void GetCommandByContextMenuType_ReturnsMatchingCommands()
    {
        var result = _manager.GetCommandByContextMenuType("TypeA");
        Assert.Contains(result, c => c.Id == "cmd2");
        Assert.Contains(result, c => c.Id == "cmd3");
    }


    /// <summary>
    /// Verifies that <see cref="CommandManager.SearchCommandMenuItems(string, int)"/> excludes disabled items
    /// </summary>
    [Fact]
    public void SearchCommandMenuItems_ExcludesDisabled()
    {
        var commands = new List<ICommand>
        {
            new FakeCommand1("cmd1", 1, enabled: false),
            new FakeCommand2("cmd2", 2, enabled: false, contextMenuTypes: new[] { "TypeA" }),
            new FakeCommand3("cmd3", 3, enabled: true, contextMenuTypes: new[] { "TypeA", "TypeB" })
        };
        var manager = new CommandManager(commands, _userPreferencesManager);

        var result = manager.SearchCommandMenuItems("cmd", 10);
        Assert.Single(result);

        var filtered = manager.SearchCommandMenuItems("cmd1", 10);
        Assert.Empty(filtered);
    }

    /// <summary>
    /// Verifies that <see cref="CommandManager.SearchCommandMenuItems(string, int)"/> finds commands by name.
    /// </summary>
    [Fact]
    public void SearchCommandMenuItems_FindsByName()
    {
        var commands = new List<ICommand>
        {
            new FakeCommand1("cmd1", 1, enabled: true),
            new FakeCommand2("cmd2", 2, enabled: true, contextMenuTypes: new[] { "TypeA" }),
            new FakeCommand3("cmd3", 3, enabled: true, contextMenuTypes: new[] { "TypeA", "TypeB" })
        };
        var manager = new CommandManager(commands, _userPreferencesManager);

        var result = manager.SearchCommandMenuItems("cmd", 10);
        Assert.Equal(3, result.Count);

        var filtered = manager.SearchCommandMenuItems("cmd1", 10);
        Assert.Single(filtered);
        Assert.Equal("cmd1", filtered[0].Command!.Id);
    }

    private class FakeCommand1 : FakeCommand
    {
        public FakeCommand1(string id, int sortOrder, bool enabled = true, string[]? contextMenuTypes = null) : base(id, sortOrder, enabled, contextMenuTypes) { }
    }
    private class FakeCommand2 : FakeCommand
    {
        public FakeCommand2(string id, int sortOrder, bool enabled = true, string[]? contextMenuTypes = null) : base(id, sortOrder, enabled, contextMenuTypes) { }
    }
    private class FakeCommand3 : FakeCommand
    {
        public FakeCommand3(string id, int sortOrder, bool enabled = true, string[]? contextMenuTypes = null) : base(id, sortOrder, enabled, contextMenuTypes) { }
    }

    /// <summary>
    /// Fake implementation of <see cref="ICommand"/> for testing purposes.
    /// </summary>
    private class FakeCommand : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeCommand"/> class.
        /// </summary>
        /// <param name="id">Command ID.</param>
        /// <param name="sortOrder">Sort order.</param>
        /// <param name="enabled">Whether the command is enabled.</param>
        /// <param name="contextMenuTypes">Context menu types.</param>
        public FakeCommand(string id, int sortOrder, bool enabled = true, string[]? contextMenuTypes = null)
        {
            Id = id;
            SortOrder = sortOrder;
            Enabled = enabled;
            ContextMenuTypes = contextMenuTypes ?? Array.Empty<string>();
            DefaultKeyboardBinding = new KeyboardBinding { CommandId = id, Key = "A", ModifierKeys = ModifierKey.Ctrl };
        }

        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc/>
        public int SortOrder { get; }

        /// <inheritdoc/>
        public bool Enabled { get; }

        /// <inheritdoc/>
        public string Name => Id;

        /// <inheritdoc/>
        public string Description => "desc";

        /// <inheritdoc/>
        public string? Icon => null;

        /// <inheritdoc/>
        public string? Folder => null;

        /// <inheritdoc/>
        public string[] ContextMenuTypes { get; }

        /// <inheritdoc/>
        public KeyboardBinding? DefaultKeyboardBinding { get; }

        /// <inheritdoc/>
        public PermissionEntityType? PermissionEntityType => null;

        /// <inheritdoc/>
        public PermissionLevel? RequiredLevel => null;

        /// <inheritdoc/>
        public ValueTask ExecuteAsync(ClaimsPrincipal principal) => ValueTask.CompletedTask;
    }
}