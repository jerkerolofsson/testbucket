using TestBucket.Contracts.Comments;

namespace TestBucket.Domain.Comments.Mapping;
public static class CommmentMapper
{
    public static CommentDto ToDto(this Comment comment)
    {
        return new CommentDto
        {
            Markdown = comment.Markdown,
            LoggedActionIcon = comment.LoggedActionIcon,
            LoggedActionColor = comment.LoggedActionColor,
            LoggedAction = comment.LoggedAction,
            LoggedActionArgument = comment.LoggedActionArgument,
            Modified = comment.Modified,
            Created = comment.Created,
            ModifiedBy = comment.ModifiedBy,
            CreatedBy = comment.CreatedBy,
        };
    }
    public static Comment ToDbo(this CommentDto comment)
    {
        return new Comment
        {
            Markdown = comment.Markdown,
            LoggedActionIcon = comment.LoggedActionIcon,
            LoggedActionColor = comment.LoggedActionColor,
            LoggedAction = comment.LoggedAction,
            LoggedActionArgument = comment.LoggedActionArgument,
            Modified = comment.Modified,
            Created = comment.Created,
            ModifiedBy = comment.ModifiedBy,
            CreatedBy = comment.CreatedBy,
        };
    }
}
