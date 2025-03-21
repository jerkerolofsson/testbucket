using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Identity.Models;
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
    /// Use dark mode
    /// </summary>
    public bool DarkMode { get; set; } = true;

    /// <summary>
    /// Custom Theme
    /// </summary>
    public string? Theme { get; set; }

    /// <summary>
    /// Increased contrast
    /// </summary>
    public bool IncreasedContrast { get; set; }

    #endregion User Interface

    #region Test Execution

    /// <summary>
    /// Shows a dialog that let's the user enter a failure message
    /// </summary>
    public bool ShowFailureMessageDialogWhenFailingTestCaseRun { get; set; } = true;

    #endregion Test Execution
}
