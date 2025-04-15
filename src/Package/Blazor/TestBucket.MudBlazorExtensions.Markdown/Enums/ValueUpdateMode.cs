using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.MudBlazorExtensions.Markdown.Enums;

/// <summary>
/// The event that triggers an update of value/markdown text from JS to C#
/// </summary>
public enum ValueUpdateMode
{
    /// <summary>
    /// Every character change
    /// </summary>
    OnChange = 1,

    /// <summary>
    /// When the editor lost focus
    /// </summary>
    OnBlur = 2,
}
