using System;
using System.Xml.Linq;

using Microsoft.AspNetCore.Components.Forms;

using MudBlazor;
using MudBlazor.Utilities;

using TestBucket.MudBlazorExtensions.Markdown.Dialogs;

using static MudBlazor.Colors;

namespace TestBucket.MudBlazorExtensions.Markdown
{
    /// <summary>
    /// Markdown Editor
    /// </summary>
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
    public partial class MarkdownEditor : IDisposable
    {
        public const string FigmaIcon = "<svg viewBox=\"0 0 24 24\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\">\r\n<path opacity=\"0.6\" d=\"M11.6667 2H8.33333C6.49238 2 5 3.49238 5 5.33333C5 7.17428 6.49238 8.66667 8.33333 8.66667H11.6667V2Z\" fill=\"currentColor\"/>\r\n<path opacity=\"0.4\" d=\"M11.6667 8.6665H8.33333C6.49238 8.6665 5 10.1589 5 11.9998C5 13.8408 6.49238 15.3332 8.33333 15.3332H11.6667V8.6665Z\" fill=\"currentColor\"/>\r\n<path d=\"M18.3327 11.9998C18.3327 13.8408 16.8403 15.3332 14.9993 15.3332C13.1584 15.3332 11.666 13.8408 11.666 11.9998C11.666 10.1589 13.1584 8.6665 14.9993 8.6665C16.8403 8.6665 18.3327 10.1589 18.3327 11.9998Z\" fill=\"currentColor\"/>\r\n<path opacity=\"0.2\" d=\"M8.33333 15.3335H11.6667V18.6668C11.6667 20.5078 10.1743 22.0002 8.33333 22.0002C6.49238 22.0002 5 20.5078 5 18.6668C5 16.8259 6.49238 15.3335 8.33333 15.3335Z\" fill=\"currentColor\"/>\r\n<path opacity=\"0.8\" d=\"M11.666 2H14.9993C16.8403 2 18.3327 3.49238 18.3327 5.33333C18.3327 7.17428 16.8403 8.66667 14.9993 8.66667H11.666V2Z\" fill=\"currentColor\"/>\r\n</svg>";

        /// <summary>
        /// State of the editor, cascaded to the toolbar
        /// </summary>
        public MarkdownEditorState State { get; } = new MarkdownEditorState();

        /// <summary>
        /// Determines how .NET is updated from JS when the value is changing
        /// </summary>
        [Parameter] public ValueUpdateMode ValueUpdateMode { get; set; } = ValueUpdateMode.OnBlur;

        #region Mud

        /// <summary>
        /// CSS class for toolbar buttons. Default (rounded-0)
        /// </summary>
        [Parameter] public string ToolbarButtonClass { get; set; } = "rounded-0";
        [Parameter] public Color Color { get; set; } = Color.Surface;
        [Parameter] public Color ColorAlt { get; set; } = Color.Secondary;
        [Parameter] public Size IconSize { get; set; } = Size.Small;
        [Parameter] public bool ShowToolbar { get; set; } = true;
        [Parameter] public RenderFragment? ToolBarContent { get; set; }
        [Parameter] public RenderFragment? ToolBarStartContent { get; set; }
        [Parameter] public Variant Variant { get; set; } = Variant.Outlined;

        [Parameter(CaptureUnmatchedValues = true)] public IDictionary<string, object>? AdditionalAttributes { get; set; }
        protected string RootClassname =>
            new CssBuilder("mud-markdown-editor-root")
            .AddClass("mud-input-outlined", Variant == Variant.Outlined)
            .AddClass("mud-input-filled", Variant == Variant.Filled)
            .AddClass("mud-input-text", Variant == Variant.Text)
            .AddClass("mud-markdown-editor-root-preview", Preview == true)
            .Build();

        protected string EditButtonClassname =>
            new CssBuilder("mud-markdown-toolbar-toggle-button")
                .AddClass($"mud-{ColorAlt.ToDescriptionString()}-selected", Color != Color.Default && Color != Color.Inherit)
                .AddClass($"mud-markdown-toolbar-toggle-button-selected", Preview != true)
                .Build();

        protected string PreviewButtonClassname =>
            new CssBuilder("mud-markdown-toolbar-toggle-button")
                .AddClass($"mud-{ColorAlt.ToDescriptionString()}-selected", Color != Color.Default && Color != Color.Inherit)
                .AddClass($"mud-markdown-toolbar-toggle-button-selected", Preview == true)
                .AddClass(" mr-5", true)
                .Build();

        protected string ToolbarClassname =>
            new CssBuilder("mud-markdown-toolbar")
                .AddClass($"mud-{Color.ToDescriptionString()}-markdown-toolbar", Color != Color.Default && Color != Color.Inherit)
                .Build();

        #endregion
        #region Inject and JavaScript

        /// <summary>
        /// The dotnet object reference
        /// </summary>
        private DotNetObjectReference<MarkdownEditor>? dotNetObjectRef;

        /// <summary>
        /// Gets or sets the <see cref = "JSMarkdownInterop"/> instance.
        /// </summary>
        protected JSMarkdownInterop? JSModule { get; private set; }

        #endregion Inject and JavaScript
        #region Element references

        /// <summary>
        /// Gets or sets the element identifier.
        /// </summary>
        /// <value>
        /// The element identifier.
        /// </value>
        private string ElementId { get; set; } = $"markdown-{Guid.NewGuid()}";
        private string RootElementId => "root-" + ElementId;
        private string PreviewElementId { get; set; } = $"markdown-preview-{Guid.NewGuid()}";

        /// <summary>
        /// Gets or sets the element reference.
        /// </summary>
        /// <value>
        /// The element reference.
        /// </value>
        private ElementReference ElementRef { get; set; }

        #endregion Element references
        #region Local variable

        /// <summary>
        /// Percentage of the current file-read status.
        /// </summary>
        protected double Progress;

        /// <summary>
        /// Number of processed bytes in current file.
        /// </summary>
        protected long ProgressProgress;

        /// <summary>
        /// Total number of bytes in currently processed file.
        /// </summary>
        protected long ProgressTotal;

        /// <summary>
        /// Indicates if markdown editor is properly initialized.
        /// </summary>
        protected bool Initialized { get; set; }

        /// <inheritdoc/>
        protected bool ShouldAutoGenerateId => true;

        #endregion Local variable
       
        #region Event Callback

        /// <summary>
        /// Occurs after the custom toolbar button is clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MarkdownButtonEventArgs> CustomButtonClicked { get; set; }

        /// <summary>
        /// An event that occurs after the markdown value has changed.
        /// </summary>
        [Parameter] public EventCallback<string> ValueChanged { get; set; }

        [Parameter] public EventCallback OnSave { get; set; }

        [Parameter] public EventCallback<RunCodeRequest> RunCode { get; set; }

        /// <summary>
        /// A space delimited list of code languages to show a run icon for 
        /// </summary>
        [Parameter] public string? RunCodeLanguages { get; set; }

        #endregion Event Callback

        #region Parameters
        [Parameter] public bool CanToggleEdit { get; set; } = true;
        [Parameter] public bool ShowSaveButton { get; set; } = false;
        [Parameter] public string Style { get; set; } = "";

        /// <summary>
        /// Adds a button to run code embedded in the markdown
        /// </summary>
        [Parameter] public bool EnableRunCode { get; set; }

        /// <summary>
        /// Adds a button to run code embedded in the markdown
        /// </summary>
        [Parameter] public bool EnableCopyCodeToClipboard { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the textarea where the component is hosted can be resizable.
        /// </summary>
        /// <value><c>null</c> if it contains no value, <c>true</c> if the textarea has to be resized by the user; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool? AllowResize { get; set; }

        /// <summary>
        /// If set to true, force downloads Font Awesome (used for icons). If set to false, prevents downloading.
        /// </summary>
        [Parameter]
        public bool? AutoDownloadFontAwesome { get; set; }

        /// <summary>
        /// Gets or sets the characters status text.
        /// </summary>
        /// <value>The characters status text.</value>
        [Parameter]
        public string? CharactersStatusText { get; set; } = "characters: ";

        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        /// <value>The CSS class.</value>
        [Parameter]
        public string? TextAreaClass { get; set; }

        /// <summary>
        /// rtl or ltr. Changes text direction to support right-to-left languages. Defaults to ltr.
        /// </summary>
        [Parameter]
        public string Direction { get; set; } = "ltr";

        /// <summary>
        /// A callback function used to define how to display an error message. Defaults to (errorMessage) => alert(errorMessage).
        /// </summary>
        [Parameter]
        public Func<string, Task>? ErrorCallback { get; set; }

        /// <summary>
        /// Errors displayed to the user, using the errorCallback option, where #image_name#, #image_size#
        /// and #image_max_size# will replaced by their respective values, that can be used for customization
        /// or internationalization.
        /// </summary>
        [Parameter]
        public MarkdownErrorMessages? ErrorMessages { get; set; }

        /// <summary>
        /// An array of icon names to hide. Can be used to hide specific icons shown by default without
        /// completely customizing the toolbar.
        /// </summary>
        [Parameter]
        public string[] HideIcons { get; set; } = new[] { "side-by-side", "fullscreen" };

        /// <summary>
        /// A comma-separated list of mime-types used to check image type before upload (note: never trust client, always
        /// check file types at server-side). Defaults to image/png, image/jpeg.
        /// </summary>
        [Parameter]
        public string ImageAccept { get; set; } = "image/png, image/jpeg, image/jpg, image.gif";

        /// <summary>
        /// Handler that should save the image uploaded by the user and return an URL to the image
        /// </summary>
        [Parameter] public ImageUploadHandler? UploadHandler { get; set; }

        /// <summary>
        /// CSRF token to include with AJAX call to upload image. For instance used with Django backend.
        /// </summary>
        [Parameter]
        public string? ImageCSRFToken { get; set; }

        /// <summary>
        /// Maximum image size in bytes, checked before upload (note: never trust client, always check image
        /// size at server-side). Defaults to 1024*1024*2 (2Mb).
        /// </summary>
        [Parameter]
        public long ImageMaxSize { get; set; } = 1024 * 1024 * 2;

        /// <summary>
        /// If set to true, enables line numbers in the editor.
        /// </summary>
        [Parameter]
        public bool LineNumbers { get; set; }

        /// <summary>
        /// If set to false, disable line wrapping. Defaults to true.
        /// </summary>
        [Parameter]
        public bool LineWrapping { get; set; } = true;

        /// <summary>
        /// Gets or sets the lines status text.
        /// </summary>
        /// <value>The lines status text.</value>
        [Parameter]
        public string? LinesStatusText { get; set; } = "lines: ";

        /// <summary>
        /// Gets or sets the markdown explanation URL.
        /// </summary>
        /// <value>The markdown explanation URL.</value>
        [Parameter]
        public string? MarkdownUrl { get; set; }

        /// <summary>
        /// Sets fixed height for the composition area. minHeight option will be ignored.
        /// Should be a string containing a valid CSS value like "500px". Defaults to undefined.
        /// </summary>
        [Parameter]
        public string? MaxHeight { get; set; }

        /// <summary>
        /// Gets or sets the max chunk size when uploading the file.
        /// </summary>
        [Parameter]
        public int MaxUploadImageChunkSize { get; set; } = 20 * 1024;

        /// <summary>
        /// Gets or sets a value indicating whether native spell checker is enable or not
        /// </summary>
        /// <value>
        ///   <c>true</c> if [native spell checker]; otherwise, <c>false</c>.
        /// </value>
        [Parameter]
        public bool NativeSpellChecker { get; set; } = true;

        /// <summary>
        /// Sets the minimum height for the composition area, before it starts auto-growing.
        /// Should be a string containing a valid CSS value like "500px". Defaults to "300px".
        /// </summary>
        [Parameter]
        public string? MinHeight { get; set; }

        /// <summary>
        /// If set, displays a custom placeholder message.
        /// </summary>
        [Parameter]
        public string? Placeholder { get; set; }

        /// <summary>
        /// Gets or sets the Segment Fetch Timeout when uploading the file.
        /// </summary>
        [Parameter]
        public TimeSpan SegmentFetchTimeout { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// An array of icon names to show. Can be used to show specific icons hidden by default without
        /// completely customizing the toolbar.
        /// </summary>
        [Parameter]
        public string[] ShowIcons { get; set; } = new[] { "code", "table" };

        /// <summary>
        /// Gets or sets a value indicating whether spell checker is enable or nor
        /// </summary>
        /// <value>
        ///   <c>true</c> if [spell checker]; otherwise, <c>false</c>.
        /// </value>
        [Parameter]
        public bool SpellChecker { get; set; } = true;

        /// <summary>
        /// Gets or sets a value controlling whether the preview should be enabled by default (default off).
        /// </summary>
        [Parameter] public bool Preview { get; set; } = false;

        [Parameter] public EventCallback<bool> PreviewChanged { get; set; }

        /// <summary>
        /// If set, customize the tab size. Defaults to 2.
        /// </summary>
        [Parameter]
        public int TabSize { get; set; } = 2;

        /// <summary>
        /// Override the theme. Defaults to easymde.
        /// </summary>
        [Parameter]
        public string Theme { get; set; } = "easymde";

        /// <summary>
        /// [Optional] Gets or sets the content of the toolbar.
        /// </summary>
        [Parameter]
        public RenderFragment? Toolbar { get; set; }

        /// <summary>
        /// If set to false, disable toolbar button tips. Defaults to true.
        /// </summary>
        [Parameter]
        public bool ToolbarTips { get; set; } = true;

        /// <summary>
        /// If set to true, enables the image upload functionality, which can be triggered by drag-drop,
        /// copy-paste and through the browse-file window (opened when the user click on the upload-image icon).
        /// Defaults to false.
        /// </summary>
        [Parameter]
        public bool UploadImage { get; set; }

        /// <summary>
        /// Gets or sets the markdown value.
        /// </summary>
        [Parameter]
        public string? Value { get; set; }
        private string? _value = null;

        /// <summary>
        /// Gets or sets the words status text.
        /// </summary>
        /// <value>The words status text.</value>
        [Parameter]
        public string? WordsStatusText { get; set; } = "words: ";

        #endregion Parameters
        #region JSInvokable

        private string? _prevValueBeforeUpdate = null;

        /// <summary>
        /// Updates the internal markdown value. This method should only be called internally!
        /// </summary>
        /// <param name="value">New value.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        [JSInvokable]
        public async Task UpdateInternalValue(string value)
        {
            if (_value != value)
            {
                _value = value;
                await ValueChanged.InvokeAsync(value);
            }
        }

        [JSInvokable]
        public async Task RunCodeInternal(string language, string code)
        {
            await RunCode.InvokeAsync(new RunCodeRequest() { Code = code, Language= language});
        }

        /// <summary>
        /// Updates the internal markdown value. This method should only be called internally!
        /// </summary>
        /// <param name="value">New value.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        [JSInvokable]
        public Task BeginAppendInternalValue()
        {
            _prevValueBeforeUpdate = _value;
            _value = "";
            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates the internal markdown value. This method should only be called internally!
        /// </summary>
        /// <param name="value">New value.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        [JSInvokable]
        public Task AppendInternalValue(string value)
        {
            _value += value;
            return Task.CompletedTask;
        }

        [JSInvokable]
        public async Task EndAppendInternalValue()
        {
            if(_value != _prevValueBeforeUpdate)
            {
                await ValueChanged.InvokeAsync(_value);
            }
            _prevValueBeforeUpdate = "";
        }


        /// <summary>
        /// Notifies the error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        [JSInvokable]
        public Task NotifyErrorMessage(string errorMessage)
        {
            if (ErrorCallback is not null)
                return ErrorCallback.Invoke(errorMessage);

            return Task.CompletedTask;
        }


        #endregion JSInvokable
        #region Public Methods

        /// <summary>
        /// Inserts an image
        /// </summary>
        public async Task DrawImageAsync()
        {
            if (!Initialized || JSModule is null)
            {
                return;
            }

            await JSModule.DrawImage(ElementId);
        }

        /// <summary>
        /// Inserts an image
        /// </summary>
        public async Task UploadImageAsync(IBrowserFile browserFile)
        {
            if (UploadHandler is null)
            {
                return;
            }

            // Now we need to save the image, and get an URL in return so we can insert it
            var url = await UploadHandler.UploadImageAsync(browserFile, ImageMaxSize);

            if (!Initialized || JSModule is null)
            {
                return;
            }
            var name = browserFile.Name;
            await JSModule.InsertText(ElementId, $"![{name}]({url})");
        }

        /// <summary>
        /// Inserts a link
        /// </summary>
        public async Task DrawLinkAsync()
        {
            if (!Initialized || JSModule is null)
            {
                return;
            }
            await JSModule.DrawLink(ElementId);
        }

        /// <summary>
        /// Inserts a table
        /// </summary>
        public async Task DrawTableAsync()
        {
            if (!Initialized || JSModule is null)
            {
                return;
            }
            await JSModule.DrawTable(ElementId);
        }

        /// <summary>
        /// Inserts a unordered list (ul)
        /// </summary>
        public async Task ToggleUnorderedListAsync()
        {
            if (!Initialized || JSModule is null)
            {
                return;
            }
            await JSModule.ToggleUnorderedList(ElementId);
        }
        /// <summary>
        /// Inserts a ordered list (ol)
        /// </summary>
        public async Task ToggleOrderedListAsync()
        {
            if (!Initialized || JSModule is null)
            {
                return;
            }
            await JSModule.ToggleOrderedList(ElementId);
        }

        /// <summary>
        /// Toggle code block
        /// </summary>
        public async Task ToggleCodeBlockAsync()
        {
            if (!Initialized || JSModule is null)
            {
                return;
            }
            await JSModule.ToggleCodeBlock(ElementId);
        }

        /// <summary>
        /// Toggles italic
        /// </summary>
        public async Task ToggleItalicAsync()
        {
            if (!Initialized || JSModule is null)
            {
                return;
            }
            await JSModule.ToggleItalic(ElementId);
        }

        /// <summary>
        /// Toggles bold
        /// </summary>
        public async Task ToggleBoldAsync()
        {
            if (!Initialized || JSModule is null)
            {
                return;
            }
            await JSModule.ToggleBold(ElementId);
        }


        /// <summary>
        /// Sets fullscreen
        /// </summary>
        public async Task SetFullScreenAsync(bool isfullscreen)
        {
            if (!Initialized || JSModule is null)
            {
                return;
            }
            await JSModule.SetFullScreen(RootElementId, isfullscreen);
            State.Fullscreen = isfullscreen;
        }

        /// <summary>
        /// Toggles fullscreen
        /// </summary>
        public async Task ToggleFullScreenAsync()
        {
            if (!Initialized || JSModule is null)
            {
                return;
            }
            if(State.Fullscreen == true)
            {
                await JSModule.SetFullScreen(RootElementId, false);
                State.Fullscreen = false;
            }
            else
            {
                await JSModule.SetFullScreen(RootElementId, true);
                State.Fullscreen = true;
            }
        }

        public async Task SaveAsync()
        {
            await OnSave.InvokeAsync();
        }

        /// <summary>
        /// Toggles preview mode
        /// </summary>
        public async Task TogglePreviewAsync()
        {
            if (State.Preview == true)
            {
                await SetPreviewAsync(false);
            }
            else
            {
                await SetPreviewAsync(true);
            }
        }

        /// <summary>
        /// Enables or disables the preview
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        public async Task SetPreviewAsync(bool wantedState)
        {
            if (!Initialized || JSModule is null)
            {
                return;
            }

            await JSModule.SetPreview(ElementId, wantedState);

            // Keep track of the state to support changing the state with the Preview parameter
            if (State.Preview != wantedState)
            {
                State.Preview = wantedState;
                await PreviewChanged.InvokeAsync(wantedState);
            }
        }

        /// <summary>
        /// Sets the markdown value.
        /// </summary>
        /// <param name = "value">Value to set.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetValueAsync(string value)
        {
            if (!Initialized || JSModule is null)
            { 
                return;
            }

            await JSModule.SetValue(ElementId, value);
        }

        /// <summary>
        /// Gets the markdown value.
        /// </summary>
        /// <returns>Markdown value.</returns>
        public async Task<string?> GetValueAsync()
        {
            if (!Initialized || JSModule is null)
            {
                return null;
            }

            return await JSModule.GetValue(ElementId);
        }

        #endregion

        private async Task AddFigmaEmbedding()
        {
            var url = "https://embed.figma.com/design/nrPSsILSYjesyc5UHjYYa4?embed-host=figma-embed-docs";

            var parameters = new DialogParameters<EnterUrlDialog>
            {
                { x => x.Url, url },
            };
            var dialog = await dialogService.ShowAsync<EnterUrlDialog>(null, parameters);
            var result = await dialog.Result;

            if (result?.Data is string userUrl && JSModule is not null)
            {
                var markdown = $"""
                    ```figma
                    {url}
                    ```
                    """;

                await JSModule.InsertText(ElementId, markdown);
            }
        }
        private async Task AddYoutubeEmbedding()
        {
            var url = "https://www.youtube.com/embed/awztkr8n0AA?si=m4B3SnjkMB9NMpwN";

            var parameters = new DialogParameters<EnterUrlDialog>
            {
                { x => x.Url, url },
            };
            var dialog = await dialogService.ShowAsync<EnterUrlDialog>(null, parameters);
            var result = await dialog.Result;

            if (result?.Data is string userUrl && JSModule is not null)
            {
                var markdown = $"""
                    ```video
                    {url}
                    ```
                    """;

                await JSModule.InsertText(ElementId, markdown);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            // Save initial state
            if (State.Preview is null)
            {
                State.Preview = Preview;
            }
            //if (_value is null)
            //{
            //    _value = Value;
            //}

            if (Initialized && _value != Value)
            {
                await SetValueAsync(Value ?? "");
                _value = Value;
            }

            if (Initialized && State.Preview != Preview)
            {
                await SetPreviewAsync(Preview);
            }
        }

        /// <summary>
        /// Method invoked after each time the component has been rendered. Note that the component does
        /// not automatically re-render after the completion of any returned <see cref="T:System.Threading.Tasks.Task" />, because
        /// that would cause an infinite render loop.
        /// </summary>
        /// <param name="firstRender">Set to <c>true</c> if this is the first time <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> has been invoked
        /// on this component instance; otherwise <c>false</c>.</param>
        /// <remarks>
        /// The <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> and <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRenderAsync(System.Boolean)" /> lifecycle methods
        /// are useful for performing interop, or interacting with values received from <c>@ref</c>.
        /// Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
        /// once.
        /// </remarks>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                JSModule ??= new JSMarkdownInterop(JSRuntime);
                dotNetObjectRef ??= DotNetObjectReference.Create(this);

                //string Value = this.Value;

                _value = this.Value;
                string initialValue = this.Value ?? "";
                bool useChunkedSetValue = false;
                if(initialValue.Length > 30_000)
                {
                    useChunkedSetValue = true;
                    initialValue = "";
                }

                string? minHeight = MinHeight;
                string? maxHeight = MaxHeight;

                await JSModule.Initialize(dotNetObjectRef, ElementRef, ElementId, PreviewElementId, new
                {
                    AutoSave = new
                    {
                        enabled = false,
                    },
                    Value = initialValue,
                    AutoDownloadFontAwesome,
                    HideIcons,
                    ShowIcons,
                    LineNumbers,
                    LineWrapping,
                    MarkdownUrl,
                    minHeight,
                    maxHeight,
                    Placeholder,
                    TabSize,
                    Theme,
                    Direction,
                    NativeSpellChecker,
                    SpellChecker,
                    StatusTexts = new
                    {
                        Characters = CharactersStatusText,
                        Lines = LinesStatusText,
                        Words = WordsStatusText,
                    },
                    ToolbarTips,
                    ImageCSRFToken,
                    EnableRunCode,
                    RunCodeLanguages,
                    EnableCopyCodeToClipboard,
                    ValueUpdateMode,
                    ErrorMessages,
                    Preview,
                });

                if (AllowResize != null && (bool)AllowResize)
                {
                    TextAreaClass ??= "";
                    TextAreaClass += " resizable";
                    await JSModule.AllowResize(ElementId);
                }

                Initialized = true;

                if (useChunkedSetValue)
                {
                    await SetValueAsync(this.Value??"");
                }
            }
            else if(Initialized)
            {
                if(_value != Value)
                {
                    await SetValueAsync(this.Value ?? "");
                    _value = this.Value;
                }
            }
        }

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            if (JSModule == null)
            {
                JSModule = new JSMarkdownInterop(JSRuntime);
            }


            base.OnInitialized();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (ElementId is not null)
            {
                JSModule?.Destroy(ElementRef, ElementId);
            }
        }
    }
}