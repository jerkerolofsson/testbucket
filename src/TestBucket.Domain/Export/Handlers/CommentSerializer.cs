using TestBucket.Contracts.Comments;
using TestBucket.Domain.Comments.Mapping;

namespace TestBucket.Domain.Export.Handlers;
internal class CommentSerializer
{
    internal static List<CommentDto>? Serialize(List<Comment>? comments)
    {
        if (comments?.Count > 0)
        {
            var dtos = new List<CommentDto>(comments.Count);

            foreach (var comment in comments)
            {
                dtos.Add(comment.ToDto());
            }

            return dtos;
        }
        return null;
    }
    internal static List<Comment>? Deserialize(List<CommentDto>? comments)
    {
        if (comments?.Count > 0)
        {
            var dbos = new List<Comment>(comments.Count);

            foreach (var comment in comments)
            {
                dbos.Add(comment.ToDbo());
            }

            return dbos;
        }
        return null;
    }
}
