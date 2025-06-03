using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.Models;
using TestBucket.Domain.Issues.Models;

namespace TestBucket.Domain.Issues.Mapping;
internal static class IssueMapper
{
    public static void CopyTo(IssueDto src, LinkedIssue dest)
    {
        dest.ExternalSystemId = src.ExternalSystemId;
        dest.ExternalSystemName = src.ExternalSystemName;
        dest.ExternalDisplayId = src.ExternalDisplayId;
        dest.ExternalId = src.ExternalId;

        dest.Created = src.Created ?? DateTimeOffset.UtcNow;
        dest.Modified = src.Modified ?? DateTimeOffset.UtcNow;
        dest.Author = src.Author;
        dest.IssueType = src.IssueType;

        dest.Title = src.Title;
        dest.State = src.State;
        dest.Description = src.Description;
    }
    public static void CopyTo(IssueDto src, LocalIssue dest)
    {
        dest.ExternalSystemId = src.ExternalSystemId;
        dest.ExternalSystemName = src.ExternalSystemName;
        dest.ExternalDisplayId = src.ExternalDisplayId;
        dest.ExternalId = src.ExternalId;

        dest.Created = src.Created ?? DateTimeOffset.UtcNow;
        dest.Modified = src.Modified ?? DateTimeOffset.UtcNow;
        dest.Author = src.Author;
        dest.IssueType = src.IssueType;

        dest.Title = src.Title;
        dest.State = src.State;
        dest.MappedState = src.MappedState;
        dest.Description = src.Description;
    }
    public static void CopyTo(LinkedIssue src, IssueDto dest)
    {
        dest.ExternalSystemId = src.ExternalSystemId;
        dest.ExternalSystemName = src.ExternalSystemName;
        dest.ExternalDisplayId = src.ExternalDisplayId;
        dest.ExternalId = src.ExternalId;

        dest.Created = src.Created;
        dest.Modified = src.Modified;
        dest.Author = src.Author;
        dest.IssueType = src.IssueType;

        dest.Title = src.Title;
        dest.State = src.State;
        dest.Description = src.Description;
    }
    public static void CopyTo(LocalIssue src, IssueDto dest)
    {
        dest.ExternalSystemId = src.ExternalSystemId;
        dest.ExternalSystemName = src.ExternalSystemName;
        dest.ExternalDisplayId = src.ExternalDisplayId;
        dest.ExternalId = src.ExternalId;

        dest.Created = src.Created;
        dest.Modified = src.Modified;
        dest.Author = src.Author;
        dest.IssueType = src.IssueType;

        dest.Title = src.Title;
        dest.State = src.State;
        dest.MappedState = src.MappedState ?? Contracts.Issues.States.MappedIssueState.Other;
        dest.Description = src.Description;
    }
    public static LocalIssue ToDbo(this IssueDto src)
    {
        var dest = new LocalIssue();
        CopyTo(src, dest);
        return dest;
    }
    public static IssueDto ToDto(this LocalIssue src)
    {
        var dto = new IssueDto();
        CopyTo(src, dto);
        return dto;
    }
    public static IssueDto ToDto(this LinkedIssue src)
    {
        var dto = new IssueDto();
        CopyTo(src, dto);
        return dto;
    }


}
