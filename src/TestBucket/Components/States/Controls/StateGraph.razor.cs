using Microsoft.JSInterop;

using TestBucket.Contracts.States;

namespace TestBucket.Components.States.Controls;

public partial class StateGraph
{
    private bool _initialized = false;
    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<StateGraphInterop>? _interopReference;

    [Inject] IJSRuntime JS { get; set; } = default!;

    [Parameter] public IReadOnlyList<BaseState>? States { get; set; }

    //private class StateGraphInterop(StateGraph Graph)
    private class StateGraphInterop()
    {
        [JSInvokable]
        public Task SelectState(string id)
        {
            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task StateContextMenu(string id, int screenWidth, int screenHeight, int clientX, int clientY)
        {
            return Task.CompletedTask;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_initialized)
        {
            // Before any awaits, set a flag to indicate the graph is initialized. This prevents the graph being initialized multiple times.
            _initialized = true;

            _jsModule = await JS.InvokeAsync<IJSObjectReference>("import", "/js/app-stategraph.js");

            _interopReference = DotNetObjectReference.Create(new StateGraphInterop());

            await _jsModule.InvokeVoidAsync("initializeStates", _interopReference);
            await UpdateGraphAsync();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await UpdateGraphAsync();
    }

    private async Task UpdateGraphAsync()
    {
        if (_jsModule is null)
        {
            return;
        }

        if (States is not null)
        {
            await _jsModule.InvokeVoidAsync("updateGraph", States);
        }
    }
}
