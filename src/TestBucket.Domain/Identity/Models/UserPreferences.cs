using TestBucket.Contracts.Appearance.Models;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Domain.Identity.Models;

// todo: Add for next migration
//[Index(nameof(UserName))]
public class UserPreferences
{
    public long Id { get; set; }

    /// <summary>
    /// Tenant
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// Authenticated Identity.Name
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// Active team
    /// </summary>
    public long? ActiveTeamId { get; set; }

    /// <summary>
    /// Active project
    /// </summary>
    public long? ActiveProjectId { get; set; }

    #region User Interface

    /// <summary>
    /// Keyboard bindings
    /// </summary>
    [Column(TypeName = "jsonb")]
    public KeyboardBindings? KeyboardBindings { get; set; }

    /// <summary>
    /// Use dark mode
    /// </summary>
    public bool DarkMode { get; set; } = true;

    /// <summary>
    /// Where to dock the explorer window (left or right)
    /// </summary>
    public Dock? ExplorerDock { get; set; }

    /// <summary>
    /// Custom Theme
    /// </summary>
    public string? Theme { get; set; }

    /// <summary>
    /// Increased contrast. This applies an overlay to the current theme, changing colors to colors with high contrast
    /// </summary>
    public bool IncreasedContrast { get; set; }

    /// <summary>
    /// Increased font size. This applies an overlay to the current theme, increasing the font size
    /// </summary>
    public bool IncreasedFontSize { get; set; }

    /// <summary>
    /// Reduced motion. This disables animation/transitions
    /// </summary>
    public bool? ReducedMotion { get; set; }

    /// <summary>
    /// If true, text is used instead of icons or images
    /// </summary>
    public bool PreferTextToIcons { get; set; }

    #endregion User Interface

    #region Test Execution

    /// <summary>
    /// Shows a dialog that let's the user enter a failure message
    /// </summary>
    public bool ShowFailureMessageDialogWhenFailingTestCaseRun { get; set; } = true;

    /// <summary>
    /// Advances to the next test when passing/failing etc a test
    /// </summary>
    public bool AdvanceToNextNotCompletedTestWhenSettingResult { get; set; } = true;

    #endregion Test Execution
}
