using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Octokit;

using TestBucket.Contracts.Code.Models;

namespace TestBucket.Github.Mapping;
internal static class CommitMapper
{
    internal static CommitDto ToDto(this GitHubCommit commit)
    {
        var dto = new CommitDto
        {
            Url = commit.Url,
            Message = commit.Commit?.Message,
            Label = commit.Label,
            Ref = commit.Ref ?? commit.Sha,
            Sha = commit.Sha,
        };

        if(commit.Files is not null)
        {
            dto.Files = new List<CommitFileDto>(commit.Files.Select(f => new CommitFileDto
            {
                Status = f.Status,
                PreviousFileName = f.PreviousFileName,
                RawUrl = f.RawUrl,
                Sha = f.Sha,
                BlobUrl = f.BlobUrl,
                ContentsUrl = f.ContentsUrl,
                Patch = f.Patch,
                Filename = f.Filename,
                Additions = f.Additions,
                Deletions = f.Deletions,
                Changes = f.Changes
            }));
        }

        return dto;
    }
}
