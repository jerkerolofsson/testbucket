﻿
using System.Text;

using TestBucket.Contracts.Fields;

namespace TestBucket.Domain.Fields.Models;
public class SearchFieldQuery : SearchQuery
{
    public FieldTarget? Target { get; set; }
    public string? Name { get; set; }

    public string AsCacheKey()
    {
        var sb = new StringBuilder();
        if (Name is not null)
        {
            sb.Append("name=");
            sb.Append(Name);
        }
        if (TeamId is not null)
        {
            sb.Append("t=");
            sb.Append(TeamId);
        }
        if (Text is not null)
        {
            sb.Append("x=");
            sb.Append(Text);
        }
        if (ProjectId is not null)
        {
            sb.Append("p=");
            sb.Append(ProjectId);
        }
        if (Target is not null)
        {
            sb.Append("t=");
            sb.Append(Target);
        }
        sb.Append("offset=");
        sb.Append(Offset);
        sb.Append("count=");
        sb.Append(Count);

        return sb.ToString();
    }
}
