
using MudBlazor.Utilities;

using TestBucket.Components.Shared.Editor;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Contracts.Fields;
using TestBucket.Domain.Comments.Models;

namespace TestBucket.Components.Tests.TestCases.Controls;
public partial class TestCaseEditor
{
    [Parameter(CaptureUnmatchedValues = true)] public IDictionary<string, object>? AdditionalAttributes { get; set; }
    [Parameter] public Color Color { get; set; } = Color.Surface;
    [Parameter] public Color ColorAlt { get; set; } = Color.Tertiary;

    [CascadingParameter] public TestProject? Project { get; set; }
    [CascadingParameter] public Team? Team { get; set; }
    [CascadingParameter] public Tenant? Tenant { get; set; }
    [Parameter] public TestCase? Test { get; set; }
    [Parameter] public EventCallback<TestCase?> TestChanged { get; set; }

    [Parameter] public bool ReadOnly { get; set; } = false;

    public bool CanEdit => !ReadOnly;

    private IReadOnlyList<FieldDefinition> _fields = [];
    private long? _projectId;

    private MarkdownEditor? _editor;
    private MarkdownEditor? _preconditionsEditor;
    private MarkdownEditor? _postconditionsEditor;
    private string? _descriptionText;
    private string? _previewText;
    private string? _preconditionsText;
    private string? _previewPreconditions;
    private string? _postconditionsText;
    private string? _previewPostconditions;
    private readonly List<CompilerError> _errors = new List<CompilerError>();
    private List<Comment> _comments = [];

    public ImageUploadHandler UploadHandler => new TestCaseImageUploadHandler(uploads, Test?.Id);

    public string? Text
    {
        get
        {
            if (ReadOnly && _previewText is not null)
            {
                return _previewText;
            }
            return _descriptionText;
        }
    }

    public string? PostconditionsText
    {
        get
        {
            if (ReadOnly && _previewPostconditions is not null)
            {
                return _previewPostconditions;
            }
            return _postconditionsText;
        }
    }
    public string? PreconditionsText
    {
        get
        {
            if (ReadOnly && _previewPreconditions is not null)
            {
                return _previewPreconditions;
            }
            return _preconditionsText;
        }
    }

    public async Task OnPostconditionsChanged(string description)
    {
        if (ReadOnly)
        {
            return;
        }
        if (Test is not null)
        {
            _postconditionsText = description;
            Test.Postconditions = description;
            await CompilePreviewAsync();

            await TestChanged.InvokeAsync(Test);
        }
    }
    public async Task OnPreconditionsChanged(string description)
    {
        if (ReadOnly)
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
        if (ReadOnly)
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
                _postconditionsText = Test.Postconditions;
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

    //private async Task OnPreviewChanged(bool preview)
    //{
    //    _preview = preview;
    //    if (_descriptionText is not null && _editor is not null)
    //    {
    //        await _editor.SetValueAsync(_descriptionText);
    //    }
    //    if (_preconditionsText is not null && _preconditionsEditor is not null)
    //    {
    //        await _preconditionsEditor.SetValueAsync(_preconditionsText);
    //    }
    //    if (_postconditionsText is not null && _postconditionsEditor is not null)
    //    {
    //        await _postconditionsEditor.SetValueAsync(_postconditionsText);
    //    }
    //}

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
                if(ReadOnly && _editor is not null)
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
                if (ReadOnly && _preconditionsEditor is not null)
                {
                    await _preconditionsEditor.SetValueAsync(_previewPreconditions);
                }
            }
            else
            {
                _previewPreconditions = "";
            }

            options = new CompilationOptions(Test, _postconditionsText ?? "");
            context = await testCaseEditorController.CompileAsync(options, _errors);
            previewText = context?.CompiledText ?? _postconditionsText;
            if (_previewPostconditions != previewText && previewText is not null)
            {
                _previewPostconditions = previewText;
                if (ReadOnly && _postconditionsEditor is not null)
                {
                    await _postconditionsEditor.SetValueAsync(_previewPostconditions);
                }
            }
            else
            {
                _previewPostconditions = "";
            }
        }
    }

    private async Task OnSessionDurationChangedAsync(int? durationInMinutes)
    {
        if (Test is not null)
        {
            Test.SessionDuration = durationInMinutes;
            await TestChanged.InvokeAsync(Test);
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