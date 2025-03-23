using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;

using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace TestBucket.Components.Shared.Splitter
{
    /// <summary>
    /// Split two panels with a bar.
    /// </summary>
    public partial class Splitter : MudComponentBase
    {

        private readonly Guid _guid = Guid.NewGuid();

        protected string ElementId => "tb-element-" + _guid.ToString();

        protected ElementReference? ElementRef;

        /// <summary>
        /// 
        /// </summary>
        protected string? Classname => new CssBuilder("splitter")
            .AddClass("horizontal", !Vertical)
            .AddClass("vertical", Vertical)
            .AddClass($"tb-splitter-{ElementId}")
            .AddClass(Class)
            .Build();

        protected string? StartContentClassname => new CssBuilder($"tb-splitter-content tb-splitter-content-start tb-splitter-content-{ElementId} d-flex")
                   .AddClass("ma-2", EnableMargin)
                   .AddClass(ClassContent)
                   .Build();

        protected string? EndContentClassname => new CssBuilder($"tb-splitter-content tb-splitter-content-end tb-splitter-content-{ElementId} d-flex")
                   .AddClass("ma-2", EnableMargin)
                   .AddClass(ClassContent)
                   .Build();

        protected string BarStyle
        {
            get
            {
                StringBuilder css = new StringBuilder();

                css.Append($"--bar-color: {EffectiveColor};");
                if(BarSize is not null)
                {
                    css.Append($"--bar-size: {BarSize}");
                }

                return css.ToString();
            }
        }

        /// <summary>
        /// The two contents' (sections) classes, seperated by space.
        /// </summary>
        [Parameter]
        public string? ClassContent { get; set; }

        /// <summary>
        /// Vertical or horizontal
        /// 
        /// Horizontal
        /// [Start] | [End]
        /// 
        /// Vertical
        /// [Start]
        /// ------
        /// [End]
        /// 
        /// </summary>
        [Parameter] public bool Vertical { get; set; } = false;

        /// <summary>
        /// The width of splitter. For example: "400px"
        /// </summary>
        /// <remarks>The default is 100%</remarks>
        [Parameter]
        public string? Width { get; set; }

        /// <summary>
        /// The height of splitter. For example: "400px"
        /// </summary>
        /// <remarks>The default is 100%</remarks>
        [Parameter]
        public string? Height { get; set; }

        /// <summary>
        /// The color
        /// </summary>
        [Parameter]
        public Color Color { get; set; }

        /// <summary>
        /// If true, splitter has borders.
        /// </summary>
        [Parameter]
        public bool Bordered { get; set; }

        /// <summary>
        /// The style to apply to both content sections, seperated by space.
        /// </summary>
        [Parameter]
        public string? ContentStyle { get; set; }

        /// <summary>
        /// The style of the <see cref="StartContent"/>, seperated by space.
        /// </summary>
        [Parameter]
        public string? StartContentStyle { get; set; }

        /// <summary>
        /// The style of the <see cref="EndContent"/>, seperated by space.
        /// </summary>
        [Parameter]
        public string? EndContentStyle { get; set; }

        /// <summary>
        /// The splitter bar's width
        /// </summary>
        /// <remarks>The default is 2px</remarks>
        [Parameter]
        public string? BarSize { get; set; }

        /// <summary>
        /// Enables the default margin.
        /// </summary>
        /// <remarks>The default is true, which adds class: "ma-2"</remarks>
        [Parameter]
        public bool EnableMargin { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public RenderFragment? StartContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public RenderFragment? EndContent { get; set; }

        /// <summary>
        /// The start content's initial percentage.
        /// Default is 50.
        /// </summary>
        /// <remarks>The default is 50</remarks>
        [Parameter]
        public double Dimension { get; set; } = 50;

        string? EffectiveStartStyle { get { return !string.IsNullOrWhiteSpace(StartContentStyle) ? StartContentStyle : ContentStyle; } }
        string? EffectiveEndStyle { get { return !string.IsNullOrWhiteSpace(EndContentStyle) ? EndContentStyle : ContentStyle; } }
        string? EffectiveColor { get { return $"var(--mud-palette-{(Color == Color.Default ? "action-default" : Color.ToDescriptionString())}"; } }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                string direction = Vertical ? "vertical" : "horizontal";

                var interop = new SplitterJSInterop(js);
                await interop.Initialize(ElementId, new
                {
                    dimension = Dimension + "%",
                    direction,
                });
            }
        }
    }
}