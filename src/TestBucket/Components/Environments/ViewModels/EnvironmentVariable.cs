using System.Diagnostics.CodeAnalysis;

namespace TestBucket.Components.Environments.ViewModels;

public class EnvironmentVariable
{
    public required string Key { get; set; }
    public required string Value { get; set; }


    [SetsRequiredMembers]
    public EnvironmentVariable(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public EnvironmentVariable() { }
}
