using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TestBucket.Jira.Models;

public class JiraIssue
{
    public string? expand { get; set; }
    public string? id { get; set; }
    public string? self { get; set; }
    public string? key { get; set; }
    public Fields? fields { get; set; }
}


public class Component
{

}

public class Fields
{
    //public DateTimeOffset? statuscategorychangedate { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JiraIssueType? issuetype { get; set; }

    //public Component[]? components { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Project? project { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Content? description { get; set; }
    //public string[]? fixVersions { get; set; }
    //public Statuscategory? statusCategory { get; set; }
    //public object aggregatetimespent { get; set; }
    //public object resolution { get; set; }
    //public Timetracking? timetracking { get; set; }
    //public object customfield_10037 { get; set; }
    //public object security { get; set; }
    //public object[] attachment { get; set; }
    //public object aggregatetimeestimate { get; set; }
    //public object resolutiondate { get; set; }
    //public int workratio { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? summary { get; set; }

    //public Issuerestriction? issuerestriction { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JiraUser? creator { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? created { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JiraUser? reporter { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Aggregateprogress? aggregateprogress { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Priority? priority { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? labels { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? duedate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Progress? progress { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Votes? votes { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Comment? comment { get; set; }
    //public object[] issuelinks { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JiraUser? assignee { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Worklog? worklog { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? updated { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Status? status { get; set; }
}

public class Project
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? self { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? key { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? projectTypeKey { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? simplified { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Avatarurls? avatarUrls { get; set; }
}

public class Avatarurls
{

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? _48x48 { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? _24x24 { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? _16x16 { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? _32x32 { get; set; }
}

public class Statuscategory
{
    public string? self { get; set; }
    public int? id { get; set; }
    public string? key { get; set; }
    public string? colorName { get; set; }
    public string? name { get; set; }
}

public class Timetracking
{
}

public class Watches
{
    public string? self { get; set; }
    public int? watchCount { get; set; }
    public bool? isWatching { get; set; }
}

public class Issuerestriction
{
    public Issuerestrictions? issuerestrictions { get; set; }
    public bool shouldDisplay { get; set; }
}

public class Issuerestrictions
{
}



public class Aggregateprogress
{
    public int? progress { get; set; }
    public int? total { get; set; }
}

public class Priority
{
    public string? self { get; set; }
    public string? iconUrl { get; set; }
    public string? name { get; set; }
    public string? id { get; set; }
}

public class Progress
{
    public int? progress { get; set; }
    public int? total { get; set; }
}

public class Votes
{
    public string? self { get; set; }
    public int? votes { get; set; }
    public bool? hasVoted { get; set; }
}

public class Comment
{
    public Comment1[]? comments { get; set; }
    public string? self { get; set; }
    public int? maxResults { get; set; }
    public int? total { get; set; }
    public int? startAt { get; set; }
}

public class Comment1
{
    public string? self { get; set; }
    public string? id { get; set; }
    public JiraUser? author { get; set; }
    public Body? body { get; set; }
    public JiraUser? updateAuthor { get; set; }
    public DateTime? created { get; set; }
    public DateTime? updated { get; set; }
}

public class JiraUser
{
    public string? self { get; set; }
    public string? accountId { get; set; }
    public string? emailAddress { get; set; }
    public Avatarurls? avatarUrls { get; set; }
    public string? displayName { get; set; }
    public bool active { get; set; }
    public string? timeZone { get; set; }
    public string? accountType { get; set; }
}

public class Body
{
    public string? type { get; set; }
    public int version { get; set; }
    public Content[]? content { get; set; }
}

public class Content
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? version { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? type { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public string? text { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public Content[]? content { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ContentMark[]? marks { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? attrs { get; set; }
}

public class ContentMark
{
    public required string type { get; set; }
    public Dictionary<string, object>[]? attrs { get; set; }
}

public class Worklog
{
    public int startAt { get; set; }
    public int maxResults { get; set; }
    public int total { get; set; }
}

public class Status
{
    public string? self { get; set; }
    public string? description { get; set; }
    public string? iconUrl { get; set; }
    public string? name { get; set; }
    public string? id { get; set; }
    public Statuscategory? statusCategory { get; set; }
}
