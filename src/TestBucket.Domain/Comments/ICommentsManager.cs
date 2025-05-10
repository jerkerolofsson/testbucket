

namespace TestBucket.Domain.Comments;

public interface ICommentsManager
{
    Task AddCommentAsync(ClaimsPrincipal principal, Comment comment);
    Task DeleteCommentAsync(ClaimsPrincipal principal, Comment comment);
}