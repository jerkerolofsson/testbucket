using MudExtensions;

using TestBucket.Components.Environments.ViewModels;
using TestBucket.Components.Shared.Fields;
using TestBucket.Domain.TestAccounts.Helpers;
using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Components.TestAccounts.Dialogs;
public partial class CreateAccountDialog
{
    private readonly TestAccount _account = new TestAccount
    {
        Name = "New Account",
        Type = "",
        Owner = "server"
    };

    public bool ShowUsername => _account.Type == "email";
    public bool ShowPassword => _account.Type is "email" or "wifi";

    private string?[] _subTypes = [];
    private FieldDefinition[] _fieldDefinitions = [];
    private readonly Dictionary<string, FieldValue> _fields = [];
    private IEnumerable<FieldValue> Fields => _fields.Values;
    private List<EnvironmentVariable> _customVariables = [];

    private void AccountTypeChanged(string type)
    {
        _account.Type = type;
        _subTypes = TestAccountTemplateHelper.GetSubTypesForAccountType(type);
    }

    private Task<IEnumerable<string?>> SearchSubType(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value))
            return Task.FromResult(_subTypes.AsEnumerable());
        return Task.FromResult(_subTypes.Where(x => x?.Contains(value, StringComparison.InvariantCultureIgnoreCase) == true));
    }

    private Task<IEnumerable<string?>> SearchType(string value, CancellationToken token)
    {
        string?[] types = AccountTypes.All;
        if (string.IsNullOrEmpty(value))
            return Task.FromResult(types.AsEnumerable());
        return Task.FromResult(types.Where(x => x?.Contains(value, StringComparison.InvariantCultureIgnoreCase) == true));
    }

    private string? _errorMessage;

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    private void Close()
    {
        MudDialog.Close();
    }

    private int _page;

    private void OnActiveStepChanged(int page)
    {
        _page = page;
        _fieldDefinitions = TestAccountTemplateHelper.GetFieldDefinitionsForAccount(accountLoc, _account.Type, _account.SubType ?? "");

        // Remove fields no longer valid
        foreach (var fieldName in Fields.Select(x => x.FieldDefinition!.Name).ToList())
        {
            if (!_fieldDefinitions.Any(x => x.Name == fieldName))
            {
                _fields.Remove(fieldName);
            }
        }

        // Add missing fields
        foreach(var fieldDefinition in _fieldDefinitions)
        {
            if (!_fields.ContainsKey(fieldDefinition.Name))
            {
                _fields[fieldDefinition.Name] = new FieldValue { FieldDefinitionId = fieldDefinition.Id, FieldDefinition = fieldDefinition };
            }
        }
    }

    private void FinalStatusChanged(StepStatus status)
    {
        Submit();
    }

    private void Submit()
    {
        if (string.IsNullOrWhiteSpace(_account.Name))
        {
            _errorMessage = "Invalid name";
            return;
        }

        foreach (var field in _fields.Values)
        {
            _account.Variables[field.FieldDefinition!.Name] = field.GetValueAsString();
        }
        foreach(var variable in _customVariables)
        {
            _account.Variables[variable.Key] = variable.Value;  
        }

        _account.Enabled = true;
        MudDialog.Close(_account);
    }
}