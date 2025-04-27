using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Settings.Models;
public class SettingsLink
{
    /// <summary>
    /// Title
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Icon
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Search keywords
    /// </summary>
    public required string Keywords { get; set; }

    /// <summary>
    /// URL without tenant
    /// </summary>
    public required string RelativeUrl { get; set; }
}
