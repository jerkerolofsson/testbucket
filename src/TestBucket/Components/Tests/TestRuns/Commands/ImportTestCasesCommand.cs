using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.Services;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestRuns.Commands;

/// <summary>
/// Command for importing test cases into the selected project.
/// </summary>
internal class ImportTestCasesCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestBrowser _browser;
    private readonly IStringLocalizer<SharedStrings> _loc;

    /// <summary>
    /// Gets the entity type required for permission checks.
    /// </summary>
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestCase;

    /// <summary>
    /// Gets the required permission level to execute the command.
    /// </summary>
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;

    /// <summary>
    /// Gets a value indicating whether the command is enabled.
    /// </summary>
    public bool Enabled => _appNavigationManager.State.SelectedProject is not null;

    /// <summary>
    /// Gets the unique identifier for the command.
    /// </summary>
    public string Id => "import-cases";

    /// <summary>
    /// Gets the localized name of the command.
    /// </summary>
    public string Name => _loc["import-test-cases"];

    /// <summary>
    /// Gets the localized description of the command.
    /// </summary>
    public string Description => _loc["import-test-cases"];

    /// <summary>
    /// Gets the default keyboard binding for the command, if any.
    /// </summary>
    public KeyboardBinding? DefaultKeyboardBinding => null;

    /// <summary>
    /// Gets the icon identifier for the command.
    /// </summary>
    public string? Icon => TbIcons.BoldDuoTone.Import;

    /// <summary>
    /// Gets the context menu types where this command should appear.
    /// </summary>
    public string[] ContextMenuTypes => ["menu-test"];

    /// <summary>
    /// Gets the sort order for the command in UI lists.
    /// </summary>
    public int SortOrder => 99;

    /// <summary>
    /// Gets the folder grouping for the command, if any.
    /// </summary>
    public string? Folder => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportTestCasesCommand"/> class.
    /// </summary>
    /// <param name="appNavigationManager">The application navigation manager.</param>
    /// <param name="browser">The test browser service.</param>
    /// <param name="loc">The string localizer for shared strings.</param>
    public ImportTestCasesCommand(AppNavigationManager appNavigationManager, TestBrowser browser, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _loc = loc;
    }

    /// <summary>
    /// Executes the import test results command asynchronously.
    /// </summary>
    /// <param name="principal">The user principal executing the command.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if (_appNavigationManager.State.SelectedProject is null)
        {
            return;
        }
        await _browser.ImportTestCasesAsync(_appNavigationManager.State.SelectedTeam, _appNavigationManager.State.SelectedProject);
    }
}