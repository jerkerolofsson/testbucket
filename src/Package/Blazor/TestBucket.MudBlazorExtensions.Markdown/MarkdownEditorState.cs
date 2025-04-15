using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.MudBlazorExtensions.Markdown;
public class MarkdownEditorState
{
    /// <summary>
    /// True if preview mode is enabled
    /// </summary>
    public bool? Preview { get; set; }

    /// <summary>
    /// True if fullscreen is enabled
    /// </summary>
    public bool? Fullscreen { get; set; }
}
