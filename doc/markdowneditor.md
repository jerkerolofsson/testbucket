# Test Bucket Blazor Markdown Editor

This is a fork of the Blazor Markdown Editor by Enrico Rossini (https://github.com/erossini/BlazorMarkdownEditor) with styling based on MudBlazor.

## Comparison to original

- Content is segmented allowing for larger documents exceeding the SignalR hub max message size.
- @bind-Value is working to update the markdown properly, there is no need to update the markdown directly with SetValueAsync

## Usage 

### App.razor

Add the following stylesheets to the head-element:
```html
    <link href="_content/TestBucket.MudBlazorExtensions.Markdown/css/markdowneditor.css" rel="stylesheet" />
    <link href="_content/TestBucket.MudBlazorExtensions.Markdown/css/easymde.min.css" rel="stylesheet" />
```

Add the following javascripts in the body element after ```<script src="_framework/blazor.web.js"></script>```:

```html11:59 2025-04-15
    <script src="_content/TestBucket.MudBlazorExtensions.Markdown/js/easymde.min.js"></script>
    <script src="_content/TestBucket.MudBlazorExtensions.Markdown/js/markdownEditor.js"></script>
    <script src="_content/TestBucket.MudBlazorExtensions.Markdown/js/highlight.min.js"></script>
    <script src="_content/TestBucket.MudBlazorExtensions.Markdown/js/mermaid.min.js"></script>
    <script> 
      mermaid.initialize({ startOnLoad: false });
    </script>
```