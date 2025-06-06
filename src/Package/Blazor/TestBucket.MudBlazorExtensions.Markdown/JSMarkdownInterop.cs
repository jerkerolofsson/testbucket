﻿namespace TestBucket.MudBlazorExtensions.Markdown
{
    /// <summary>
    /// JSMarkdownInterop class
    /// </summary>
    public class JSMarkdownInterop
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        /// <summary>
        /// The js runtime
        /// </summary>
        private readonly IJSRuntime jsRuntime;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSMarkdownInterop"/> class.
        /// </summary>
        /// <param name="JSRuntime">The js runtime.</param>
        public JSMarkdownInterop(IJSRuntime JSRuntime)
        {
            jsRuntime = JSRuntime;

            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import",
                "./_content/PSC.Blazor.Components.MarkdownEditor/js/markdownEditor.js").AsTask());
        }

        /// <summary>
        /// Adds the CSS.
        /// </summary>
        /// <param name="targetUrl">The target URL.</param>
        /// <returns></returns>
        public async ValueTask AddCSS(string targetUrl)
        {
            await jsRuntime.InvokeVoidAsync("meLoadCSS", targetUrl);
        }

        /// <summary>
        /// Adds the js.
        /// </summary>
        /// <param name="targetUrl">The target URL.</param>
        /// <returns></returns>
        public async ValueTask AddJS(string targetUrl)
        {
            await jsRuntime.InvokeVoidAsync("meLoadJs", targetUrl);
        }

        /// <summary>
        /// Allows the resize or the textarea.
        /// </summary>
        /// <param name="Id">The identifier of the MarkEditor control.</param>
        /// <returns>ValueTask.</returns>
        public async ValueTask AllowResize(string Id)
        {
            await jsRuntime.InvokeVoidAsync("allowResize", Id);
        }

        /// <summary>
        /// Deletes the automatic save entry in the local storage.
        /// </summary>
        /// <returns>ValueTask.</returns>
        public async ValueTask DeleteAllAutoSave()
        {
            await jsRuntime.InvokeVoidAsync("deleteAllAutoSave");
        }

        /// <summary>
        /// Deletes the automatic save.
        /// </summary>
        /// <param name="autoSaveId">The automatic save identifier.</param>
        /// <returns>ValueTask.</returns>
        public async ValueTask DeleteAutoSave(string autoSaveId)
        {
            await jsRuntime.InvokeVoidAsync("deleteAutoSave", autoSaveId);
        }

        /// <summary>
        /// Destroys the specified element reference.
        /// </summary>
        /// <param name="elementRef">The element reference.</param>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        public async ValueTask Destroy(ElementReference elementRef, string elementId)
        {
            await jsRuntime.InvokeVoidAsync("destroy", elementRef, elementId);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        public async ValueTask<string> GetValue(string elementId)
        {
            return await jsRuntime.InvokeAsync<string>("getValue", elementId);
        }

        /// <summary>
        /// Initializes the specified dot net object reference.
        /// </summary>
        /// <param name="dotNetObjectRef">The dotnet object reference.</param>
        /// <param name="elementRef">The element reference.</param>
        /// <param name="elementId">The element identifier.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public async ValueTask Initialize(
            DotNetObjectReference<MarkdownEditor> dotNetObjectRef, 
            ElementReference elementRef,
            string elementId, string previewElementId, object options)
        {
            await jsRuntime.InvokeVoidAsync("initialize", dotNetObjectRef, elementRef, elementId, previewElementId, options);
        }
        /// <summary>
        /// Notifies the image upload error.
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public async ValueTask NotifyImageUploadError(string elementId, string errorMessage)
        {
            await jsRuntime.InvokeVoidAsync("notifyImageUploadError", elementId, errorMessage);
        }

        /// <summary>
        /// Notifies the image upload success.
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        /// <param name="imageUrl">The image URL.</param>
        /// <returns></returns>
        public async ValueTask NotifyImageUploadSuccess(string elementId, string imageUrl)
        {
            await jsRuntime.InvokeVoidAsync("notifyImageUploadSuccess", elementId, imageUrl);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public async ValueTask SetValue(string elementId, string value)
        {
            if(value.Length == 0)
            {
                await jsRuntime.InvokeVoidAsync("setValue", elementId, "");
                return;
            }

            // Should be SignalR HubOptions max length
            int maxLength = 30_000;
            int remaining = value.Length;
            int offset = 0;

            while(remaining > 0)
            {
                var chunkSize = Math.Min(maxLength, remaining);
                var chunk = value.Substring(offset, chunkSize);

                if (offset == 0)
                {
                    await jsRuntime.InvokeVoidAsync("setValue", elementId, chunk);
                }
                else
                {
                    await jsRuntime.InvokeVoidAsync("appendValue", elementId, chunk);
                }
                remaining -= chunkSize;
                offset += chunkSize;
            }

            //await jsRuntime.InvokeVoidAsync("setInitValue", elementId, value);
        }

        /// <summary>
        /// Toggles fullscreen
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        public async ValueTask ToggleFullScreen(string elementId)
        {
            await jsRuntime.InvokeVoidAsync("toggleFullScreen", elementId);
        }
        /// <summary>
        /// Enables or disables fullscreen
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="wantedFullscreen">true to set fullscreen</param>
        /// <returns></returns>
        public async ValueTask SetFullScreen(string elementId, bool wantedFullscreen)
        {
            await jsRuntime.InvokeVoidAsync("setFullScreen", elementId, wantedFullscreen);
        }

        /// <summary>
        /// Draw unordered list (ul)
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        public async ValueTask ToggleUnorderedList(string elementId)
        {
            await jsRuntime.InvokeVoidAsync("toggleUnorderedList", elementId);
        }
        /// <summary>
        /// Draw ordered list (ol)
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        public async ValueTask ToggleOrderedList(string elementId)
        {
            await jsRuntime.InvokeVoidAsync("toggleOrderedList", elementId);
        }

        /// <summary>
        /// Toggle code block
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        public async ValueTask ToggleCodeBlock(string elementId)
        {
            await jsRuntime.InvokeVoidAsync("toggleCodeBlock", elementId);
        }

        /// <summary>
        /// Draw table
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        public async ValueTask DrawTable(string elementId)
        {
            await jsRuntime.InvokeVoidAsync("drawTable", elementId);
        }

        /// <summary>
        /// Draw link
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        public async ValueTask DrawLink(string elementId)
        {
            await jsRuntime.InvokeVoidAsync("drawLink", elementId);
        }

        /// <summary>
        /// Draw image
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        public async ValueTask DrawImage(string elementId)
        {
            await jsRuntime.InvokeVoidAsync("drawImage", elementId);
        }


        /// <summary>
        /// Inserts markdown at the cursor position, replacing any selection
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        public async ValueTask InsertText(string elementId, string text)
        {
            await jsRuntime.InvokeVoidAsync("insertText", elementId, text);
        }

        /// <summary>
        /// Toggles italic
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        public async ValueTask ToggleItalic(string elementId)
        {
            await jsRuntime.InvokeVoidAsync("toggleItalic", elementId);
        }

        /// <summary>
        /// Toggles bold
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        public async ValueTask ToggleBold(string elementId)
        {
            await jsRuntime.InvokeVoidAsync("toggleBold", elementId);
        }

        /// <summary>
        /// Toggles preview mode.
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        public async ValueTask TogglePreview(string elementId)
        {
            await jsRuntime.InvokeVoidAsync("togglePreview", elementId);
        }

        /// <summary>
        /// Enables or disables the preview
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        /// <param name="wantedState">If true preview will be enabled</param>
        /// <returns></returns>
        public async ValueTask SetPreview(string elementId, bool wantedState)
        {
            await jsRuntime.InvokeVoidAsync("setPreview", elementId, wantedState);
        }
    }
}