
using TestBucket.Domain.Comments;
using TestBucket.Domain.Comments.Models;

namespace TestBucket.Components.Comments;

internal class CommentsController : TenantBaseService
{
    private readonly ICommentsManager _manager;

    public CommentsController(ICommentsManager manager, AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _manager = manager;
    }

    public async Task AddCommentAsync(Comment comment)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.AddCommentAsync(principal, comment);
    }
    public async Task DeleteCommentAsync(Comment comment)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.DeleteCommentAsync(principal, comment);
    }
}
