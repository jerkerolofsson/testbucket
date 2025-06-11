
using MudBlazor.Utilities;

using TestBucket.Components.Shared.Editor;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Contracts.Fields;
using TestBucket.Domain.Comments.Models;

namespace TestBucket.Components.Tests.TestCases.Controls;
public partial class TestCaseEditor
{
    protected string EditButtonClassname =>
    new CssBuilder("mud-markdown-toolbar-toggle-button")
        .AddClass($"mud-{ColorAlt.ToDescriptionString()}-selected", Color != Color.Default && Color != Color.Inherit)
        .AddClass($"mud-markdown-toolbar-toggle-button-selected", _preview != true)
        .Build();

    protected string PreviewButtonClassname =>
        new CssBuilder("mud-markdown-toolbar-toggle-button")
            .AddClass($"mud-{ColorAlt.ToDescriptionString()}-selected", Color != Color.Default && Color != Color.Inherit)
            .AddClass($"mud-markdown-toolbar-toggle-button-selected", _preview == true)
            .AddClass(" mr-5", true)
            .Build();

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }
    [Parameter] public Color Color { get; set; } = Color.Surface;
    [Parameter] public Color ColorAlt { get; set; } = Color.Tertiary;

    [CascadingParameter] public TestProject? Project { get; set; }
    [CascadingParameter] public Team? Team { get; set; }
    [CascadingParameter] public Tenant? Tenant { get; set; }
    [Parameter] public TestCase? Test { get; set; }
    [Parameter] public EventCallback<TestCase?> TestChanged { get; set; }

    private IReadOnlyList<FieldDefinition> _fields = [];
    private long? _projectId;

    private bool _preview = true;
    private MarkdownEditor? _editor;
    private MarkdownEditor? _preconditionsEditor;
    private string? _descriptionText;
    private string? _previewText;
    private string? _preconditionsText;
    private string? _previewPreconditions;
    private readonly List<CompilerError> _errors = new List<CompilerError>();
    private List<Comment> _comments = [];

    public ImageUploadHandler UploadHandler => new TestCaseImageUploadHandler(uploads, Test?.Id);

    public string? Text
    {
        get
        {
            if (_preview && _previewText is not null)
            {
                return _previewText;
            }
            return _descriptionText;
        }
    }

    public string? PreconditionsText
    {
        get
        {
            if (_preview && _previewPreconditions is not null)
            {
                return _previewPreconditions;
            }
            return _preconditionsText;
        }
    }

    public async Task OnPreconditionsChanged(string description)
    {
        if (_preview)
        {
            return;
        }
        if (Test is not null)
        {
            _preconditionsText = description;
            Test.Preconditions = description;
            await CompilePreviewAsync();

            await TestChanged.InvokeAsync(Test);
        }
    }
    public async Task OnDescriptionChanged(string description)
    {
        if (_preview)
        {
            return;
        }
        if (Test is not null)
        {
            _descriptionText = description;
            Test.Description = description;
            await CompilePreviewAsync();

            await TestChanged.InvokeAsync(Test);
        }
    }
  
    private async Task OnRunCodeClickedAsync(RunCodeRequest request)
    {
        if (Test is null || Test.TestProjectId is null || request.Code is null || request.Language is null)
        {
            return;
        }
        try
        {
            await testRunCreation.EvalMarkdownCodeAsync(Test, null, request.Language, request.Code);
        }
        catch(Exception ex)
        {
            snackbar.Add(ex.Message, Severity.Error, (options) => { });
        }
    }
 

    private long? _testId = null;

    protected override async Task OnParametersSetAsync()
    {
        if (_projectId != Test?.TestProjectId && Test?.TestProjectId is not null)
        {
            _projectId = Test?.TestProjectId;
            _fields = await fieldService.SearchDefinitionsAsync(new SearchFieldQuery() { ProjectId = Test?.TestProjectId, Target = FieldTarget.TestCase });
        }

        if (Test is not null)
        {
            if(_testId != Test.Id)
            {
                _comments = Test.Comments ?? [];

                _testId = Test.Id;

                _descriptionText = Test.Description;
                _preconditionsText = Test.Preconditions;
                await CompilePreviewAsync();
            }
        }
    }

    private async Task OnCommentAdded(Comment comment)
    {
        if (Test is not null)
        {
            comment.TeamId = Test.TeamId;
            comment.TestProjectId = Test.TestProjectId;
            comment.TestCaseId = Test.Id;
            _comments.Add(comment);
            await comments.AddCommentAsync(comment);
        }
    }
    private async Task OnCommentDeleted(Comment comment)
    {
        _comments.Remove(comment);
        await comments.DeleteCommentAsync(comment);
    }

    private async Task OnPreviewChanged(bool preview)
    {
        _preview = preview;
        if (_descriptionText is not null && _editor is not null)
        {
            await _editor.SetValueAsync(_descriptionText);
        }
        if (_preconditionsText is not null && _preconditionsEditor is not null)
        {
            await _preconditionsEditor.SetValueAsync(_preconditionsText);
        }
    }

    private async Task CompilePreviewAsync()
    {
        if (Test is not null)
        {
            var options = new CompilationOptions(Test, _descriptionText ?? "");
            var context = await testCaseEditorController.CompileAsync(options, _errors);
            var previewText = context?.CompiledText ?? _descriptionText;
            if (_previewText != previewText && previewText is not null)
            {
                _previewText = previewText;
                if(_preview && _editor is not null)
                {
                    await _editor.SetValueAsync(_previewText);
                }
            }
            else
            {
                _previewText = "";
            }

            options = new CompilationOptions(Test, _preconditionsText ?? "");
            context = await testCaseEditorController.CompileAsync(options, _errors);
            previewText = context?.CompiledText ?? _preconditionsText;
            if (_previewPreconditions != previewText && previewText is not null)
            {
                _previewPreconditions = previewText;
                if (_preview && _preconditionsEditor is not null)
                {
                    await _preconditionsEditor.SetValueAsync(_previewPreconditions);
                }
            }
            else
            {
                _previewPreconditions = "";
            }
        }
    }

    public async Task OnNameChanged(string name)
    {
        if (Test is not null)
        {
            Test.Name = name;
            await TestChanged.InvokeAsync(Test);
        }
    }

}