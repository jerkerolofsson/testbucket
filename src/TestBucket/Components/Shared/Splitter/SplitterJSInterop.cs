using Microsoft.JSInterop;

namespace TestBucket.Components.Shared.Splitter;

public class SplitterJSInterop
{
    /// <summary>
    /// The js runtime
    /// </summary>
    private readonly IJSRuntime jsRuntime;

    public SplitterJSInterop(IJSRuntime JSRuntime)
    {
        jsRuntime = JSRuntime;
    }

    public async ValueTask Initialize(string elementId, object options)
    {
        await jsRuntime.InvokeVoidAsync("splitterInitialize", elementId, options);
    }
}
