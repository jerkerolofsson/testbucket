
using TestBucket.Domain.Teams.Models;
using PSC.Blazor.Components.MarkdownEditor;

namespace TestBucket.Components.Tests.Controls;
public partial class TestCaseEditor
{
    [CascadingParameter] public TestProject? Project { get; set; }
    [CascadingParameter] public Team? Team { get; set; }
    [Parameter] public TestCase? Test { get; set; }
    [Parameter] public EventCallback<TestCase?> TestChanged { get; set; }

    private bool _preview = true;

    private void BeginEdit()
    {
        _preview = false;
    }

    private async Task OnRunCodeClickedAsync(string code)
    {
        if (Test is null || Test.TestProjectId is null)
        {
            return;
        }
        var run = await testRunCreation.CreateTestRunAsync(Test.Name, Test.TestProjectId.Value, [Test.Id]);
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

    private IReadOnlyList<FieldDefinition> _fields = [];

    private MarkdownEditor? _editor;
    private string? _boundDescription;

    private long? _projectId;

    protected override async Task OnParametersSetAsync()
    {
        if (_projectId != Test?.TestProjectId && Test?.TestProjectId is not null)
        {
            _projectId = Test?.TestProjectId;
            _fields = await fieldService.SearchDefinitionsAsync(new SearchFieldQuery() { ProjectId = Test?.TestProjectId, Target = FieldTarget.TestCase });
        }
    }

    protected override void OnParametersSet()
    {
        if (Test is null)
        {
            _boundDescription = null;
        }
        else
        {
            if (Test.Description != _boundDescription)
            {
                _boundDescription = null;
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Test is not null)
        {
            Test.Description ??= "";
            if (_boundDescription != Test.Description)
            {
                _boundDescription = Test.Description;

                if (_editor is not null)
                {
                    await _editor.SetValueAsync(_boundDescription);
                }
                this.StateHasChanged();
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

    public async Task OnDescriptionChanged(string description)
    {
        if (Test is not null)
        {
            _boundDescription = description;
            Test.Description = description;
            await TestChanged.InvokeAsync(Test);
        }
    }
}