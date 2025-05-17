using Microsoft.JSInterop;

namespace TestBucket.Blazor.Components.Splitter;

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
    public async ValueTask Destroy(string elementId)
    {
        await jsRuntime.InvokeVoidAsync("splitterDestroy", elementId);
    }
}
