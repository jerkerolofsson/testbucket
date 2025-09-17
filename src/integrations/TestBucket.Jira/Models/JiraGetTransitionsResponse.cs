using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TestBucket.Jira.Models;

public class JiraGetTransitionsResponse
{
    public Transition[]? transitions { get; set; }
}

public class Transition
{
    public required string id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public string? name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public Status? to { get; set; }
}