using MudBlazor;
using MudBlazor.Utilities;

using static MudBlazor.Colors;

namespace PSC.Blazor.Components.MarkdownEditor
{
    /// <summary>
    /// Markdown Editor
    /// </summary>
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
    public partial class MarkdownEditor : IDisposable
    {
        #region Mud
        [Parameter] public Color Color { get; set; } = Color.Primary;
        [Parameter] public Size IconSize { get; set; } = Size.Medium;

        [Parameter] public bool ShowToolbar { get; set; } = true;

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
        /// The toolbar buttons
        /// </summary>
        private List<MarkdownToolbarButton>? toolbarButtons;

        /// <summary>
        /// Indicates if markdown editor is properly initialized.
        /// </summary>
        protected bool Initialized { get; set; }

        /// <inheritdoc/>
        protected bool ShouldAutoGenerateId => true;

        /// <summary>
        /// Keep track of the preview state
        /// </summary>
        private bool? _currentPreviewState = null;

        #endregion Local variable
        #region AutoSave

        /// <summary>
        /// Gets or sets the automatic save delay.
        /// Delay between saves, in milliseconds. Defaults to 10000 (10s).
        /// </summary>
        /// <value>
        /// The automatic save delay.
        /// </value>
        [Parameter]
        public int AutoSaveDelay { get; set; } = 10000;

        /// <summary>
        /// Gets or sets the setting for the auto save.
        /// Saves the text that's being written and will load it back in the future.
        /// It will forget the text when the form it's contained in is submitted.
        /// Recommended to choose a unique ID for the Markdown Editor.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the automatic save is enabled; otherwise, <c>false</c>.
        /// </value>
        [Parameter]
        public bool AutoSaveEnabled { get; set; }

        /// <summary>
        /// Gets or sets the automatic save identifier.
        /// You must set a unique string identifier so that EasyMDE can autosave.
        /// Something that separates this from other instances of EasyMDE elsewhere on your website.
        /// </summary>
        /// <value>
        /// The automatic save identifier.
        /// </value>
        [Parameter]
        public string? AutoSaveId { get; set; }

        /// <summary>
        /// Gets or sets the automatic save submit delay.
        /// Delay before assuming that submit of the form failed and saving the text, in milliseconds.
        /// </summary>
        /// <value>
        /// The automatic save submit delay.
        /// </value>
        [Parameter]
        public int AutoSaveSubmitDelay { get; set; } = 5000;

        /// <summary>
        /// Gets or sets the automatic save text.
        /// </summary>
        /// <value>
        /// The automatic save text.
        /// </value>
        [Parameter]
        public string AutoSaveText { get; set; } = "Autosaved: ";

        /// <summary>
        /// Gets or sets the automatic save time format day.
        /// </summary>
        /// <value>
        /// The automatic save time format day.
        /// </value>
        [Parameter]
        public string AutoSaveTimeFormatDay { get; set; } = "2-digit";

        /// <summary>
        /// Gets or sets the automatic save time format hour.
        /// </summary>
        /// <value>
        /// The automatic save time format hour.
        /// </value>
        [Parameter]
        public string AutoSaveTimeFormatHour { get; set; } = "2-digit";

        /// <summary>
        /// Gets or sets the automatic save time format locale.
        /// </summary>
        /// <value>
        /// The automatic save time format locale.
        /// </value>
        [Parameter]
        public string AutoSaveTimeFormatLocale { get; set; } = "en-US";

        /// <summary>
        /// Gets or sets the automatic save time format minute.
        /// </summary>
        /// <value>
        /// The automatic save time format m inute.
        /// </value>
        [Parameter]
        public string AutoSaveTimeFormatMinute { get; set; } = "2-digit";

        /// <summary>
        /// Gets or sets the automatic save time format month.
        /// </summary>
        /// <value>
        /// The automatic save time format month.
        /// </value>
        [Parameter]
        public string AutoSaveTimeFormatMonth { get; set; } = "long";

        /// <summary>
        /// Gets or sets the automatic save time format year.
        /// </summary>
        /// <value>
        /// The automatic save time format year.
        /// </value>
        [Parameter]
        public string AutoSaveTimeFormatYear { get; set; } = "numeric";
        #endregion AutoSave
        #region Event Callback

        /// <summary>
        /// Occurs after the custom toolbar button is clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MarkdownButtonEventArgs> CustomButtonClicked { get; set; }

        /// <summary>
        /// An event that occurs after the markdown value has changed.
        /// </summary>
        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<RunCodeRequest> RunCode { get; set; }

        #endregion Event Callback
        #region Parameters

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
        public string? CSSClass { get; set; }

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
        /// If set to true, will treat imageUrl from imageUploadFunction and filePath returned from imageUploadEndpoint as
        /// an absolute rather than relative path, i.e. not prepend window.location.origin to it.
        /// </summary>
        [Parameter]
        public string? ImagePathAbsolute { get; set; }

        /// <summary>
        /// Texts displayed to the user (mainly on the status bar) for the import image feature, where
        /// #image_name#, #image_size# and #image_max_size# will replaced by their respective values, that
        /// can be used for customization or internationalization.
        /// </summary>
        [Parameter]
        public MarkdownImageTexts? ImageTexts { get; set; }

        /// <summary>
        /// Occurs every time the selected image has changed.
        /// </summary>
        [Parameter]
        public Func<FileChangedEventArgs, Task>? ImageUploadChanged { get; set; }

        /// <summary>
        /// Occurs when an individual image upload has ended.
        /// </summary>
        [Parameter]
        public Func<FileEndedEventArgs, Task>? ImageUploadEnded { get; set; }

        /// <summary>
        /// The endpoint where the images data will be sent, via an asynchronous POST request. The server is supposed to
        /// save this image, and return a json response.
        /// </summary>
        [Parameter]
        public string? ImageUploadEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the image upload authentication schema (for example Bearer)
        /// </summary>
        /// <value>
        /// The image upload authentication schema by default is empty
        /// </value>
        [Parameter]
        public string? ImageUploadAuthenticationSchema { get; set; }

        /// <summary>
        /// Gets or sets the image upload authentication token.
        /// </summary>
        /// <value>
        /// The image upload authentication token by default is empty
        /// </value>
        [Parameter]
        public string? ImageUploadAuthenticationToken { get; set; }

        /// <summary>
        /// Notifies the progress of image being written to the destination stream.
        /// </summary>
        [Parameter]
        public Func<FileProgressedEventArgs, Task>? ImageUploadProgressed { get; set; }

        /// <summary>
        /// Occurs when an individual image upload has started.
        /// </summary>
        [Parameter]
        public Func<FileStartedEventArgs, Task>? ImageUploadStarted { get; set; }

        [Parameter]
        public Func<MarkdownEditor, FileEntry, Task<FileEntry>>? CustomImageUpload { get; set; }

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
        public string MaxHeight { get; set; } = "300px";

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
        public string MinHeight { get; set; } = "300px";

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
        /// Uploads the file.
        /// </summary>
        /// <param name="fileInfo">The file information.</param>
        [JSInvokable]
        public async Task UploadFile(FileEntry fileInfo)
        {
            bool success = false;
            if (JSModule is null)
            {
                return;
            }

            if (fileInfo is null || string.IsNullOrEmpty(fileInfo.ContentBase64))
            {
                await JSModule.NotifyImageUploadError(ElementId, $"Can't upload an empty file");
                return;
            }

            if (string.IsNullOrEmpty(ImageUploadEndpoint))
            {
                await JSModule.NotifyImageUploadError(ElementId, $"The property ImageUploadEndpoint is not specified.");
                return;
            }

            if (ImageUploadStarted is not null)
            {
                await ImageUploadStarted.Invoke(new(fileInfo));
            }

            if (CustomImageUpload is not null)
            {
                try
                {
                    fileInfo = await CustomImageUpload.Invoke(this, fileInfo);

                    success = true;
                }
                catch (Exception ex)
                {
                    fileInfo.ErrorMessage = ex.Message;
                }

                if (ImageUploadProgressed is not null)
                    await ImageUploadProgressed.Invoke(new(fileInfo, 100));

                await UpdateFileEndedAsync(fileInfo, success, fileInfo.ErrorMessage);

                return;
            }

            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(ImageUploadAuthenticationSchema) && !string.IsNullOrWhiteSpace(ImageUploadAuthenticationToken))
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(ImageUploadAuthenticationSchema, ImageUploadAuthenticationToken);

                byte[] file = Convert.FromBase64String(fileInfo.ContentBase64);

                if (ImageUploadProgressed is not null)
                    await ImageUploadProgressed.Invoke(new(fileInfo, 25.0));

                using var form = new MultipartFormDataContent();
                using var fileContent = new ByteArrayContent(file);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                form.Add(fileContent, "file", fileInfo.Name);

                if (ImageUploadProgressed is not null)
                    await ImageUploadProgressed.Invoke(new(fileInfo, 50.0));

                try
                {
                    var response = await httpClient.PostAsync(ImageUploadEndpoint, form);
                    response.EnsureSuccessStatusCode();

                    if (ImageUploadProgressed is not null)
                        await ImageUploadProgressed.Invoke(new(fileInfo, 100));

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        fileInfo.UploadUrl = result;
                        success = true;
                    }
                    else
                        fileInfo.ErrorMessage = $"Error uploading the file {fileInfo.Name}";
                }
                catch
                {
                    fileInfo.ErrorMessage = $"Bad request uploading the file {fileInfo.Name}";
                }

                await UpdateFileEndedAsync(fileInfo, success, fileInfo.ErrorMessage);
            }
        }
        /// <summary>
        /// Notifies the custom button clicked.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [JSInvokable]
        public Task NotifyCustomButtonClicked(string name, object value)
        {
            return CustomButtonClicked.InvokeAsync(new MarkdownButtonEventArgs(name, value));
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

        /// <summary>
        /// Notifies the component that file input value has changed.
        /// </summary>
        /// <param name = "file">Changed file.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [JSInvokable]
        public async Task NotifyImageUpload(FileEntry file)
        {
            if (ImageUploadChanged is not null)
            {
                await ImageUploadChanged.Invoke(new(file));
            }

            if (JSModule is not null && string.IsNullOrEmpty(ImageUploadEndpoint))
            { 
                await JSModule.NotifyImageUploadError(ElementId, $"The property ImageUploadEndpoint is not specified.");
            }

            await InvokeAsync(StateHasChanged);
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
        /// Toggles fullscreen
        /// </summary>
        public async Task ToggleFullScreenAsync()
        {
            if (!Initialized || JSModule is null)
            {
                return;
            }
            await JSModule.ToggleFullScreen(ElementId);
        }

        /// <summary>
        /// Toggles preview mode
        /// </summary>
        public async Task TogglePreviewAsync()
        {
            if (_currentPreviewState == true)
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
            if (_currentPreviewState != wantedState)
            {
                _currentPreviewState = wantedState;
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
        /// Cleans all automatic save.
        /// </summary>
        public async Task CleanAllAutoSave()
        {
            if (JSModule is null)
            {
                return;
            }
            await JSModule.DeleteAllAutoSave();
        }

        /// <summary>
        /// Cleans the automatic save in the local storage in the browser
        /// </summary>
        public async Task CleanAutoSave()
        {
            if (JSModule is null || AutoSaveId is null)
            {
                return;
            }

            await JSModule.DeleteAutoSave(AutoSaveId);
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
        /// <summary>
        /// Updates the file ended asynchronous.
        /// </summary>
        /// <param name="fileEntry">The file entry.</param>
        /// <param name="success">if set to <c>true</c> [success].</param>
        /// <param name="fileInvalidReason">The file invalid reason.</param>
        public async Task UpdateFileEndedAsync(FileEntry fileEntry, bool success, string fileInvalidReason)
        {
            if (ImageUploadEnded is not null)
            {
                await ImageUploadEnded.Invoke(new(fileEntry, success, fileInvalidReason));
            }

            if (JSModule is null)
            {
                return;
            }
            if (success)
            {
                await JSModule.NotifyImageUploadSuccess(ElementId, fileEntry.UploadUrl);
            }
            else
            {
                await JSModule.NotifyImageUploadError(ElementId, fileEntry.ErrorMessage);
            }
        }

        /// <summary>
        /// Updates the file progress asynchronous.
        /// </summary>
        /// <param name="fileEntry">The file entry.</param>
        /// <param name="progressProgress">The progress progress.</param>
        /// <returns></returns>
        public Task UpdateFileProgressAsync(FileEntry fileEntry, long progressProgress)
        {
            ProgressProgress += progressProgress;

            var progress = Math.Round((double)ProgressProgress / ProgressTotal, 3);

            if (Math.Abs(progress - Progress) > double.Epsilon)
            {
                Progress = progress;

                if (ImageUploadProgressed is not null)
                {
                    return ImageUploadProgressed.Invoke(new(fileEntry, Progress));
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates the file started asynchronous.
        /// </summary>
        /// <param name="fileEntry">The file entry.</param>
        /// <returns></returns>
        public Task UpdateFileStartedAsync(FileEntry fileEntry)
        {
            // reset all
            ProgressProgress = 0;
            ProgressTotal = 100;
            Progress = 0;

            if (ImageUploadStarted is not null)
            {
                return ImageUploadStarted.Invoke(new(fileEntry));
            }

            return Task.CompletedTask;
        }
        #endregion


        /// <summary>
        /// Adds the custom toolbar button.
        /// </summary>
        /// <param name = "toolbarButton">Button instance.</param>
        protected internal void AddMarkdownToolbarButton(MarkdownToolbarButton toolbarButton)
        {
            toolbarButtons ??= new();
            toolbarButtons.Add(toolbarButton);
        }

        /// <summary>
        /// Removes the custom toolbar button.
        /// </summary>
        /// <param name = "toolbarButton">Button instance.</param>
        protected internal void RemoveMarkdownToolbarButton(MarkdownToolbarButton toolbarButton)
        {
            toolbarButtons?.Remove(toolbarButton);
        }

        protected override async Task OnParametersSetAsync()
        {
            // Save initial state
            if (_currentPreviewState is null)
            {
                _currentPreviewState = Preview;
            }
            if (_value is null)
            {
                _value = Value;
            }

            if (Initialized && _value != Value)
            {
                await SetValueAsync(Value ?? "");
            }

            if (Initialized && _currentPreviewState != Preview)
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

                string initialValue = this.Value ?? "";
                bool useChunkedSetValue = false;
                if(initialValue.Length > 30_000)
                {
                    useChunkedSetValue = true;
                    initialValue = "";
                }


                await JSModule.Initialize(dotNetObjectRef, ElementRef, ElementId, PreviewElementId, new
                {
                    AutoSave = new
                    {
                        enabled = AutoSaveEnabled,
                        uniqueId = AutoSaveId,
                        delay = AutoSaveDelay,
                        submit_delay = AutoSaveSubmitDelay,
                        timeFormat = new
                        {
                            locale = AutoSaveTimeFormatLocale,
                            format = new
                            {
                                year = AutoSaveTimeFormatYear,
                                month = AutoSaveTimeFormatMonth,
                                day = AutoSaveTimeFormatDay,
                                hour = AutoSaveTimeFormatHour,
                                minute = AutoSaveTimeFormatMinute
                            },
                        },
                        text = AutoSaveText
                    },
                    Value = initialValue,
                    AutoDownloadFontAwesome,
                    HideIcons,
                    ShowIcons,
                    LineNumbers,
                    LineWrapping,
                    MarkdownUrl,
                    MinHeight,
                    MaxHeight,
                    Placeholder,
                    TabSize,
                    Theme,
                    Direction,
                    NativeSpellChecker,
                    SpellChecker,
                    StatusTexts = new
                    {
                        Autosave = AutoSaveText,
                        Characters = CharactersStatusText,
                        Lines = LinesStatusText,
                        Words = WordsStatusText,
                    },
                    Toolbar = Toolbar != null && toolbarButtons?.Count > 0 ? MarkdownActionProvider.Serialize(toolbarButtons) : null,
                    ToolbarTips,
                    UploadImage,
                    ImageMaxSize,
                    ImageAccept,
                    ImageUploadEndpoint,
                    ImagePathAbsolute,
                    ImageCSRFToken,
                    EnableRunCode,
                    EnableCopyCodeToClipboard,
                    ImageTexts = ImageTexts == null ? null : new
                    {
                        SbInit = ImageTexts.Init,
                        SbOnDragEnter = ImageTexts.OnDragEnter,
                        SbOnDrop = ImageTexts.OnDrop,
                        SbProgress = ImageTexts.Progress,
                        SbOnUploaded = ImageTexts.OnUploaded,
                        ImageTexts.SizeUnits,
                    },
                    ErrorMessages,
                    Preview,
                });

                if (AllowResize != null && (bool)AllowResize)
                {
                    CSSClass += " resizable";
                    await JSModule.AllowResize(ElementId);
                }

                Initialized = true;

                if (useChunkedSetValue)
                {
                    await SetValueAsync(this.Value??"");
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

            if (string.IsNullOrEmpty(AutoSaveId))
            {
                AutoSaveId = ElementId;
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