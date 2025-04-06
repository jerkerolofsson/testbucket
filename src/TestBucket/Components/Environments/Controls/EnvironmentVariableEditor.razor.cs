using TestBucket.Components.Environments.ViewModels;

namespace TestBucket.Components.Environments.Controls;

public partial class EnvironmentVariableEditor
{
    [Parameter] public List<EnvironmentVariable>? Variables { get; set; } = [];
    [Parameter] public EventCallback<List<EnvironmentVariable>> VariablesChanged { get; set; }

    [Parameter] public bool Editing { get; set; } = false;

    [Parameter] public RenderFragment? ToolbarTitle { get; set; }

    private bool _editing = false;
    private bool _saveDisabled = true;

    private List<EnvironmentVariable>? _variables = new();

    private void BeginEdit()
    {
        _editing = true;
    }

    protected override void OnParametersSet()
    {
        _editing = Editing;
        if (Variables is null)
        {
            _variables = [];
        }
        else
        {
            _variables = Variables.ToList();
        }
    }

    private async Task SaveChangesAsync()
    {
        _editing = false;
        await VariablesChanged.InvokeAsync(_variables);
    }

    private void Delete(EnvironmentVariable? variable)
    {
        if (variable is not null)
        {
            _variables = (Variables ?? []).Where(x => x != variable).ToList();
            OnChanged();
        }
    }

    public void OnChanged()
    {
        _saveDisabled = false;
    }

    public void Add()
    {
        _editing = true;
        _variables ??= [];
        var variable = new EnvironmentVariable() { Key = "KEY", Value = "" };
        _variables.Add(variable);
        OnChanged();
    }
}
