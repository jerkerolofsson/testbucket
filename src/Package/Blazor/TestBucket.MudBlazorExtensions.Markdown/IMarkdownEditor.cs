using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.MudBlazorExtensions.Markdown;
public interface IMarkdownEditor
{
    Task ToggleBoldAsync();
    Task ToggleItalicAsync();
    Task ToggleCodeBlockAsync();
    Task ToggleOrderedListAsync();
    Task ToggleUnorderedListAsync();
    Task DrawTableAsync();
    Task DrawLinkAsync();
    Task DrawImageAsync();

    Task TogglePreviewAsync();
    Task SetPreviewAsync(bool wantedState);
    Task ToggleFullScreenAsync();
    Task SetFullScreenAsync(bool wantedState);

    Task SetValueAsync(string value);
    Task<string?> GetValueAsync();

}
