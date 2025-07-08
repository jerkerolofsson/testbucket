using System.Text.Json.Serialization;

namespace TestBucket.Jira.Models;

public class JiraIssueUpdateBean
{

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Fields fields { get; set; } = new();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Update update { get; set; } = new();

    internal void SetSummary(string title)
    {
        update.summary = [new SetSummary() { set = title }];
    }

    internal void SetDescription(Content description)
    {
        update.description = [new SetDescription() { set = description }];
    }

    internal void RemoveLabel(string label)
    {
        update.labels ??= [];
        update.labels = [.. update.labels, new UpdateOperation() {  remove = new OperationRemove() { name = label } }];
    }

    internal void AddLabel(string label)
    {
        update.labels ??= [];
        update.labels = [.. update.labels, new UpdateOperation() { add = new OperationAdd() { name = label } }];
    }
}


public class OperationRemove
{
    public required string name { get; set; }
}

public class OperationAdd
{
    public required string name { get; set; }
}

public class OperationSet
{
    public required string name { get; set; }
}

public class UpdateOperation
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public OperationAdd? add { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public OperationSet? set { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public OperationRemove? remove { get; set; }
}

public class SetSummary
{
    public required string set { get; set; }
}
public class SetDescription
{
    public required Content set { get; set; }
}


public class Update
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public UpdateOperation[]? components { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public UpdateOperation[]? labels { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public UpdateOperation[]? assignee { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public SetSummary[]? summary { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public SetDescription[]? description { get; set; }
}
