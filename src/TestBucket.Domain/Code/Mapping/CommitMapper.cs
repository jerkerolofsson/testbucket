using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Code.Models;
using TestBucket.Domain.Code.Models;

namespace TestBucket.Domain.Code.Mapping;
internal static class CommitMapper
{
    public static CommitFile ToDbo(this CommitFileDto dto)
    {
        return new CommitFile
        {
            Additions= dto.Additions,
            Deletions = dto.Deletions,
            Changes = dto.Changes,
            Sha = dto.Sha,
            Status = dto.Status,
            Path = dto.Filename,
        };
    }

    public static Commit ToDbo(this CommitDto commit, Repository repository)
    {
        var dbo = new Commit
        {
            Reference = commit.Ref,
            RepositoryId = repository.Id,
            Sha = commit.Sha,
            Message = commit.Message,
            Commited = commit.CommitDate,
            CommitedBy = commit.CommiterEmail,
        };
        if(commit.Files is not null)
        {
            dbo.CommitFiles = new List<CommitFile>(commit.Files.Select(x => x.ToDbo()));
        }
        return dbo;
    }
}
