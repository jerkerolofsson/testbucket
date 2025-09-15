using TestBucket.Contracts.Comments;
using TestBucket.Domain.Comments.Mapping;
using TestBucket.Domain.Comments.Models;

namespace TestBucket.Domain.UnitTests.Comments;

/// <summary>
/// Contains unit tests for <see cref="Comment"/> and <see cref="CommentDto"/> mapping operations.
/// Ensures that all properties are correctly mapped between domain and DTO objects.
/// </summary>
[UnitTest]
[FunctionalTest]
[EnrichedTest]
[Component("Comments")]
public class CommmentMapperTests
{
    /// <summary>
    /// Verifies that <see cref="Comment.ToDto"/> maps all properties from <see cref="Comment"/> to <see cref="CommentDto"/> correctly.
    /// </summary>
    [Fact]
    public void ToDto_MapsAllPropertiesCorrectly()
    {
        var now = DateTime.UtcNow;
        var comment = new Comment
        {
            Markdown = "Test markdown",
            LoggedActionIcon = "icon",
            LoggedActionColor = "#fff",
            LoggedAction = "approved",
            LoggedActionArgument = "arg",
            Modified = now,
            Created = now.AddDays(-1),
            ModifiedBy = "user2",
            CreatedBy = "user1"
        };

        var dto = comment.ToDto();

        Assert.Equal(comment.Markdown, dto.Markdown);
        Assert.Equal(comment.LoggedActionIcon, dto.LoggedActionIcon);
        Assert.Equal(comment.LoggedActionColor, dto.LoggedActionColor);
        Assert.Equal(comment.LoggedAction, dto.LoggedAction);
        Assert.Equal(comment.LoggedActionArgument, dto.LoggedActionArgument);
        Assert.Equal(comment.Modified, dto.Modified);
        Assert.Equal(comment.Created, dto.Created);
        Assert.Equal(comment.ModifiedBy, dto.ModifiedBy);
        Assert.Equal(comment.CreatedBy, dto.CreatedBy);
    }

    /// <summary>
    /// Verifies that <see cref="CommentDto.ToDbo"/> maps all properties from <see cref="CommentDto"/> to <see cref="Comment"/> correctly.
    /// </summary>
    [Fact]
    public void ToDbo_MapsAllPropertiesCorrectly()
    {
        var now = DateTime.UtcNow;
        var dto = new CommentDto
        {
            Markdown = "Test markdown",
            LoggedActionIcon = "icon",
            LoggedActionColor = "#fff",
            LoggedAction = "approved",
            LoggedActionArgument = "arg",
            Modified = now,
            Created = now.AddDays(-1),
            ModifiedBy = "user2",
            CreatedBy = "user1"
        };

        var comment = dto.ToDbo();

        Assert.Equal(dto.Markdown, comment.Markdown);
        Assert.Equal(dto.LoggedActionIcon, comment.LoggedActionIcon);
        Assert.Equal(dto.LoggedActionColor, comment.LoggedActionColor);
        Assert.Equal(dto.LoggedAction, comment.LoggedAction);
        Assert.Equal(dto.LoggedActionArgument, comment.LoggedActionArgument);
        Assert.Equal(dto.Modified, comment.Modified);
        Assert.Equal(dto.Created, comment.Created);
        Assert.Equal(dto.ModifiedBy, comment.ModifiedBy);
        Assert.Equal(dto.CreatedBy, comment.CreatedBy);
    }
}