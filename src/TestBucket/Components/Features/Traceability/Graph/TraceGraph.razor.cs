using Microsoft.JSInterop;

using TestBucket.Components.Requirements;
using TestBucket.Components.Requirements.Services;
using TestBucket.Components.Tests.Services;
using TestBucket.Domain.Traceability.Models;

namespace TestBucket.Components.Features.Traceability.Graph;

public partial class TraceGraph
{
    private bool _initialized = false;
    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<ResourcesInterop>? _resourcesInteropReference;

    [Inject] RequirementEditorController? _requirementEditor { get; set; }

    [Inject] IJSRuntime JS { get; set; } = default!;

    [Parameter] public TraceabilityNode? Node { get; set; }

    private class ResourcesInterop(TraceGraph Graph)
    {
        [JSInvokable]
        public async Task SelectResource(string id)
        {
            if(Graph._idMap.TryGetValue(id, out var node))
            {
                if(node.Requirement is not null && Graph._requirementEditor is not null)
                {
                    await Graph._requirementEditor.OpenEditDialogAsync(node.Requirement);
                }
            }
        }

        [JSInvokable]
        public Task ResourceContextMenu(string id, int screenWidth, int screenHeight, int clientX, int clientY)
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

            _jsModule = await JS.InvokeAsync<IJSObjectReference>("import", "/js/app-tracegraph.js");

            _resourcesInteropReference = DotNetObjectReference.Create(new ResourcesInterop(this));

            await _jsModule.InvokeVoidAsync("initializeNodes", _resourcesInteropReference);
            await UpdateNodesAsync();
            //await UpdateResourceGraphSelectedAsync();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await UpdateNodesAsync();
    }

    private async Task UpdateNodesAsync()
    {
        if (_jsModule is null)
        {
            return;
        }

        List<NodeDto> nodes = [];
        _idMap.Clear();
        if (Node is not null)
        {
            FlattenAsDto(nodes, Node, null);
            await _jsModule.InvokeVoidAsync("updateGraph", nodes);
        }
    }

    private readonly Dictionary<string, TraceabilityNode> _idMap = [];

    private void FlattenAsDto(List<NodeDto> dest, TraceabilityNode node, TraceabilityNode? parent)
    {
        var dto = NodeDto.Create(node, parent);
        _idMap[dto.Id] = node;
        if (!dest.Contains(dto))
        {
            dest.Add(dto);
            foreach (var down in node.Downstream)
            {
                FlattenAsDto(dest, down, node);
            }
        }
       
        //foreach (var up in node.Upstream)
        //{
        //    FlattenAsDto(dest, up, node);
        //}
    }
}
