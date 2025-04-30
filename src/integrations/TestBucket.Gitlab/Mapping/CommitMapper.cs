using NGitLab.Models;

using TestBucket.Contracts.Code.Models;

namespace TestBucket.Gitlab.Mapping;
internal static class CommitMapper
{
    internal static CommitFileDto ToDto(this Diff diff, string sha)
    {
        return new CommitFileDto
        {
            Filename = diff.NewPath,
            Sha = sha,
            Deletions = 0,
            Additions = 0,
            Changes = 0,
        };
    }

    internal static CommitDto ToDto(this Commit commit)
    {
        var dto = new CommitDto
        {
            Url = commit.WebUrl,
            Message = commit.Message,
            CommiterEmail = commit.CommitterEmail,
            CommitDate = commit.CommittedDate,
            Sha = commit.Id.ToString(),
            Ref = commit.Id.ToString(),
        };

        return dto;
    }
}
