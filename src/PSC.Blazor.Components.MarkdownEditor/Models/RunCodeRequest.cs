using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSC.Blazor.Components.MarkdownEditor.Models;
public class RunCodeRequest
{
    /// <summary>
    /// Text from markdown code block
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Code block language
    /// </summary>
    public required string Language { get; set; }
}
