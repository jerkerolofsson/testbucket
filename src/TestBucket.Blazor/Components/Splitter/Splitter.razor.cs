using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using TestBucket.Contracts.Appearance.Models;

namespace TestBucket.Blazor.Components.Splitter
{
    /// <summary>
    /// Split two panels with a bar.
    /// </summary>
    public partial class Splitter : MudComponentBase
    {
        private readonly Guid _guid = Guid.NewGuid();

        protected string ElementId => "tb-element-" + _guid.ToString();

        protected ElementReference? ElementRef;

        private bool IsVertical
        {
            get
            {
                if (Dock is not null)
                {
                    return Dock is Contracts.Appearance.Models.Dock.Top or Contracts.Appearance.Models.Dock.Bottom;
                }
                return Vertical;
            }
        }
        private bool IsHorizontal
        {
            get
            {
                if (Dock is not null)
                {
                    return Dock is Contracts.Appearance.Models.Dock.Left or Contracts.Appearance.Models.Dock.Right;
                }
                return !Vertical;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string? Classname => new CssBuilder("splitter")
            .AddClass("horizontal", IsHorizontal)
            .AddClass("vertical", IsVertical)
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
                if (BarSize is not null)
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
        /// If Dock is set to Right or Bottom, the Start and End components are switched
        /// If dock is set, the Vertical property is ignored
        /// </summary>
        [Parameter] public Dock? Dock { get; set; }

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

        /// <summary>
        /// Returns the dimension which depends on the dock
        /// </summary>
        /// <returns></returns>
        private double GetDimension()
        {
            if(Dock is Contracts.Appearance.Models.Dock.Bottom or Contracts.Appearance.Models.Dock.Right)
            {
                return 100 - Dimension;
            }
            return Dimension;
        }

        string? EffectiveStartStyle { get { return !string.IsNullOrWhiteSpace(StartContentStyle) ? StartContentStyle : ContentStyle; } }
        string? EffectiveEndStyle { get { return !string.IsNullOrWhiteSpace(EndContentStyle) ? EndContentStyle : ContentStyle; } }
        string? EffectiveColor { get { return $"var(--mud-palette-{(Color == Color.Default ? "action-default" : Color.ToDescriptionString())})"; } }

        private Dock? _prevDock = null;
        private bool _isJsInit = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_isJsInit || _prevDock != Dock)
            {
                var interop = new SplitterJSInterop(js);
                if (_isJsInit)
                {
                    await interop.Destroy(ElementId);
                }

                _isJsInit = true;
                _prevDock = Dock;
                string direction = IsVertical ? "vertical" : "horizontal";

                await interop.Initialize(ElementId, new
                {
                    dimension = GetDimension() + "%",
                    direction,
                });
            }
        }
    }
}