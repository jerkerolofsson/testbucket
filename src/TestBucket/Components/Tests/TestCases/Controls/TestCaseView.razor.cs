
using MudBlazor.Utilities;

using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Contracts.Fields;
using TestBucket.Domain.Testing.Compiler;

namespace TestBucket.Components.Tests.TestCases.Controls;
public partial class TestCaseView
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
    private string? _descriptionText;
    private string? _previewText;
    private readonly List<CompilerError> _errors = new List<CompilerError>();

    public string? Text
    {
        get
        {
            if(_preview && _previewText is not null)
            {
                return _previewText;
            }
            return _descriptionText;
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
    private void BeginEdit()
    {
        _preview = false;
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
    private async Task RunTestAsync()
    {
        if (Test is null || Test.TestProjectId is null)
        {
            return;
        }
        var run = await testRunCreation.CreateTestRunAsync(Test.Name, Test.TestProjectId.Value, [Test.Id]);
        if (run is not null)
        {
            appNavigationManager.NavigateTo(run);
        }
    }

    private async Task SaveChangesAsync()
    {
        _preview = true;

        await CompilePreviewAsync();

        await TestChanged.InvokeAsync(Test);
    }

    private async Task DeleteTestCaseAsync()
    {
        if (Test is null)
        {
            return;
        }
        await testCaseEditorController.DeleteTestCaseAsync(Test);
    }


    protected override async Task OnParametersSetAsync()
    {
        if (_projectId != Test?.TestProjectId && Test?.TestProjectId is not null)
        {
            _projectId = Test?.TestProjectId;
            _fields = await fieldService.SearchDefinitionsAsync(new SearchFieldQuery() { ProjectId = Test?.TestProjectId, Target = FieldTarget.TestCase });
        }

        if (Test is not null)
        {
            if (Test.Description != _descriptionText)
            {
                this.StateHasChanged();
            }
        }
    }

    private async Task CompilePreviewAsync()
    {
        if (Test is not null)
        {
            var options = new CompilationOptions(Test, _descriptionText ?? "");
            var context = await testCaseEditorController.CompileAsync(options, _errors);
            _previewText = context?.CompiledText ?? options.Text;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Test is not null && _editor is not null) 
        {
            Test.Description ??= "";
            if (_descriptionText != Test.Description || _previewText is null)
            {
                _descriptionText = Test.Description;
                await CompilePreviewAsync();

                if (_editor is not null)
                {
                    await _editor.SetValueAsync(Text!);
                    this.StateHasChanged();
                }
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