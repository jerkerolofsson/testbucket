using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Octokit;

using TestBucket.Contracts.Code.Models;
using TestBucket.Contracts.Issues.Models;
using TestBucket.Contracts.Issues.States;

namespace TestBucket.Github.Mapping;
internal static class IssueMapper
{
    internal static IssueDto ToDto(this Issue issue, long externalSystemId)
    {
        var dto = new IssueDto
        {
            Url = issue.Url,
            Title = issue.Title,
            Description = issue.Body,
            ExternalId = issue.Number.ToString(),
            ExternalSystemName = ExtensionConstants.SystemName,
            ExternalSystemId = externalSystemId,
            Created = issue.CreatedAt,
            Modified = issue.UpdatedAt,
            ExternalDisplayId = "#" + issue.Number,
        };

        if (issue.State == Octokit.ItemState.Open)
        {
            dto.State = IssueStates.Open;
            dto.MappedState = MappedIssueState.Open;
        }
        if (issue.State == Octokit.ItemState.Closed)
        {
            dto.State = IssueStates.Closed;
            dto.MappedState = MappedIssueState.Closed;
        }

        return dto;
    }
}
