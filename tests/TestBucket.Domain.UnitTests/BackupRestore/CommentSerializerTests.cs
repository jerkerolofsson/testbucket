using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Comments;
using TestBucket.Domain.Comments.Models;
using TestBucket.Domain.Export.Handlers;

namespace TestBucket.Domain.UnitTests.BackupRestore;

/// <summary>
/// Contains unit tests for the <see cref="CommentSerializer"/> class, verifying serialization and deserialization
/// of <see cref="Comment"/> and <see cref="CommentDto"/> objects for backup and restore scenarios.
/// </summary>
[Component("Backup")]
[UnitTest]
[EnrichedTest]
[FunctionalTest]
public class CommentSerializerTests
{
    /// <summary>
    /// Verifies that serializing a null input returns null.
    /// </summary>
    [Fact]
    public void Serialize_NullInput_ReturnsNull()
    {
        var result = CommentSerializer.Serialize(null);
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that serializing an empty list returns null.
    /// </summary>
    [Fact]
    public void Serialize_EmptyList_ReturnsNull()
    {
        var result = CommentSerializer.Serialize(new List<Comment>());
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that serializing a valid list of <see cref="Comment"/> maps all properties to <see cref="CommentDto"/>.
    /// </summary>
    [Fact]
    public void Serialize_ValidList_MapsAllProperties()
    {
        var now = DateTimeOffset.UtcNow;
        var comments = new List<Comment>
        {
            new Comment
            {
                Markdown = "Test",
                LoggedActionIcon = "icon",
                LoggedActionColor = "#fff",
                LoggedAction = "approved",
                LoggedActionArgument = "arg",
                Modified = now,
                Created = now.AddDays(-1),
                ModifiedBy = "user2",
                CreatedBy = "user1"
            }
        };

        var dtos = CommentSerializer.Serialize(comments);

        Assert.NotNull(dtos);
        Assert.Single(dtos!);
        var dto = dtos[0];
        Assert.Equal(comments[0].Markdown, dto.Markdown);
        Assert.Equal(comments[0].LoggedActionIcon, dto.LoggedActionIcon);
        Assert.Equal(comments[0].LoggedActionColor, dto.LoggedActionColor);
        Assert.Equal(comments[0].LoggedAction, dto.LoggedAction);
        Assert.Equal(comments[0].LoggedActionArgument, dto.LoggedActionArgument);
        Assert.Equal(comments[0].Modified, dto.Modified);
        Assert.Equal(comments[0].Created, dto.Created);
        Assert.Equal(comments[0].ModifiedBy, dto.ModifiedBy);
        Assert.Equal(comments[0].CreatedBy, dto.CreatedBy);
    }

    /// <summary>
    /// Verifies that deserializing a null input returns null.
    /// </summary>
    [Fact]
    public void Deserialize_NullInput_ReturnsNull()
    {
        var result = CommentSerializer.Deserialize(null);
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that deserializing an empty list returns null.
    /// </summary>
    [Fact]
    public void Deserialize_EmptyList_ReturnsNull()
    {
        var result = CommentSerializer.Deserialize(new List<CommentDto>());
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that deserializing a valid list of <see cref="CommentDto"/> maps all properties to <see cref="Comment"/>.
    /// </summary>
    [Fact]
    public void Deserialize_ValidList_MapsAllProperties()
    {
        var now = DateTimeOffset.UtcNow;
        var dtos = new List<CommentDto>
        {
            new CommentDto
            {
                Markdown = "Test",
                LoggedActionIcon = "icon",
                LoggedActionColor = "#fff",
                LoggedAction = "approved",
                LoggedActionArgument = "arg",
                Modified = now,
                Created = now.AddDays(-1),
                ModifiedBy = "user2",
                CreatedBy = "user1"
            }
        };

        var comments = CommentSerializer.Deserialize(dtos);

        Assert.NotNull(comments);
        Assert.Single(comments!);
        var comment = comments[0];
        Assert.Equal(dtos[0].Markdown, comment.Markdown);
        Assert.Equal(dtos[0].LoggedActionIcon, comment.LoggedActionIcon);
        Assert.Equal(dtos[0].LoggedActionColor, comment.LoggedActionColor);
        Assert.Equal(dtos[0].LoggedAction, comment.LoggedAction);
        Assert.Equal(dtos[0].LoggedActionArgument, comment.LoggedActionArgument);
        Assert.Equal(dtos[0].Modified, comment.Modified);
        Assert.Equal(dtos[0].Created, comment.Created);
        Assert.Equal(dtos[0].ModifiedBy, comment.ModifiedBy);
        Assert.Equal(dtos[0].CreatedBy, comment.CreatedBy);
    }

    /// <summary>
    /// Verifies that serializing and then deserializing a list of <see cref="Comment"/> preserves all data.
    /// </summary>
    [Fact]
    public void RoundTrip_SerializeAndDeserialize_PreservesData()
    {
        var now = DateTimeOffset.UtcNow;
        var original = new List<Comment>
        {
            new Comment
            {
                Markdown = "Test",
                LoggedActionIcon = "icon",
                LoggedActionColor = "#fff",
                LoggedAction = "approved",
                LoggedActionArgument = "arg",
                Modified = now,
                Created = now.AddDays(-1),
                ModifiedBy = "user2",
                CreatedBy = "user1"
            }
        };

        var dtos = CommentSerializer.Serialize(original);
        var roundTripped = CommentSerializer.Deserialize(dtos);

        Assert.NotNull(roundTripped);
        Assert.Single(roundTripped!);
        var comment = roundTripped[0];
        Assert.Equal(original[0].Markdown, comment.Markdown);
        Assert.Equal(original[0].LoggedActionIcon, comment.LoggedActionIcon);
        Assert.Equal(original[0].LoggedActionColor, comment.LoggedActionColor);
        Assert.Equal(original[0].LoggedAction, comment.LoggedAction);
        Assert.Equal(original[0].LoggedActionArgument, comment.LoggedActionArgument);
        Assert.Equal(original[0].Modified, comment.Modified);
        Assert.Equal(original[0].Created, comment.Created);
        Assert.Equal(original[0].ModifiedBy, comment.ModifiedBy);
        Assert.Equal(original[0].CreatedBy, comment.CreatedBy);
    }
}